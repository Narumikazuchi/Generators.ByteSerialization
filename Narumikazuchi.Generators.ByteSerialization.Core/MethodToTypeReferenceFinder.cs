using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

static public class MethodToTypeReferenceFinder
{
    static public ITypeSymbol FilterType(Compilation compilation,
                                         InvocationExpressionSyntax invocation)
    {
        if (s_ByteSerializer is null)
        {
            IAssemblySymbol generatorAssembly = compilation.References.Select(compilation.GetAssemblyOrModuleSymbol)
                                                                      .OfType<IAssemblySymbol>()
                                                                      .First(a => a.Name is GlobalNames.NAMESPACE);

            s_ByteSerializer = generatorAssembly.GetTypeByMetadataName(GlobalNames.BYTESERIALIZER);
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
            return default;
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

        return type;
    }

    static private INamedTypeSymbol s_ByteSerializer = default;
    static private ImmutableArray<IMethodSymbol> s_MethodSymbols = ImmutableArray<IMethodSymbol>.Empty;
}