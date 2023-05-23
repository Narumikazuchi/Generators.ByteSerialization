﻿using Microsoft.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(source: context.AdditionalTextsProvider.Where(text => text.Path.EndsWith("Narumikazuchi.config"))
                                                                            .Collect(),
                                     action: this.ConfigureLogging);

        IncrementalValuesProvider<ITypeSymbol> declarations = context.SyntaxProvider.CreateSyntaxProvider(predicate: IsEligableTypeSyntax,
                                                                                                          transform: TransformToType)
                                                                                    .Where(static type => type is not null);

        IncrementalValueProvider<(Compilation, ImmutableArray<ITypeSymbol>)> compilationAndTypes = context.CompilationProvider.Combine(declarations.Collect());
        context.RegisterSourceOutput(source: compilationAndTypes,
                                     action: this.GenerateSerializationCode);
    }
}