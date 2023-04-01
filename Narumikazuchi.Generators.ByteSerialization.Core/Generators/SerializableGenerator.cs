namespace Narumikazuchi.Generators.ByteSerialization.Generators;

[Generator]
public sealed partial class SerializableGenerator
{
    public const String BYTESERIALIZABLE_ATTRIBUTE = "Narumikazuchi.Generators.ByteSerialization.ByteSerializableAttribute";
    public const String FIXEDSERIALIZATIONSIZE_ATTRIBUTE = "Narumikazuchi.Generators.ByteSerialization.FixedSerializationSizeAttribute";
    public const String USEBYTESERIALIZATIONSTRATEGY_ATTRIBUTE = "Narumikazuchi.Generators.ByteSerialization.UseByteSerializationStrategy";

    static private void GenerateSerializationCode(SourceProductionContext context,
                                                  (Compilation, ImmutableArray<TypeDeclarationSyntax>) compilationAndClasses)
    {
        (Compilation compilation, ImmutableArray<TypeDeclarationSyntax> types) = compilationAndClasses;
        if (types.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (TypeDeclarationSyntax type in types)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(type.SyntaxTree);
            INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(type);

            Dictionary<ITypeSymbol, ITypeSymbol> strategies = __Shared.GetStrategiesFromAttributes(symbol: typeSymbol,
                                                                                                   compilation: compilation,
                                                                                                   type: type,
                                                                                                   duplicates: out Dictionary<ITypeSymbol, List<AttributeData>> duplicates);
            if (duplicates.Count is 0)
            {
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
    }

    static private void GenerateSource(INamedTypeSymbol symbol,
                                       Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                       SemanticModel semanticModel,
                                       SourceProductionContext context)
    {
        StringBuilder builder = new();
        builder.AppendLine($"namespace {symbol.ContainingNamespace.ToDisplayString()};");
        builder.AppendLine();

        ImmutableArray<IFieldSymbol> fields = __Shared.GetFieldsToSerialize(symbol);

        String indent = String.Empty;
        INamedTypeSymbol nested = symbol.ContainingType;
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

        GenerateConstructorMediator(symbol: symbol,
                                    fields: fields,
                                    context: context);

        GenerateSuitableConstructor(symbol: symbol,
                                    fields: fields,
                                    context: context);
        
        GenerateDeserializeMethod(symbol: symbol,
                                  fields: fields,
                                  strategies: strategies,
                                  builder: builder,
                                  indent: indent);
        builder.AppendLine();
        
        GenerateGetExpectedByteSizeMethod(symbol: symbol,
                                          fields: fields,
                                          strategies: strategies,
                                          builder: builder,
                                          indent: indent);
        builder.AppendLine();

        GenerateSerializeMethod(symbol: symbol,
                                fields: fields,
                                strategies: strategies,
                                builder: builder,
                                indent: indent);

        indent = indent.Substring(4);

        while (indent.Length > 0)
        {
            builder.AppendLine($"{indent}}}");
            indent = indent.Substring(4);
        }

        builder.Append("}");

        String meta = @"//------------------------------------------------------------------------------
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
}