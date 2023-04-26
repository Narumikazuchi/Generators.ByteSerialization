using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

public partial class Extensions
{
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

    static private readonly ConcurrentDictionary<INamedTypeSymbol, ImmutableArray<ISymbol>> s_MemberCache = new(SymbolEqualityComparer.Default);
}