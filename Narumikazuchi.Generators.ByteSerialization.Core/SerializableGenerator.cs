namespace Narumikazuchi.Generators.ByteSerialization.Generators;

[Generator]
public sealed partial class SerializableGenerator
{
    static private void GenerateSerializationMethods(SourceProductionContext context,
                                                     (Compilation, ImmutableArray<TypeDeclarationSyntax>) compilationAndClasses)
    {
        (Compilation compilation, ImmutableArray<TypeDeclarationSyntax> types) = compilationAndClasses;
        if (types.IsDefaultOrEmpty)
        {
            return;
        }

        IEnumerable<TypeDeclarationSyntax> distinctTypes = types.Distinct();

        foreach (TypeDeclarationSyntax type in distinctTypes)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(type.SyntaxTree);
            INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(type)!;
            AttributeData[] attributes = FetchUsefulAttributes(symbol: typeSymbol,
                                                               compilation: compilation,
                                                               type: type);
            Dictionary<ITypeSymbol, ITypeSymbol> strategies = ReadStrategies(attributes);
            try
            {
                GenerateSource(symbol: typeSymbol,
                               strategies: strategies,
                               semanticModel: semanticModel,
                               context: context);
            }
            catch { }
        }
    }

    static private Dictionary<ITypeSymbol, ITypeSymbol> ReadStrategies(AttributeData[] attributes)
    {
        Dictionary<ITypeSymbol, ITypeSymbol> strategies = new(SymbolEqualityComparer.Default);

        foreach (AttributeData data in attributes)
        {
            ITypeSymbol targetType = data.AttributeClass!.TypeArguments[0];
            ITypeSymbol strategyType = data.AttributeClass!.TypeArguments[1];
            try
            {
                strategies.Add(targetType, strategyType);
            }
            catch { }
        }

        return strategies;
    }

    static private AttributeData[] FetchUsefulAttributes(INamedTypeSymbol symbol,
                                                         Compilation compilation,
                                                         TypeDeclarationSyntax type)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(type.SyntaxTree);
        List<AttributeData> attributes = new();
        foreach (AttributeListSyntax attributeListSyntax in type.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (semanticModel.GetSymbolInfo(attributeSyntax: attributeSyntax)
                                 .Symbol is not IMethodSymbol constructorSymbol)
                {
                    continue;
                }

                INamedTypeSymbol attributeSymbol = constructorSymbol.ContainingType;
                String fullName = attributeSymbol.ToDisplayString();
                if (fullName.StartsWith(USEBYTESERIALIZATIONSTRATEGY_ATTRIBUTE))
                {
                    SyntaxReference reference = attributeSyntax.GetReference();
                    AttributeData data = symbol.GetAttributes()
                                                   .Single(x => reference.SyntaxTree == x.ApplicationSyntaxReference?.SyntaxTree &&
                                                                reference.Span == x.ApplicationSyntaxReference?.Span);
                    attributes.Add(data);
                }
            }
        }

        if (symbol.BaseType is null)
        {
            return attributes.ToArray();
        }
        else
        {
            foreach (SyntaxReference reference in symbol.BaseType.DeclaringSyntaxReferences)
            {
                TypeDeclarationSyntax typeDeclaration = (TypeDeclarationSyntax)reference.GetSyntax();
                if (typeDeclaration is ClassDeclarationSyntax
                                    or RecordDeclarationSyntax)
                {
                    attributes.AddRange(FetchUsefulAttributes(symbol: symbol.BaseType,
                                                              compilation: compilation,
                                                              type: typeDeclaration));
                }
            }
        }

        return attributes.ToArray();
    }

    static private void GenerateSource(INamedTypeSymbol symbol,
                                       Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                       SemanticModel semanticModel,
                                       SourceProductionContext context)
    {
        StringBuilder builder = new();
        builder.AppendLine($"namespace {symbol.ContainingNamespace.ToDisplayString()};");
        builder.AppendLine();

        String indent = String.Empty;
        INamedTypeSymbol? nested = symbol.ContainingType;
        while (nested is not null)
        {

            if (nested.IsStatic)
            {
                builder.Append($"{indent}static ");
            }

            switch (nested.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    builder.Append("public ");
                    break;
                case Accessibility.Internal:
                    builder.Append("internal ");
                    break;
            }

            if (nested.IsReferenceType)
            {
                builder.AppendLine($"partial class {nested.Name}");
            }
            else if (nested.IsValueType)
            {
                builder.AppendLine($"partial struct {nested.Name}");
            }

            builder.AppendLine($"{indent}{{");
            indent += "    ";
            nested = nested.ContainingType;
        }

        if (symbol.IsStatic)
        {
            builder.Append($"{indent}static ");
        }

        switch (symbol.DeclaredAccessibility)
        {
            case Accessibility.Public:
                builder.Append("public ");
                break;
            case Accessibility.Internal:
                builder.Append("internal ");
                break;
        }

        if (symbol.IsReferenceType)
        {
            builder.Append("partial ");
            if (symbol.IsRecord)
            {
                builder.Append("record ");
            }

            builder.AppendLine($"class {symbol.Name} : Narumikazuchi.Generators.ByteSerialization.IByteSerializable<{symbol.Name}>");
        }
        else if (symbol.IsValueType)
        {
            builder.Append("partial ");
            if (symbol.IsRecord)
            {
                builder.Append("record ");
            }

            builder.AppendLine($"struct {symbol.Name} : Narumikazuchi.Generators.ByteSerialization.IByteSerializable<{symbol.Name}>");
        }

        builder.AppendLine($"{indent}{{");
        indent += "    ";

        Boolean RecordParameterNotIgnored(IFieldSymbol field)
        {
            return ParameterNotIgnored(symbol: symbol,
                                       field: field);
        }

        ITypeSymbol? fieldHost = symbol.BaseType;
        IEnumerable<IFieldSymbol> floatingFields = symbol.GetMembers()
                                                         .OfType<IFieldSymbol>()
                                                         .Where(PropertyOrFieldNotIgnored)
                                                         .Where(RecordParameterNotIgnored);
        while (fieldHost is not null)
        {
            IEnumerable<IFieldSymbol> baseFields = fieldHost.GetMembers()
                                                            .OfType<IFieldSymbol>()
                                                            .Where(PropertyOrFieldNotIgnored)
                                                            .Where(RecordParameterNotIgnored);
            if (baseFields.Any())
            {
                floatingFields = floatingFields.Concat(baseFields);
            }

            fieldHost = fieldHost.BaseType;
        }

        ImmutableArray<IFieldSymbol> fields = floatingFields.ToImmutableArray();

        GenerateConstructorMediator(symbol: symbol,
                                    fields: fields,
                                    context: context);

        GenerateSuitableConstructor(symbol: symbol,
                                    fields: fields,
                                    context: context);

        GenerateDeserializeMethod(symbol: symbol,
                                  fields: fields,
                                  strategies: strategies,
                                  semanticModel: semanticModel,
                                  builder: builder,
                                  indent: indent);
        builder.AppendLine();

        GenerateGetExpectedByteSizeMethod(symbol: symbol,
                                          fields: fields,
                                          strategies: strategies,
                                          semanticModel: semanticModel,
                                          builder: builder,
                                          indent: indent);
        builder.AppendLine();

        GenerateSerializeMethod(symbol: symbol,
                                fields: fields,
                                strategies: strategies,
                                semanticModel: semanticModel,
                                builder: builder,
                                indent: indent);

        indent = indent.Substring(4);

        while (indent.Length > 0)
        {
            builder.AppendLine($"{indent}}}");
            indent = indent.Substring(4);
        }

        builder.Append("}");

        String meta = $@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#pragma warning disable
#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

";
        String source = meta + builder.ToString();

        SourceText text = SourceText.From(text: source,
                                          encoding: Encoding.UTF8);
        context.AddSource(hintName: $"{symbol.Name}.IByteSerializable.g.cs",
                          sourceText: text);
    }

    static private Boolean PropertyOrFieldNotIgnored(IFieldSymbol field)
    {
        if (field.IsStatic)
        {
            return true;
        }
        else if (field.AssociatedSymbol is not null)
        {
            return !field.AssociatedSymbol.GetAttributes()
                                          .Any(data => data.AttributeClass is not null &&
                                                       data.AttributeClass.ToDisplayString() is "Narumikazuchi.Generators.ByteSerialization.IngoreForSerializationAttribute");
        }
        else
        {
            return !field.GetAttributes()
                         .Any(data => data.AttributeClass is not null &&
                                      data.AttributeClass.ToDisplayString() is "Narumikazuchi.Generators.ByteSerialization.IngoreForSerializationAttribute");
        }
    }

    static private Boolean ParameterNotIgnored(INamedTypeSymbol symbol,
                                               IFieldSymbol field)
    {
        if (!symbol.IsRecord ||
            field.AssociatedSymbol is null)
        {
            return true;
        }

        IMethodSymbol constructor = symbol.InstanceConstructors[0];
        IParameterSymbol? parameter = constructor.Parameters.FirstOrDefault(parameter => parameter.Name == field.AssociatedSymbol.Name);
        if (parameter is null)
        {
            return true;
        }

        return !parameter.GetAttributes()
                         .Any(data => data.AttributeClass is not null &&
                                      data.AttributeClass.ToDisplayString() is "Narumikazuchi.Generators.ByteSerialization.IngoreForSerializationAttribute");
    }

    static private readonly String[] s_KnownTypes = new[]
    {
        nameof(Boolean),
        nameof(Byte),
        nameof(Char),
        "DateOnly",
        nameof(DateTime),
        nameof(DateTimeOffset),
        nameof(Decimal),
        nameof(Double),
        nameof(Guid),
        "Half",
        nameof(Int16),
        nameof(Int32),
        nameof(Int64),
        nameof(SByte),
        nameof(Single),
        nameof(String),
        "TimeOnly",
        nameof(TimeSpan),
        nameof(UInt16),
        nameof(UInt32),
        nameof(UInt64),
        nameof(Version)
    };

    private const String BYTESERIALIZABLE_ATTRIBUTE = "Narumikazuchi.Generators.ByteSerialization.ByteSerializableAttribute";
    private const String USEBYTESERIALIZATIONSTRATEGY_ATTRIBUTE = "Narumikazuchi.Generators.ByteSerialization.UseByteSerializationStrategy";
}