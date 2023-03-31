namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TypeDeclarationSyntax> declarations = context.SyntaxProvider.CreateSyntaxProvider(predicate: IsEligableTypeSyntax,
                                                                                                                    transform: TransformToType)
                                                                                              .Where(static type => type is not null);

        IncrementalValueProvider<(Compilation, ImmutableArray<TypeDeclarationSyntax>)> compilationAndTypes = context.CompilationProvider.Combine(declarations.Collect());
        context.RegisterSourceOutput(source: compilationAndTypes,
                                     action: GenerateSerializationCode);
    }
}