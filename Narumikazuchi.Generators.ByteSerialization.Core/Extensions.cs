using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

static public class Extensions
{
    static Extensions()
    {
        s_BuiltInTypes = new();
        s_BuiltInTypes.GetOrAdd(key: "decimal",
                                value: "Decimal");
        s_BuiltInTypes.GetOrAdd(key: "double",
                                value: "Double");
        s_BuiltInTypes.GetOrAdd(key: "ushort",
                                value: "UInt16");
        s_BuiltInTypes.GetOrAdd(key: "object",
                                value: "Object");
        s_BuiltInTypes.GetOrAdd(key: "string",
                                value: "String");
        s_BuiltInTypes.GetOrAdd(key: "float",
                                value: "Single");
        s_BuiltInTypes.GetOrAdd(key: "sbyte",
                                value: "SByte");
        s_BuiltInTypes.GetOrAdd(key: "ulong",
                                value: "UInt64");
        s_BuiltInTypes.GetOrAdd(key: "short",
                                value: "Int16");
        s_BuiltInTypes.GetOrAdd(key: "bool",
                                value: "Boolean");
        s_BuiltInTypes.GetOrAdd(key: "long",
                                value: "Int64");
        s_BuiltInTypes.GetOrAdd(key: "uint",
                                value: "UInt32");
        s_BuiltInTypes.GetOrAdd(key: "byte",
                                value: "Byte");
        s_BuiltInTypes.GetOrAdd(key: "char",
                                value: "Char");
        s_BuiltInTypes.GetOrAdd(key: "int",
                                value: "Int32");
    }

