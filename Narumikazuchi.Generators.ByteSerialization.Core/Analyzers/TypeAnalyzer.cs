using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class TypeAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(action: this.Analyze,
                                         SyntaxKind.ClassDeclaration,
                                         SyntaxKind.RecordDeclaration,
                                         SyntaxKind.StructDeclaration,
                                         SyntaxKind.RecordStructDeclaration);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new DiagnosticDescriptor[]
    {
        s_NoDefaultConstructorDescriptor,
        s_MoreThanOneSerializerDefinedDescriptor
    }.ToImmutableArray();

    private void Analyze(SyntaxNodeAnalysisContext context)
    {
        TypeDeclarationSyntax syntax = (TypeDeclarationSyntax)context.Node;
        INamedTypeSymbol type = context.SemanticModel.GetDeclaredSymbol(syntax);
        if (type is null ||
            type.IsCompilerGenerated())
        {
            return;
        }

        INamedTypeSymbol handler = context.Compilation.GetTypeByMetadataName($"{GlobalNames.NAMESPACE}.{GlobalNames.ISERIALIZATIONHANDLER}");
        ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> customSerializers = CustomHandlerFinder.FindTypesWithCustomHandlerIn(context.Compilation);

        foreach (INamedTypeSymbol implements in type.AllInterfaces.Where(implements => implements.MetadataName is GlobalNames.ISERIALIZATIONHANDLER))
        {
            if (!SymbolEqualityComparer.Default.Equals(handler, implements.ConstructedFrom))
            {
                continue;
            }

            ITypeSymbol handledType = implements.TypeArguments[0];
            if (customSerializers.TryGetValue(key: handledType,
                                              value: out ImmutableHashSet<INamedTypeSymbol> serializers))
            {
                if (serializers.Count is 1)
                {
                    if (!serializers.First()
                                    .HasDefaultConstructor())
                    {
                        context.ReportDiagnostic(CreateNoDefaultConstructorDiagnostic(syntax));
                    }
                }
                else
                {
                    context.ReportDiagnostic(CreateMoreThanOneSerializerDefinedDiagnostic(type: syntax,
                                                                                          typename: handledType.Name));
                }
            }
        }
    }
}