namespace Narumikazuchi.Generators.ByteSerialization;

static public class Extensions
{
    static public String ToFrameworkString(this ITypeSymbol type)
    {
        String result = type.ToDisplayString()
                            .Replace("*", "");

        foreach (KeyValuePair<String, String> kv in s_BuiltInTypes)
        {
            result = result.Replace(kv.Key, kv.Value);
        }

        return result;
    }

    static public String ToFileString(this ITypeSymbol type)
    {
        String result = String.Empty;

        if (type is IArrayTypeSymbol array)
        {
            result = $"Array[{array.ElementType.ToFileString()}+{array.Rank}]";
        }
        else if (type is INamedTypeSymbol namedType)
        {
            if (namedType.IsGenericType)
            {
                result = namedType.ToDisplayString();
                result = result.Substring(0, result.IndexOf('<'));
                result += $"`{namedType.Arity}[";
                Boolean first = true;
                foreach (ITypeSymbol typeArgument in namedType.TypeArguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        result += "+";
                    }

                    result += typeArgument.ToFileString();
                }

                result += "]";
            }
            else
            {
                result = namedType.ToDisplayString();
            }
        }

        foreach (KeyValuePair<String, String> kv in s_BuiltInTypes)
        {
            result = result.Replace(kv.Key, kv.Value);
        }

        return result;
    }

    static public String ToNameString(this ITypeSymbol type)
    {
        String result = String.Empty;

        if (type is IArrayTypeSymbol array)
        {
            if (array.ElementType is IArrayTypeSymbol)
            {
                result += $"[{new String(Enumerable.Repeat(',', array.Rank - 1).ToArray())}]";
                ITypeSymbol element = array.ElementType;
                while (element is IArrayTypeSymbol elementArray)
                {
                    result += $"[{new String(Enumerable.Repeat(',', elementArray.Rank - 1).ToArray())}]";
                    element = elementArray.ElementType;
                }

                result = $"{element.ToNameString()}{result}";
            }
            else
            {
                result = $"{array.ElementType.ToNameString()}[{new String(Enumerable.Repeat(',', array.Rank - 1).ToArray())}]";
            }
        }
        else if (type is INamedTypeSymbol namedType)
        {
            result = namedType.ToDisplayString()
                              .Replace("?", "")
                              .Replace("<", "_")
                              .Replace(">", "_");
        }

        foreach (KeyValuePair<String, String> kv in s_BuiltInTypes)
        {
            result = result.Replace(kv.Key, kv.Value);
        }

        return result.Replace(".", "");
    }

    static public String CreateArray(this IArrayTypeSymbol array,
                                     params String[] sizes)
    {
        StringBuilder builder = new();
        if (array.ElementType is IArrayTypeSymbol elementArray)
        {
            Int32 bracketCount = 1;
            ITypeSymbol rootElement = elementArray.ElementType;
            while (rootElement is IArrayTypeSymbol rootElementArray)
            {
                rootElement = rootElementArray.ElementType;
                bracketCount++;
            }

            builder.Append($"new {rootElement.ToFrameworkString()}[");
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(sizes[index]);
            }

            builder.Append(']');
            while (bracketCount > 0)
            {
                builder.Append("[]");
                bracketCount--;
            }
        }
        else
        {
            builder.Append($"new {array.ElementType.ToFrameworkString()}[");
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(sizes[index]);
            }

            builder.Append(']');
        }

        return builder.ToString();
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

    static public Boolean ImplementsInterface(this ITypeSymbol type,
                                              ITypeSymbol @interface)
    {
        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(@interface, i));
    }

    static public Boolean ExtendsClass(this ITypeSymbol type,
                                       ITypeSymbol @base)
    {
        ITypeSymbol baseType = type.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, @base))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    static public ImmutableArray<INamedTypeSymbol> GetDerivedTypes(this ITypeSymbol type)
    {
        IAssemblySymbol assembly = type.ContainingAssembly;
        List<INamedTypeSymbol> builder = new();

        void ScanNamespace(INamespaceSymbol @namespace)
        {
            foreach (INamespaceOrTypeSymbol member in @namespace.GetMembers())
            {
                if (member is INamespaceSymbol namespaceSymbol)
                {
                    ScanNamespace(namespaceSymbol);
                }
                else if (member is INamedTypeSymbol typeSymbol)
                {
                    if (type.BaseType is null &&
                        typeSymbol.ImplementsInterface(type))
                    {
                        builder.Add(typeSymbol);
                    }
                    else if (typeSymbol.ExtendsClass(type))
                    {
                        builder.Add(typeSymbol);
                    }
                }
            }
        }

        Int32 SortBySealed(ITypeSymbol left,
                           ITypeSymbol right)
        {
            if (left.IsValueType &&
                !right.IsValueType)
            {
                return -1;
            }
            else if (!left.IsValueType &&
                     right.IsValueType)
            {
                return 1;
            }
            else if (left.IsSealed &&
                     !right.IsSealed)
            {
                return -1;
            }
            else if (!left.IsSealed &&
                     right.IsSealed)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        ScanNamespace(assembly.GlobalNamespace);
        builder.Sort(SortBySealed);
        return builder.ToImmutableArray();
    }

    static public Boolean CanBeSerialized(this ITypeSymbol type)
    {
        if (type.IsIntrinsicallySerializable(out _))
        {
            return true;
        }
        else if (type.IsUnmanagedSerializable())
        {
            return true;
        }
        else if (type.IsEnumerableSerializable())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static public Boolean IsIntrinsicallySerializable(this ITypeSymbol type,
                                                      out Int32 fixedSize)
    {
        String typename = type.ToFrameworkString();
        if (Array.IndexOf(array: __Shared.IntrinsicTypes,
                          value: typename) > -1)
        {
            fixedSize = __Shared.IntrinsicTypeFixedSize[typename];
            return true;
        }
        else
        {
            fixedSize = -1;
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