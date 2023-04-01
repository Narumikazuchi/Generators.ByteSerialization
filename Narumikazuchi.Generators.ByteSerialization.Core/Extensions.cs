namespace Narumikazuchi.Generators.ByteSerialization;

static public class Extensions
{
    static public String ToFrameworkString(this ITypeSymbol type)
    {
        String result = type.ToDisplayString();

        foreach (KeyValuePair<String, String> kv in s_BuiltInTypes)
        {
            result = result.Replace(kv.Key, kv.Value);
        }

        return result;
    }

    static public String EnumerableCount(this ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol ||
            type.ToFrameworkString().StartsWith("System.Collections.Immutable.ImmutableArray<"))
        {
            return ".Length";
        }
        else
        {
            return ".Count";
        }
    }

    static public Boolean CanBeSerialized(this ITypeSymbol type,
                                          Dictionary<ITypeSymbol, ITypeSymbol> strategies = default)
    {
        if (strategies is not null &&
            strategies.ContainsKey(type))
        {
            return true;
        }
        else if (type.IsIntrinsicallySerializable())
        {
            return true;
        }
        else if (type.IsUnmanagedSerializable())
        {
            return true;
        }
        else if (type.IsByteSerializable())
        {
            return true;
        }
        else if (type.IsEnumerableSerializable(strategies))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static public Boolean IsIntrinsicallySerializable(this ITypeSymbol type)
    {
        if (Array.IndexOf(array: __Shared.IntrinsicTypes,
                          value: type.ToFrameworkString()) > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static public Boolean IsUnmanagedSerializable(this ITypeSymbol type)
    {
        if (type.IsUnmanagedType &&
            type.TypeKind is not TypeKind.Pointer &&
            type.Name is not "IntPtr"
                      and not "UIntPtr")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static public Boolean IsByteSerializable(this ITypeSymbol type)
    {
        return type.GetAttributes().Any(attribute => attribute.AttributeClass is not null &&
                                                     attribute.AttributeClass.ToFrameworkString() is Generators.SerializableGenerator.BYTESERIALIZABLE_ATTRIBUTE);
    }

    static public Boolean IsEnumerableSerializable(this ITypeSymbol type,
                                                   Dictionary<ITypeSymbol, ITypeSymbol> strategies = default)
    {
        if (type.IsDictionaryEnumerable(out INamedTypeSymbol keyValuePair))
        {
            ITypeSymbol key = keyValuePair.TypeArguments[0];
            ITypeSymbol value = keyValuePair.TypeArguments[1];
            if (!key.CanBeSerialized(strategies))
            {
                return false;
            }
            else if (!value.CanBeSerialized(strategies))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (type.IsEnumerable(out INamedTypeSymbol elementType))
        {
            return elementType.CanBeSerialized(strategies);
        }
        else
        {
            return false;
        }
    }

    static public Boolean IsEnumerable(this ITypeSymbol type,
                                       out INamedTypeSymbol elementType)
    {
        if (type is IArrayTypeSymbol array)
        {
            elementType = (INamedTypeSymbol)array.ElementType;
            return true;
        }
        else
        {
            foreach (String enumerableType in s_EnumerableTypes)
            {
                if (type.ToFrameworkString()
                        .StartsWith(enumerableType))
                {
                    elementType = (INamedTypeSymbol)type.AllInterfaces.First(@interface => @interface.ToFrameworkString()
                                                                                                     .StartsWith("System.Collections.Generic.IEnumerable<"))
                                                                      .TypeArguments[0];
                    return true;
                }
            }

            elementType = null;
            return false;
        }
    }

    static public Boolean IsDictionaryEnumerable(this ITypeSymbol type,
                                                 out INamedTypeSymbol keyValuePair)
    {
        foreach (String enumerableType in s_DictionaryTypes)
        {
            if (type.ToFrameworkString()
                    .StartsWith(enumerableType))
            {
                keyValuePair = (INamedTypeSymbol)type.AllInterfaces.First(@interface => @interface.ToFrameworkString()
                                                                                                  .StartsWith("System.Collections.Generic.IEnumerable<"))
                                                                   .TypeArguments[0];
                return true;
            }
        }

        keyValuePair = null;
        return false;
    }

    static private readonly String[] s_EnumerableTypes = new String[]
    {
        "System.Collections.Generic.Dictionary<",
        "System.Collections.Generic.HashSet<",
        "System.Collections.Generic.List<",
        "System.Collections.Generic.SortedDictionary<",
        "System.Collections.Generic.SortedList<",
        "System.Collections.Generic.SortedSet<",
        "System.Collections.Immutable.ImmutableArray<",
        "System.Collections.Immutable.ImmutableDictionary<",
        "System.Collections.Immutable.ImmutableHashSet<",
        "System.Collections.Immutable.ImmutableList<",
        "System.Collections.Immutable.ImmutableSortedDictionary<",
        "System.Collections.Immutable.ImmutableSortedSet<",
    };

    static private readonly String[] s_DictionaryTypes = new String[]
    {
        "System.Collections.Generic.Dictionary<",
        "System.Collections.Generic.SortedDictionary<",
        "System.Collections.Generic.SortedList<",
        "System.Collections.Immutable.ImmutableDictionary<",
        "System.Collections.Immutable.ImmutableSortedDictionary<"
    };

    static private readonly Dictionary<String, String> s_BuiltInTypes = new()
    {
        { "decimal", "Decimal" },
        { "double", "Double" },
        { "ushort", "UInt16" },
        { "object", "Object" },
        { "string", "String" },
        { "float", "Single" },
        { "sbyte", "SByte" },
        { "nuint", "UIntPtr" },
        { "ulong", "UInt64" },
        { "short", "Int16" },
        { "bool", "Boolean" },
        { "long", "Int64" },
        { "nint", "IntPtr" },
        { "uint", "UInt32" },
        { "byte", "Byte" },
        { "char", "Char" },
        { "int", "Int32" },
    };
}