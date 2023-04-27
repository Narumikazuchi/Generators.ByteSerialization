using Microsoft.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ITypeSymbol> declarations = context.SyntaxProvider.CreateSyntaxProvider(predicate: IsEligableTypeSyntax,
                                                                                                          transform: TransformToType)
                                                                                    .SelectMany((types, _) => types);

        IncrementalValueProvider<(Compilation, ImmutableArray<ITypeSymbol>)> compilationAndTypes = context.CompilationProvider.Combine(declarations.Collect());
        context.RegisterSourceOutput(source: compilationAndTypes,
                                     action: GenerateSerializationCode);
    }
}