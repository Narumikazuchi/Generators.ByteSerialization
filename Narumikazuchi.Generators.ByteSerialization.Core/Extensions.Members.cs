using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

public partial class Extensions
{
    static internal ImmutableArray<ISymbol> GetMembersToSerialize(this INamedTypeSymbol type)
    {
        if (s_MemberCache.TryGetValue(key: type,
                                      value: out ImmutableArray<ISymbol> result))
        {
            return result;
        }

        AttributeData data = type.GetAttributes()
                                 .FirstOrDefault(data => data.AttributeClass is not null &&
                                                         data.AttributeClass.ToFrameworkString() is GlobalNames.DATALAYOUTATTRIBUTE);
        Byte layout = 0;
        if (data is not null)
        {
            layout = (Byte)data.ConstructorArguments[0].Value;
        }

        ImmutableArray<ISymbol> typeMembers = type.GetMembers();
        IEnumerable<IFieldSymbol> floatingFields = FilterUseableFields(type: type,
                                                                       members: typeMembers);
        IEnumerable<IPropertySymbol> floatingProperties = FilterUseableProperties(typeMembers);

        INamedTypeSymbol baseType = type.BaseType;
        while (baseType is not null)
        {
            typeMembers = baseType.GetMembers();
            IEnumerable<IFieldSymbol> baseFields = FilterUseableFields(type: baseType,
                                                                       members: typeMembers);
            if (baseFields.Any())
            {
                floatingFields = floatingFields.Concat(baseFields);
            }

            IEnumerable<IPropertySymbol> baseProperties = FilterUseableProperties(typeMembers);
            if (baseProperties.Any())
            {
                floatingProperties = floatingProperties.Concat(baseProperties);
            }

            baseType = baseType.BaseType;
        }

        floatingFields = floatingFields.Where(field => field.AssociatedSymbol is null);

        if (layout is 0)
        {
            result = floatingProperties.Where(property => property.GetMethod is not null &&
                                                          property.SetMethod is not null)
                                       .Cast<ISymbol>()
                                       .Concat(floatingFields)
                                       .ToImmutableArray();
        }
        else if (layout is 1)
        {
            result = floatingProperties.Where(property => property.GetMethod is not null &&
                                                          property.SetMethod is not null)
                                       .Cast<ISymbol>()
                                       .Concat(floatingFields)
                                       .OrderBy(ExplicitSort)
                                       .ToImmutableArray();
        }
        else
        {
            result = ImmutableArray<ISymbol>.Empty;
        }

        s_MemberCache.GetOrAdd(key: type,
                               value: result);
        return result;
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
                                      data.AttributeClass.ToFrameworkString() is GlobalNames.IGNOREFORSERIALIZATIONATTRIBUTE);
    }

    static private Boolean PropertyNotIgnored(IPropertySymbol property)
    {
        return !property.GetAttributes()
                        .Any(data => data.AttributeClass is not null &&
                                     data.AttributeClass.ToFrameworkString() is GlobalNames.IGNOREFORSERIALIZATIONATTRIBUTE);
    }

    static private Boolean PropertyOrFieldNotIgnored(IFieldSymbol field)
    {
        if (field.AssociatedSymbol is not null)
        {
            return !field.AssociatedSymbol.GetAttributes()
                                          .Any(data => data.AttributeClass is not null &&
                                                       data.AttributeClass.ToFrameworkString() is GlobalNames.IGNOREFORSERIALIZATIONATTRIBUTE);
        }
        else
        {
            return !field.GetAttributes()
                         .Any(data => data.AttributeClass is not null &&
                                      data.AttributeClass.ToFrameworkString() is GlobalNames.IGNOREFORSERIALIZATIONATTRIBUTE);
        }
    }

    static private IEnumerable<IFieldSymbol> FilterUseableFields(INamedTypeSymbol type,
                                                                 ImmutableArray<ISymbol> members)
    {
        Boolean RecordParameterNotIgnored(IFieldSymbol field)
        {
            return ParameterNotIgnored(symbol: type,
                                       field: field);
        }

        return members.OfType<IFieldSymbol>()
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

    }

    static private IEnumerable<IPropertySymbol> FilterUseableProperties(ImmutableArray<ISymbol> members)
    {
        return members.OfType<IPropertySymbol>()
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
    }

    static private Int32 ExplicitSort(ISymbol member)
    {
        if (member is IFieldSymbol field)
        {
            AttributeData layoutPosition = field.GetAttributes()
                                                .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                        data.AttributeClass.ToFrameworkString() is GlobalNames.DATALAYOUTPOSITIONATTRIBUTE);
            if (layoutPosition is not null)
            {
                return (Byte)layoutPosition.ConstructorArguments[0].Value;
            }
            else
            {
                return Int32.MaxValue;
            }
        }
        else if (member is IPropertySymbol property)
        {
            AttributeData layoutPosition = property.GetAttributes()
                                                   .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                           data.AttributeClass.ToFrameworkString() is GlobalNames.DATALAYOUTPOSITIONATTRIBUTE);
            if (layoutPosition is not null)
            {
                return (Byte)layoutPosition.ConstructorArguments[0].Value;
            }
            else if (property.ContainingType.IsRecord)
            {
                IParameterSymbol parameter = property.ContainingType.InstanceConstructors[0].Parameters.FirstOrDefault(parameter => parameter.Name == property.Name);

                if (parameter is null)
                {
                    return Int32.MaxValue;
                }

                layoutPosition = parameter.GetAttributes()
                                          .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                  data.AttributeClass.ToFrameworkString() is GlobalNames.DATALAYOUTPOSITIONATTRIBUTE);
                if (layoutPosition is not null)
                {
                    return (Byte)layoutPosition.ConstructorArguments[0].Value;
                }
                else
                {
                    return Int32.MaxValue;
                }
            }
            else
            {
                return Int32.MaxValue;
            }
        }
        else
        {
            return Int32.MaxValue;
        }
    }

    static private readonly ConcurrentDictionary<INamedTypeSymbol, ImmutableArray<ISymbol>> s_MemberCache = new(SymbolEqualityComparer.Default);
}