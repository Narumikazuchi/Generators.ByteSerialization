using Microsoft.CodeAnalysis;

namespace Narumikazuchi.CodeAnalysis;

static public class Extensions
{
    static public String ToFrameworkString(this ITypeSymbol type)
    {
        if (s_FrameworkStringCache.TryGetValue(key: type,
                                               value: out String result))
        {
            return result;
        }
        else
        {
            result = type.ToDisplayString()
                         .Replace("*", "");

            foreach (KeyValuePair<String, String> kv in s_BuiltInTypes)
            {
                result = result.Replace(kv.Key, kv.Value);
            }

            if ((type.ContainingNamespace is not null &&
                 type.ContainingNamespace.Name is "System") ||
                (result.Count(c => c == '.') is 1 &&
                result.StartsWith("System.")))
            {
                result = result.Replace("System.", "");
            }
            s_FrameworkStringCache.GetOrAdd(key: type,
                                            value: result);

            return result;
        }
    }

    static public Boolean IsOpenGenericType(this INamedTypeSymbol type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }
        else if (s_OpenGenericCache.TryGetValue(key: type,
                                                value: out Boolean result))
        {
            return result;
        }
        else
        {
            result = type.TypeArguments.Any(argument => argument.TypeKind is TypeKind.TypeParameter);
            s_OpenGenericCache.GetOrAdd(key: type,
                                        value: result);
            return result;
        }
    }

    static public ImmutableArray<INamedTypeSymbol> GetDerivedTypes(this INamedTypeSymbol type)
    {
        if (s_DerivedTypeCache.TryGetValue(key: type,
                                           value: out ImmutableArray<INamedTypeSymbol> result))
        {
            return result;
        }
        else
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
                    return right.BaseTypeCount()
                                .CompareTo(left.BaseTypeCount());
                }
            }

            ScanNamespace(assembly.GlobalNamespace);
            builder.Sort(SortBySealed);
            result = builder.ToImmutableArray();
            s_DerivedTypeCache.GetOrAdd(key: type,
                                        value: result);
            return result;
        }
    }

    static public Int32 BaseTypeCount(this ITypeSymbol type)
    {
        Int32 result = 0;
        INamedTypeSymbol baseType = type.BaseType;
        while (baseType is not null)
        {
            result++;
        }

        return result;
    }

    static public Boolean ImplementsInterface(this INamedTypeSymbol type,
                                              INamedTypeSymbol @interface)
    {
        InheritancePair pair = new(type, @interface);
        if (s_ImplementsInterfaceCache.TryGetValue(key: pair,
                                                   value: out Boolean result))
        {
            return result;
        }
        else
        {
            result = type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(@interface, i));
            s_ImplementsInterfaceCache.GetOrAdd(key: pair,
                                                value: result);
            return result;
        }
    }

    static public Boolean ExtendsClass(this INamedTypeSymbol type,
                                       INamedTypeSymbol @base)
    {
        InheritancePair pair = new(type, @base);
        if (s_ExtendsClassCache.TryGetValue(key: pair,
                                            value: out Boolean result))
        {
            return result;
        }
        else
        {
            ITypeSymbol baseType = type.BaseType;
            while (baseType is not null)
            {
                if (SymbolEqualityComparer.Default.Equals(baseType, @base))
                {
                    result = true;
                    break;
                }

                baseType = baseType.BaseType;
            }

            s_ExtendsClassCache.GetOrAdd(key: pair,
                                         value: result);
            return result;
        }
    }

    static public Boolean IsValidCSharpTypename(this String value)
    {
        UnicodeCategory category = Char.GetUnicodeCategory(value[0]);
        if (value[0] is not '_' &&
            category is not UnicodeCategory.UppercaseLetter
                     and not UnicodeCategory.LowercaseLetter
                     and not UnicodeCategory.TitlecaseLetter
                     and not UnicodeCategory.ModifierLetter
                     and not UnicodeCategory.OtherLetter)
        {
            return false;
        }

        foreach (Char character in value.AsSpan().Slice(1))
        {
            category = Char.GetUnicodeCategory(character);
            if (category is not UnicodeCategory.UppercaseLetter
                         and not UnicodeCategory.LowercaseLetter
                         and not UnicodeCategory.TitlecaseLetter
                         and not UnicodeCategory.ModifierLetter
                         and not UnicodeCategory.OtherLetter
                         and not UnicodeCategory.LetterNumber
                         and not UnicodeCategory.OtherNumber
                         and not UnicodeCategory.NonSpacingMark
                         and not UnicodeCategory.SpacingCombiningMark
                         and not UnicodeCategory.ConnectorPunctuation
                         and not UnicodeCategory.Format)
            {
                return false;
            }
        }

        return true;
    }

    static public String ToValidCSharpTypename(this String value)
    {
        StringBuilder result = new();
        for (Int32 index = 0;
             index < value.Length;
             index++)
        {
            if (index is 0)
            {
                UnicodeCategory category = Char.GetUnicodeCategory(value[index]);
                if (value[index] is not '_' &&
                    category is not UnicodeCategory.UppercaseLetter
                            and not UnicodeCategory.LowercaseLetter
                            and not UnicodeCategory.TitlecaseLetter
                            and not UnicodeCategory.ModifierLetter
                            and not UnicodeCategory.OtherLetter)
                {
                    result.Append('_');
                }
                else
                {
                    result.Append(value[index]);
                }
            }
            else
            {
                UnicodeCategory category = Char.GetUnicodeCategory(value[index]);

                if (category is not UnicodeCategory.UppercaseLetter
                             and not UnicodeCategory.LowercaseLetter
                             and not UnicodeCategory.TitlecaseLetter
                             and not UnicodeCategory.ModifierLetter
                             and not UnicodeCategory.OtherLetter
                             and not UnicodeCategory.LetterNumber
                             and not UnicodeCategory.OtherNumber
                             and not UnicodeCategory.NonSpacingMark
                             and not UnicodeCategory.SpacingCombiningMark
                             and not UnicodeCategory.ConnectorPunctuation
                             and not UnicodeCategory.Format)
                {
                    result.Append('_');
                }
                else
                {
                    result.Append(value[index]);
                }
            }
        }

        return result.ToString();
    }

    static public void ClearCaches()
    {
        s_DerivedTypeCache.Clear();
        s_ExtendsClassCache.Clear();
        s_FrameworkStringCache.Clear();
        s_ImplementsInterfaceCache.Clear();
        s_OpenGenericCache.Clear();
    }

    static private readonly ConcurrentDictionary<ITypeSymbol, String> s_FrameworkStringCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, Boolean> s_OpenGenericCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, ImmutableArray<INamedTypeSymbol>> s_DerivedTypeCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<InheritancePair, Boolean> s_ImplementsInterfaceCache = new();
    static private readonly ConcurrentDictionary<InheritancePair, Boolean> s_ExtendsClassCache = new();

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