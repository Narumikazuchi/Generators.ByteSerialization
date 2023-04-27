using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

static public class MethodToTypeReferenceFinder
{
    static public ImmutableArray<ITypeSymbol> FindTypes(Compilation compilation,
                                                        InvocationExpressionSyntax invocation)
    {
        if (s_ByteSerializer is null)
        {
            IAssemblySymbol generatorAssembly = compilation.References.Select(compilation.GetAssemblyOrModuleSymbol)
                                                                      .OfType<IAssemblySymbol>()
                                                                      .First(a => a.Name is GlobalNames.NAMESPACE);

            s_ByteSerializer = generatorAssembly.GetTypeByMetadataName(GlobalNames.BYTESERIALIZER);
        }

        if (s_IgnoredTypeSymbols.Length is 0)
        {
            ImmutableArray<INamedTypeSymbol>.Builder namedBuilder = ImmutableArray.CreateBuilder<INamedTypeSymbol>(22);
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Boolean));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Byte));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Char));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_DateTime));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Decimal));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Double));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Enum));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Int16));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Int32));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Int64));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Nullable_T));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_SByte));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_Single));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_String));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_UInt16));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_UInt32));
            namedBuilder.Add(compilation.GetSpecialType(SpecialType.System_UInt64));
            s_IgnoredTypeSymbols = namedBuilder.ToImmutable();
        }

        if (s_MethodSymbols.Length is 0)
        {
            s_MethodSymbols = s_ByteSerializer.GetMembers()
                                              .OfType<IMethodSymbol>()
                                              .Where(method => method.Name is "Deserialize"
                                                                           or "DeserializeAsynchronously"
                                                                           or "GetExpectedSerializedSize"
                                                                           or "Serialize"
                                                                           or "SerializeAsynchronously")
                                              .ToImmutableArray();
        }

        SymbolInfo symbolInfo = compilation.GetSemanticModel(invocation.SyntaxTree)
                                           .GetSymbolInfo(invocation);
        IMethodSymbol method = (IMethodSymbol)symbolInfo.Symbol;
        if (method is null ||
            !method.IsGenericMethod ||
            !SymbolEqualityComparer.Default.Equals(s_ByteSerializer, method.ContainingType))
        {
            return ImmutableArray<ITypeSymbol>.Empty;
        }

        ITypeSymbol type = default;
        IMethodSymbol genericMethod = method.ConstructedFrom;
        foreach (IMethodSymbol serializerMethod in s_MethodSymbols)
        {
            if (SymbolEqualityComparer.Default.Equals(genericMethod, serializerMethod))
            {
                type = method.TypeArguments.Last();
                break;
            }
        }

        if (type is null)
        {
            return ImmutableArray<ITypeSymbol>.Empty;
        }
        else
        {
            ImmutableArray<ITypeSymbol>.Builder builder = ImmutableArray.CreateBuilder<ITypeSymbol>();
            if (!RequiresGeneration(type))
            {
                return ImmutableArray<ITypeSymbol>.Empty;
            }

            builder.Add(type);
            return builder.ToImmutable();
        }
    }

    static private Boolean RequiresGeneration(ITypeSymbol type)
    {
        if (type.CanBeSerialized())
        {
            return false;
        }

        foreach (ITypeSymbol ignored in s_IgnoredTypeSymbols)
        {
            if (SymbolEqualityComparer.Default.Equals(type, ignored))
            {
                return false;
            }
        }

        return true;
    }

    static private ITypeSymbol[] GetDependendTypes(ITypeSymbol type)
    {
        HashSet<ITypeSymbol> builder = new(SymbolEqualityComparer.Default);
        if (type is IArrayTypeSymbol array)
        {
            builder.Add(array.ElementType);

            foreach (ITypeSymbol dependent in GetDependendTypes(array.ElementType))
            {
                builder.Add(dependent);
            }
        }
        else if (type is INamedTypeSymbol namedType)
        {
            if (namedType.IsGenericType)
            {
                foreach (ITypeSymbol typeArgument in namedType.TypeArguments.Where(argument => argument.TypeKind is not TypeKind.TypeParameter))
                {
                    builder.Add(typeArgument);
                    foreach (ITypeSymbol dependent in GetDependendTypes(typeArgument))
                    {
                        builder.Add(dependent);
                    }
                }
            }

            ImmutableArray<ISymbol> members = namedType.GetMembersToSerialize();
            foreach (IFieldSymbol field in members.OfType<IFieldSymbol>())
            {
                if (field.Type is INamedTypeSymbol fieldType &&
                    fieldType.SpecialType is SpecialType.None
                                          or SpecialType.System_DateTime
                                          or SpecialType.System_Enum
                                          or SpecialType.System_Nullable_T
                                          or SpecialType.System_String)
                {
                    builder.Add(fieldType);
                    foreach (ITypeSymbol dependent in GetDependendTypes(fieldType))
                    {
                        builder.Add(dependent);
                    }
                }
                else if (field.Type is IArrayTypeSymbol arrayField &&
                         arrayField.ElementType is INamedTypeSymbol element)
                {
                    builder.Add(arrayField);
                    builder.Add(element);
                }
            }

            foreach (IPropertySymbol property in members.OfType<IPropertySymbol>())
            {
                if (property.Type is INamedTypeSymbol propertyType &&
                    propertyType.SpecialType is SpecialType.None
                                          or SpecialType.System_DateTime
                                          or SpecialType.System_Enum
                                          or SpecialType.System_Nullable_T
                                          or SpecialType.System_String)
                {
                    builder.Add(propertyType);
                    foreach (ITypeSymbol dependent in GetDependendTypes(propertyType))
                    {
                        builder.Add(dependent);
                    }
                }
                else if (property.Type is IArrayTypeSymbol arrayField &&
                         arrayField.ElementType is INamedTypeSymbol element)
                {
                    builder.Add(arrayField);
                    builder.Add(element);
                }
            }

            if (!namedType.IsValueType &&
                !namedType.IsSealed)
            {
                foreach (INamedTypeSymbol derived in namedType.GetDerivedTypes())
                {
                    builder.Add(derived);
                    foreach (ITypeSymbol dependent in GetDependendTypes(derived))
                    {
                        builder.Add(dependent);
                    }
                }
            }

            INamedTypeSymbol collection = type.AllInterfaces.FirstOrDefault(@interface => @interface.ToFrameworkString()
                                                                                                    .StartsWith("System.Collections.Generic.ICollection<"));
            if (collection is not null)
            {
                builder.Add(collection.TypeArguments[0]);
                foreach (ITypeSymbol dependent in GetDependendTypes(collection.TypeArguments[0]))
                {
                    builder.Add(dependent);
                }
            }
        }

        return builder.ToArray();
    }

    static private INamedTypeSymbol s_ByteSerializer = default;
    static private ImmutableArray<INamedTypeSymbol> s_IgnoredTypeSymbols = ImmutableArray<INamedTypeSymbol>.Empty;
    static private ImmutableArray<IMethodSymbol> s_MethodSymbols = ImmutableArray<IMethodSymbol>.Empty;
}