﻿namespace Narumikazuchi.Generators.ByteSerialization;

static internal class __Shared
{
    static internal Dictionary<ITypeSymbol, ITypeSymbol> GetStrategiesFromAttributes(INamedTypeSymbol symbol,
                                                                                     Compilation compilation,
                                                                                     TypeDeclarationSyntax type,
                                                                                     out Dictionary<ITypeSymbol, List<AttributeData>> duplicates)
    {
        AttributeData[] attributes = FetchUsefulAttributes(symbol: symbol,
                                                           compilation: compilation,
                                                           type: type);
        Dictionary<ITypeSymbol, ITypeSymbol> strategies = new(SymbolEqualityComparer.Default);
        duplicates = new(SymbolEqualityComparer.Default);

        foreach (AttributeData data in attributes)
        {
            ITypeSymbol targetType = data.AttributeClass!.TypeArguments[0];
            ITypeSymbol strategyType = data.AttributeClass!.TypeArguments[1];
            try
            {
                strategies.Add(targetType, strategyType);
            }
            catch
            {
                if (duplicates.TryGetValue(key: targetType,
                                           value: out List<AttributeData> values))
                {
                    values.Add(data);
                }
                else
                {
                    values = new()
                    {
                        data
                    };
                    AttributeData second = attributes.First(attribute => SymbolEqualityComparer.Default.Equals(targetType, attribute.AttributeClass!.TypeArguments[0]));
                    values.Add(second);
                    duplicates.Add(key: targetType,
                                   value: values);
                }
            }
        }

        return strategies;
    }

    static internal ImmutableArray<IFieldSymbol> GetFieldsToSerialize(INamedTypeSymbol symbol)
    {
        Boolean RecordParameterNotIgnored(IFieldSymbol field)
        {
            return ParameterNotIgnored(symbol: symbol,
                                       field: field);
        }

        ITypeSymbol fieldHost = symbol.BaseType;
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

        return floatingFields.ToImmutableArray();
    }

    static internal String SizeOf(ITypeSymbol type)
    {
        String typename = type.ToFrameworkString();
        if (type.TypeKind is TypeKind.Enum ||
            Array.IndexOf(array: s_BuiltInTypes,
                          value: typename) > -1)
        {
            return $"sizeof({typename})";
        }
        else
        {
            return $"Marshal.SizeOf<{typename}>()";
        }
    }

    static internal String[] IntrinsicTypes { get; } = new[]
    {
        typeof(DateTime).FullName!,
        typeof(DateTimeOffset).FullName!,
        nameof(String)
    };

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
                String fullName = attributeSymbol.ToFrameworkString();
                if (fullName.StartsWith(Generators.SerializableGenerator.USEBYTESERIALIZATIONSTRATEGY_ATTRIBUTE))
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

    static private Boolean ParameterNotIgnored(INamedTypeSymbol symbol,
                                               IFieldSymbol field)
    {
        if (!symbol.IsRecord ||
            field.AssociatedSymbol is null)
        {
            return true;
        }

        IMethodSymbol constructor = symbol.InstanceConstructors[0];
        IParameterSymbol parameter = constructor.Parameters.FirstOrDefault(parameter => parameter.Name == field.AssociatedSymbol.Name);
        if (parameter is null)
        {
            return true;
        }

        return !parameter.GetAttributes()
                         .Any(data => data.AttributeClass is not null &&
                                      data.AttributeClass.ToFrameworkString() is "Narumikazuchi.Generators.ByteSerialization.IngoreForSerializationAttribute");
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
                                                       data.AttributeClass.ToFrameworkString() is "Narumikazuchi.Generators.ByteSerialization.IngoreForSerializationAttribute");
        }
        else
        {
            return !field.GetAttributes()
                         .Any(data => data.AttributeClass is not null &&
                                      data.AttributeClass.ToFrameworkString() is "Narumikazuchi.Generators.ByteSerialization.IngoreForSerializationAttribute");
        }
    }

    static private readonly String[] s_BuiltInTypes = new String[]
    {
        nameof(Decimal),
        nameof(Double),
        nameof(UInt16),
        nameof(Single),
        nameof(SByte),
        nameof(UInt64),
        nameof(Int16),
        nameof(Boolean),
        nameof(Int64),
        nameof(UInt32),
        nameof(Byte),
        nameof(Char),
        nameof(Int32)
    };
}