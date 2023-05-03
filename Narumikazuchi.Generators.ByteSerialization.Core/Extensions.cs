using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

static public partial class Extensions
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
                    result = namedType.ToFrameworkString();
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

    static public Boolean IsCompilerGenerated(this INamedTypeSymbol type)
    {
        if (s_IsCompilerGeneratedCache.TryGetValue(key: type,
                                                   value: out Boolean result))
        {
            return result;
        }
        else
        {
            result = type.GetAttributes()
                         .Any(data => data.AttributeClass is not null &&
                                      data.AttributeClass.ToFrameworkString() is "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
            s_IsCompilerGeneratedCache.GetOrAdd(key: type,
                                                value: result);
            return result;
        }
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

    static public IMethodSymbol ParameterizedConstructor(this INamedTypeSymbol type)
    {
        if (s_ParameterizedConstructor.TryGetValue(key: type,
                                                   value: out IMethodSymbol result))
        {
            return result;
        }
        else
        {
            foreach (IMethodSymbol constructor in type.InstanceConstructors.OrderByDescending(method => method.Parameters.Length))
            {
                if (constructor.DeclaredAccessibility is not Accessibility.Public ||
                    constructor.Parameters.Length is 0)
                {
                    continue;
                }

                if (constructor.Parameters.All(ParameterHasSerializeAttribute))
                {
                    s_ParameterizedConstructor.GetOrAdd(key: type,
                                                        value: constructor);
                    return constructor;
                }
            }

            s_ParameterizedConstructor.GetOrAdd(key: type,
                                                value: default);
            return default;
        }
    }

    static public Boolean CanBeSerialized(this ITypeSymbol type,
                                          ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> customSerializers)
    {
        if (s_CanBeSerializedCache.TryGetValue(key: type,
                                               value: out Boolean result))
        {
            return result;
        }
        else if (type.SpecialType is SpecialType.System_IntPtr
                                  or SpecialType.System_UIntPtr
                                  or SpecialType.System_Delegate
                                  or SpecialType.System_MulticastDelegate)
        {
            s_CanBeSerializedCache.GetOrAdd(key: type,
                                            value: false);
            return false;
        }
        else if (type.IsUnmanagedSerializable())
        {
            s_CanBeSerializedCache.GetOrAdd(key: type,
                                            value: true);
            return true;
        }
        else if (customSerializers.TryGetValue(key: type,
                                               value: out ImmutableHashSet<INamedTypeSymbol> serializers))
        {
            s_CanBeSerializedCache.GetOrAdd(key: type,
                                            value: serializers.Count is 1);
            return serializers.Count is 1;
        }
        else if (type is INamedTypeSymbol named)
        {
            if (named.IsOpenGenericType())
            {
                s_CanBeSerializedCache.GetOrAdd(key: type,
                                                value: false);
                return false;
            }
            else if (named.IsAbstract &&
                     named.GetDerivedTypes().Length is 0)
            {
                s_CanBeSerializedCache.GetOrAdd(key: type,
                                                value: false);
                return false;
            }
            else if (named.SpecialType is SpecialType.System_String)
            {
                s_CanBeSerializedCache.GetOrAdd(key: type,
                                                value: true);
                return true;
            }
            else if (named.HasDefaultConstructor())
            {
                result = named.GetMembersToSerialize()
                              .All(member => member.CanBeSerialized(customSerializers));
                s_CanBeSerializedCache.GetOrAdd(key: type,
                                                value: result);
                return result;
            }
            else if (named.IsRecord)
            {
                result = named.GetMembersToSerialize()
                              .All(member => member.CanBeSerialized(customSerializers));
                s_CanBeSerializedCache.GetOrAdd(key: type,
                                                value: result);
                return result;
            }
            else if (named.ParameterizedConstructor() is not null)
            {
                result = named.GetMembersToSerialize()
                              .All(member => member.CanBeSerialized(customSerializers));
                s_CanBeSerializedCache.GetOrAdd(key: type,
                                                value: result);
                return result;
            }
            else
            {
                s_CanBeSerializedCache.GetOrAdd(key: type,
                                                value: false);
                return false;
            }
        }
        else if (type is IArrayTypeSymbol array)
        {
            result = array.ElementType.CanBeSerialized(customSerializers);
            s_CanBeSerializedCache.GetOrAdd(key: type,
                                            value: result);
            return result;
        }
        else
        {
            s_CanBeSerializedCache.GetOrAdd(key: type,
                                            value: false);
            return false;
        }
    }

    static public Boolean CanBeSerialized(this ISymbol member,
                                          ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> customSerializers)
    {
        if (member is IFieldSymbol field)
        {
            return field.Type.CanBeSerialized(customSerializers);
        }
        else if (member is IPropertySymbol property)
        {
            return property.Type.CanBeSerialized(customSerializers);
        }
        else if (member is ITypeSymbol type)
        {
            return type.CanBeSerialized(customSerializers);
        }
        else
        {
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
    
    static public void ClearCaches()
    {
        s_CanBeSerializedCache.Clear();
        s_FileStringCache.Clear();
        s_HasDefaultConstructorCache.Clear();
        s_IsCollectionCache.Clear();
        s_IsCompilerGeneratedCache.Clear();
        s_IsSerializationHandlerCache.Clear();
        s_IsUnmanagedCache.Clear();
        s_MemberCache.Clear();
    }

    static private Boolean ParameterHasSerializeAttribute(IParameterSymbol parameter)
    {
        ImmutableArray<AttributeData> attributes = parameter.GetAttributes();
        Boolean result = false;

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass is null)
            {
                continue;
            }

            if (attribute.AttributeClass.ToFrameworkString()
                                        .StartsWith(GlobalNames.SERIALIZEDEFAULTATTRIBUTE) ||
                (attribute.AttributeClass.ToFrameworkString()
                                         .StartsWith(GlobalNames.SERIALIZEFROMMEMBERATTRIBUTE) &&
                !attribute.ConstructorArguments[0].IsNull))
            {
                result = !result;
            }
        }

        return result;
    }

    static private readonly ConcurrentDictionary<ITypeSymbol, String> s_FileStringCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<ITypeSymbol, Boolean> s_CanBeSerializedCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<ITypeSymbol, Boolean> s_IsUnmanagedCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, Boolean> s_IsCompilerGeneratedCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, Boolean> s_HasDefaultConstructorCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, Boolean> s_IsSerializationHandlerCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, ITypeSymbol> s_IsCollectionCache = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<INamedTypeSymbol, IMethodSymbol> s_ParameterizedConstructor = new(SymbolEqualityComparer.Default);
    static private readonly ConcurrentDictionary<String, String> s_BuiltInTypes;
}