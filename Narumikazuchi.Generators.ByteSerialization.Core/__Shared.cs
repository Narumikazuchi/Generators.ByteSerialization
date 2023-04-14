namespace Narumikazuchi.Generators.ByteSerialization;

static internal class __Shared
{
    static internal ImmutableArray<IFieldSymbol> GetFieldsToSerialize(INamedTypeSymbol symbol)
    {
        Boolean RecordParameterNotIgnored(IFieldSymbol field)
        {
            return ParameterNotIgnored(symbol: symbol,
                                       field: field);
        }

        IEnumerable<IFieldSymbol> floatingFields = symbol.GetMembers()
                                                         .OfType<IFieldSymbol>()
                                                         .Where(field => !field.IsStatic)
                                                         .Where(field => (field.AssociatedSymbol is null &&
                                                                         field.DeclaredAccessibility is Accessibility.Public
                                                                                                     or Accessibility.Internal) ||
                                                                         (field.AssociatedSymbol is not null &&
                                                                         field.AssociatedSymbol.DeclaredAccessibility is Accessibility.Public
                                                                                                                      or Accessibility.Internal))
                                                         .Where(field => field.Type.SpecialType is SpecialType.None
                                                                                                or SpecialType.System_Array
                                                                                                or SpecialType.System_Boolean
                                                                                                or SpecialType.System_Byte
                                                                                                or SpecialType.System_Char
                                                                                                or SpecialType.System_DateTime
                                                                                                or SpecialType.System_Decimal
                                                                                                or SpecialType.System_Double
                                                                                                or SpecialType.System_Enum
                                                                                                or SpecialType.System_Int16
                                                                                                or SpecialType.System_Int32
                                                                                                or SpecialType.System_Int64
                                                                                                or SpecialType.System_Nullable_T
                                                                                                or SpecialType.System_SByte
                                                                                                or SpecialType.System_Single
                                                                                                or SpecialType.System_String
                                                                                                or SpecialType.System_UInt16
                                                                                                or SpecialType.System_UInt32
                                                                                                or SpecialType.System_UInt64)
                                                         .Where(PropertyOrFieldNotIgnored)
                                                         .Where(RecordParameterNotIgnored);

        INamedTypeSymbol fieldHost = symbol.BaseType;
        while (fieldHost is not null)
        {
            IEnumerable<IFieldSymbol> baseFields = fieldHost.GetMembers()
                                                            .OfType<IFieldSymbol>()
                                                            .Where(field => !field.IsStatic)
                                                            .Where(field => (field.AssociatedSymbol is null &&
                                                                            field.DeclaredAccessibility is Accessibility.Public
                                                                                                        or Accessibility.Internal) ||
                                                                            (field.AssociatedSymbol is not null &&
                                                                            field.AssociatedSymbol.DeclaredAccessibility is Accessibility.Public
                                                                                                                         or Accessibility.Internal))
                                                            .Where(field => field.Type.SpecialType is SpecialType.None
                                                                                                   or SpecialType.System_Array
                                                                                                   or SpecialType.System_Boolean
                                                                                                   or SpecialType.System_Byte
                                                                                                   or SpecialType.System_Char
                                                                                                   or SpecialType.System_DateTime
                                                                                                   or SpecialType.System_Decimal
                                                                                                   or SpecialType.System_Double
                                                                                                   or SpecialType.System_Enum
                                                                                                   or SpecialType.System_Int16
                                                                                                   or SpecialType.System_Int32
                                                                                                   or SpecialType.System_Int64
                                                                                                   or SpecialType.System_Nullable_T
                                                                                                   or SpecialType.System_SByte
                                                                                                   or SpecialType.System_Single
                                                                                                   or SpecialType.System_String
                                                                                                   or SpecialType.System_UInt16
                                                                                                   or SpecialType.System_UInt32
                                                                                                   or SpecialType.System_UInt64)
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
            return $"Unsafe.SizeOf<{typename}>()";
        }
    }

    static internal String[] IntrinsicTypes { get; } = new String[]
    {
        typeof(DateTime).FullName!,
        typeof(DateTimeOffset).FullName!,
        nameof(String)
    };

    static internal Dictionary<String, Int32> IntrinsicTypeFixedSize { get; } = new()
    {
        { typeof(DateTime).FullName!, 8 },
        { typeof(DateTimeOffset).FullName!, 8 },
        { nameof(String), -1 }
    };

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