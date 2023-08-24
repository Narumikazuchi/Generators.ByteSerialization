using Microsoft.CodeAnalysis;
using Narumikazuchi.Generators.ByteSerialization;

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
            if (s_BuiltInTypes.Contains(type.Name))
            {
                result = $"System.{type.Name}";
                s_FrameworkStringCache.GetOrAdd(key: type,
                                                value: result);

                return result;

            }
            else if (type is IArrayTypeSymbol array)
            {
                StringBuilder builder = new();
                builder.Append(array.ElementType.ToFrameworkString());
                builder.Append('[');
                builder.Append(new String(Enumerable.Repeat(',', array.Rank - 1).ToArray()));
                builder.Append(']');

                result = builder.ToString();

                s_FrameworkStringCache.GetOrAdd(key: type,
                                                value: result);

                return result;
            }
            else
            {
                if (type is not INamedTypeSymbol named)
                {
                    return type.Name;
                }

                StringBuilder builder = new();
                if (type.ContainingType is not null)
                {
                    builder.Append(type.ContainingType.ToFrameworkString());
                }
                else if (type.ContainingNamespace is not null)
                {
                    builder.Append(type.ContainingNamespace.ToDisplayString());
                }

                builder.Append('.');
                builder.Append(type.Name);

                if (named.IsGenericType)
                {
                    builder.Append('<');
                    Boolean first = true;
                    foreach (ITypeSymbol typeArgument in named.TypeArguments)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            builder.Append(", ");
                        }

                        builder.Append(typeArgument.ToFrameworkString());
                    }

                    builder.Append('>');
                }

                result = builder.ToString();

                return result;
            }
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

    static public ImmutableArray<INamedTypeSymbol> GetDerivedTypes(this INamedTypeSymbol type,
                                                                   ImmutableArray<IAssemblySymbol> assemblies)
    {
        if (s_DerivedTypeCache.TryGetValue(key: type,
                                           value: out ImmutableArray<INamedTypeSymbol> result))
        {
            return result;
        }
        else
        {
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
                        if (typeSymbol.IsInterface())
                        {
                            continue;
                        }
                        else if (type.IsInterface() &&
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

            foreach (IAssemblySymbol assembly in assemblies)
            {
                ScanNamespace(assembly.GlobalNamespace);
            }

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
            baseType = baseType.BaseType;
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

    static private readonly ImmutableArray<String> s_BuiltInTypes = new String[]
    {
        "Decimal",
        "Double",
        "UInt16",
        "Object",
        "String",
        "Single",
        "SByte",
        "UIntPtr",
        "UInt64",
        "Int16",
        "Boolean",
        "Int64",
        "IntPtr",
        "UInt32",
        "Byte",
        "Char",
        "Int32"
    }.ToImmutableArray();
}