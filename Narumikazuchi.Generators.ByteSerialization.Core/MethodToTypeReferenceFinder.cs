using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

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
        if (symbolInfo.Symbol is not IMethodSymbol method ||
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
        if (type.IsUnmanagedSerializable())
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

    static private INamedTypeSymbol s_ByteSerializer = default;
    static private ImmutableArray<INamedTypeSymbol> s_IgnoredTypeSymbols = ImmutableArray<INamedTypeSymbol>.Empty;
    static private ImmutableArray<IMethodSymbol> s_MethodSymbols = ImmutableArray<IMethodSymbol>.Empty;
}