    static public String ToFileString(this ITypeSymbol type)
    {
        if (s_FileStringCache.TryGetValue(key: type,
                                          value: out String result))
        {
            return result;
        }
        else
        {
            result = String.Empty;

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
                    result += $"`{namedType.Arity}";
                    if (namedType.TypeArguments[0].TypeKind is not TypeKind.TypeParameter)
                    {
                        result += "[";
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

            s_FileStringCache.GetOrAdd(key: type,
                                       value: result);

            return result;
        }
    }

    static public String CreateArray(this IArrayTypeSymbol array,
                                     params String[] sizes)
    {
        StringBuilder builder = new();
        if (array.ElementType is IArrayTypeSymbol elementArray)
        {
            ITypeSymbol rootElement = elementArray.ElementType;
            while (rootElement is IArrayTypeSymbol rootElementArray)
            {
                rootElement = rootElementArray.ElementType;
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
            rootElement = array.ElementType;
            while (rootElement is IArrayTypeSymbol rootElementArray)
            {
                builder.Append("[");
                builder.Append(new String(Enumerable.Repeat(',', rootElementArray.Rank - 1).ToArray()));
                builder.Append("]");
                rootElement = rootElementArray.ElementType;
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

    static public Boolean HasDefaultConstructor(this INamedTypeSymbol type)
    {
        if (s_HasDefaultConstructorCache.TryGetValue(key: type,
                                                     value: out Boolean result))
        {
            return result;
        }
        else
        {
            result = type.InstanceConstructors.Any(method => method.DeclaredAccessibility is Accessibility.Public &&
                                                             method.Parameters.Length is 0);
            s_HasDefaultConstructorCache.GetOrAdd(key: type,
                                                  value: result);
            return result;
        }
    }

    static public Boolean CanBeSerialized(this ITypeSymbol type)
    {
        if (s_CanBeSerializedCache.TryGetValue(key: type,
                                               value: out Boolean result))
        {
            return result;
        }
        else if (type.SpecialType is SpecialType.System_String ||
                 type.IsUnmanagedSerializable())
        {
            s_CanBeSerializedCache.GetOrAdd(key: type,
                                            value: true);
            return true;
        }
        else
        {
            s_CanBeSerializedCache.GetOrAdd(key: type,
                                            value: false);
            return false;
        }
    }

    static public Boolean IsInterface(this ITypeSymbol type)
    {
        if (type is INamedTypeSymbol named)
        {
            return named.IsAbstract &&
                   named.BaseType is null;
        }
        else
        {
            return false;
        }
    }

    static public Boolean IsSerializationHandler(this INamedTypeSymbol type)
    {
        if (s_IsSerializationHandlerCache.TryGetValue(key: type,
                                                      value: out Boolean result))
        {
            return result;
        }
        else if (type.BaseType is null &&
                 (type.ToFrameworkString().StartsWith($"{GlobalNames.NAMESPACE}.ISimpleSerializationHandler<") ||
                 type.ToFrameworkString().StartsWith($"{GlobalNames.NAMESPACE}.IComplexSerializationHandler<")))
        {
            s_IsSerializationHandlerCache.GetOrAdd(key: type,
                                                   value: true);
            return true;
        }
        else
        {
            s_IsSerializationHandlerCache.GetOrAdd(key: type,
                                                   value: false);
            return false;
        }
    }

    static public Boolean IsUnmanagedSerializable(this ITypeSymbol type)
    {
        if (s_IsUnmanagedCache.TryGetValue(key: type,
                                           value: out Boolean result))
        {
            return result;
        }
        else if (type.IsUnmanagedType &&
                 type.TypeKind is not TypeKind.Pointer &&
                 type.Name is not "IntPtr"
                           and not "UIntPtr")
        {
            s_IsUnmanagedCache.GetOrAdd(key: type,
                                        value: true);
            return true;
        }
        else
        {
            s_IsUnmanagedCache.GetOrAdd(key: type,
                                        value: false);
            return false;
        }
    }

    static public Boolean IsCollection(this INamedTypeSymbol type,
                                       out ITypeSymbol elementType)
    {
        if (s_IsCollectionCache.TryGetValue(key: type,
                                            value: out elementType))
        {
            return elementType is not null;
        }
        else
        {
            INamedTypeSymbol collection = type.AllInterfaces.FirstOrDefault(@interface => @interface.ToFrameworkString()
                                                                                                    .StartsWith("System.Collections.Generic.ICollection<"));
            if (collection is null)
            {
                elementType = null;
                s_IsCollectionCache.GetOrAdd(key: type,
                                              value: null);
                return false;
            }
            else
            {
                elementType = collection.TypeArguments[0];
                s_IsCollectionCache.GetOrAdd(key: type,
                                             value: collection.TypeArguments[0]);
                return true;
            }
        }
    }
    
    static internal ImmutableArray<ISymbol> GetMembersToSerialize(this INamedTypeSymbol type)
    {
        Boolean RecordParameterNotIgnored(IFieldSymbol field)
        {
            return ParameterNotIgnored(symbol: type,
                                       field: field);
        }

        if (s_MemberCache.TryGetValue(key: type,
                                      value: out ImmutableArray<ISymbol> result))
        {
            return result;
        }

        IEnumerable<IFieldSymbol> floatingFields = type.GetMembers()
                                                       .OfType<IFieldSymbol>()
                                                       .Where(field => !field.IsStatic)
                                                       .Where(field => !field.IsReadOnly)
                                                       .Where(field => field.DeclaredAccessibility is Accessibility.Public)
                                                       .Where(field => field.Type.SpecialType is not SpecialType.System_ArgIterator
                                                                                              and not SpecialType.System_AsyncCallback
                                                                                              and not SpecialType.System_Collections_Generic_ICollection_T
                                                                                              and not SpecialType.System_Collections_Generic_IEnumerable_T
                                                                                              and not SpecialType.System_Collections_Generic_IEnumerator_T
                                                                                              and not SpecialType.System_Collections_Generic_IList_T
                                                                                              and not SpecialType.System_Collections_Generic_IReadOnlyCollection_T
                                                                                              and not SpecialType.System_Collections_Generic_IReadOnlyList_T
                                                                                              and not SpecialType.System_Collections_IEnumerable
                                                                                              and not SpecialType.System_Collections_IEnumerator
                                                                                              and not SpecialType.System_Delegate
                                                                                              and not SpecialType.System_IAsyncResult
                                                                                              and not SpecialType.System_IDisposable
                                                                                              and not SpecialType.System_IntPtr
                                                                                              and not SpecialType.System_MulticastDelegate
                                                                                              and not SpecialType.System_Object
                                                                                              and not SpecialType.System_Runtime_CompilerServices_IsVolatile
                                                                                              and not SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute
                                                                                              and not SpecialType.System_Runtime_CompilerServices_RuntimeFeature
                                                                                              and not SpecialType.System_RuntimeArgumentHandle
                                                                                              and not SpecialType.System_RuntimeFieldHandle
                                                                                              and not SpecialType.System_RuntimeMethodHandle
                                                                                              and not SpecialType.System_RuntimeTypeHandle
                                                                                              and not SpecialType.System_TypedReference
                                                                                              and not SpecialType.System_UIntPtr
                                                                                              and not SpecialType.System_ValueType
                                                                                              and not SpecialType.System_Void)
                                                       .Where(PropertyOrFieldNotIgnored)
                                                       .Where(RecordParameterNotIgnored);

        IEnumerable<IPropertySymbol> floatingProperties = type.GetMembers()
                                                              .OfType<IPropertySymbol>()
                                                              .Where(property => !property.IsStatic)
                                                              .Where(property => property.DeclaredAccessibility is Accessibility.Public)
                                                              .Where(property => property.Parameters.Length is 0)
                                                              .Where(property => property.Type.SpecialType is not SpecialType.System_ArgIterator
                                                                                                           and not SpecialType.System_AsyncCallback
                                                                                                           and not SpecialType.System_Collections_Generic_ICollection_T
                                                                                                           and not SpecialType.System_Collections_Generic_IEnumerable_T
                                                                                                           and not SpecialType.System_Collections_Generic_IEnumerator_T
                                                                                                           and not SpecialType.System_Collections_Generic_IList_T
                                                                                                           and not SpecialType.System_Collections_Generic_IReadOnlyCollection_T
                                                                                                           and not SpecialType.System_Collections_Generic_IReadOnlyList_T
                                                                                                           and not SpecialType.System_Collections_IEnumerable
                                                                                                           and not SpecialType.System_Collections_IEnumerator
                                                                                                           and not SpecialType.System_Delegate
                                                                                                           and not SpecialType.System_IAsyncResult
                                                                                                           and not SpecialType.System_IDisposable
                                                                                                           and not SpecialType.System_IntPtr
                                                                                                           and not SpecialType.System_MulticastDelegate
                                                                                                           and not SpecialType.System_Object
                                                                                                           and not SpecialType.System_Runtime_CompilerServices_IsVolatile
                                                                                                           and not SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute
                                                                                                           and not SpecialType.System_Runtime_CompilerServices_RuntimeFeature
                                                                                                           and not SpecialType.System_RuntimeArgumentHandle
                                                                                                           and not SpecialType.System_RuntimeFieldHandle
                                                                                                           and not SpecialType.System_RuntimeMethodHandle
                                                                                                           and not SpecialType.System_RuntimeTypeHandle
                                                                                                           and not SpecialType.System_TypedReference
                                                                                                           and not SpecialType.System_UIntPtr
                                                                                                           and not SpecialType.System_ValueType
                                                                                                           and not SpecialType.System_Void)
                                                               .Where(PropertyNotIgnored);

        INamedTypeSymbol baseType = type.BaseType;
        while (baseType is not null)
        {
            IEnumerable<IFieldSymbol> baseFields = baseType.GetMembers()
                                                           .OfType<IFieldSymbol>()
                                                           .Where(field => !field.IsStatic)
                                                           .Where(field => !field.IsReadOnly)
                                                           .Where(field => field.DeclaredAccessibility is Accessibility.Public)
                                                           .Where(field => field.Type.SpecialType is not SpecialType.System_ArgIterator
                                                                                                  and not SpecialType.System_AsyncCallback
                                                                                                  and not SpecialType.System_Collections_Generic_ICollection_T
                                                                                                  and not SpecialType.System_Collections_Generic_IEnumerable_T
                                                                                                  and not SpecialType.System_Collections_Generic_IEnumerator_T
                                                                                                  and not SpecialType.System_Collections_Generic_IList_T
                                                                                                  and not SpecialType.System_Collections_Generic_IReadOnlyCollection_T
                                                                                                  and not SpecialType.System_Collections_Generic_IReadOnlyList_T
                                                                                                  and not SpecialType.System_Collections_IEnumerable
                                                                                                  and not SpecialType.System_Collections_IEnumerator
                                                                                                  and not SpecialType.System_Delegate
                                                                                                  and not SpecialType.System_IAsyncResult
                                                                                                  and not SpecialType.System_IDisposable
                                                                                                  and not SpecialType.System_IntPtr
                                                                                                  and not SpecialType.System_MulticastDelegate
                                                                                                  and not SpecialType.System_Object
                                                                                                  and not SpecialType.System_Runtime_CompilerServices_IsVolatile
                                                                                                  and not SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute
                                                                                                  and not SpecialType.System_Runtime_CompilerServices_RuntimeFeature
                                                                                                  and not SpecialType.System_RuntimeArgumentHandle
                                                                                                  and not SpecialType.System_RuntimeFieldHandle
                                                                                                  and not SpecialType.System_RuntimeMethodHandle
                                                                                                  and not SpecialType.System_RuntimeTypeHandle
                                                                                                  and not SpecialType.System_TypedReference
                                                                                                  and not SpecialType.System_UIntPtr
                                                                                                  and not SpecialType.System_ValueType
                                                                                                  and not SpecialType.System_Void)
                                                           .Where(PropertyOrFieldNotIgnored)
                                                           .Where(RecordParameterNotIgnored);
            if (baseFields.Any())
            {
                floatingFields = floatingFields.Concat(baseFields);
            }

            IEnumerable<IPropertySymbol> baseProperties = baseType.GetMembers()
                                                                  .OfType<IPropertySymbol>()
                                                                  .Where(property => !property.IsStatic)
                                                                  .Where(property => property.DeclaredAccessibility is Accessibility.Public)
                                                                  .Where(property => property.Parameters.Length is 0)
                                                                  .Where(property => property.Type.SpecialType is not SpecialType.System_ArgIterator
                                                                                                               and not SpecialType.System_AsyncCallback
                                                                                                               and not SpecialType.System_Collections_Generic_ICollection_T
                                                                                                               and not SpecialType.System_Collections_Generic_IEnumerable_T
                                                                                                               and not SpecialType.System_Collections_Generic_IEnumerator_T
                                                                                                               and not SpecialType.System_Collections_Generic_IList_T
                                                                                                               and not SpecialType.System_Collections_Generic_IReadOnlyCollection_T
                                                                                                               and not SpecialType.System_Collections_Generic_IReadOnlyList_T
                                                                                                               and not SpecialType.System_Collections_IEnumerable
                                                                                                               and not SpecialType.System_Collections_IEnumerator
                                                                                                               and not SpecialType.System_Delegate
                                                                                                               and not SpecialType.System_IAsyncResult
                                                                                                               and not SpecialType.System_IDisposable
                                                                                                               and not SpecialType.System_IntPtr
                                                                                                               and not SpecialType.System_MulticastDelegate
                                                                                                               and not SpecialType.System_Object
                                                                                                               and not SpecialType.System_Runtime_CompilerServices_IsVolatile
                                                                                                               and not SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute
                                                                                                               and not SpecialType.System_Runtime_CompilerServices_RuntimeFeature
                                                                                                               and not SpecialType.System_RuntimeArgumentHandle
                                                                                                               and not SpecialType.System_RuntimeFieldHandle
                                                                                                               and not SpecialType.System_RuntimeMethodHandle
                                                                                                               and not SpecialType.System_RuntimeTypeHandle
                                                                                                               and not SpecialType.System_TypedReference
                                                                                                               and not SpecialType.System_UIntPtr
                                                                                                               and not SpecialType.System_ValueType
                                                                                                               and not SpecialType.System_Void)
                                                                  .Where(PropertyNotIgnored);
            if (baseProperties.Any())
            {
                floatingProperties = floatingProperties.Concat(baseProperties);
            }

            baseType = baseType.BaseType;
        }

        result = floatingFields.Where(field => field.AssociatedSymbol is null)
                               .Cast<ISymbol>()
                               .Concat(floatingProperties.Where(property => property.GetMethod is not null &&
                                                                            property.SetMethod is not null))
                               .ToImmutableArray();
        s_MemberCache.GetOrAdd(key: type,
                               value: result);
        return result;
    }

    static public void ClearCaches()
    {
        s_CanBeSerializedCache.Clear();
        s_FileStringCache.Clear();
        s_HasDefaultConstructorCache.Clear();
        s_IsCollectionCache.Clear();
        s_IsSerializationHandlerCache.Clear();
        s_IsUnmanagedCache.Clear();
        s_MemberCache.Clear();
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

    static private Boolean PropertyNotIgnored(IPropertySymbol property)
    {
        if (property.IsStatic)
        {
            return true;
        }
        else
        {
            return !property.GetAttributes()
                            .Any(data => data.AttributeClass is not null &&
                                         data.AttributeClass.ToFrameworkString() is "Narumikazuchi.Generators.ByteSerialization.IngoreForSerializationAttribute");
        }
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

    static private readonly ConcurrentDictionary<ITypeSymbol, String> s_FileStringCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<ITypeSymbol, Boolean> s_CanBeSerializedCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<ITypeSymbol, Boolean> s_IsUnmanagedCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, Boolean> s_HasDefaultConstructorCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, Boolean> s_IsSerializationHandlerCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, ITypeSymbol> s_IsCollectionCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, ImmutableArray<ISymbol>> s_MemberCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<String, String> s_BuiltInTypes;
}