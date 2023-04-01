﻿namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class TypeAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(action: this.Analyze,
                                         SyntaxKind.ClassDeclaration,
                                         SyntaxKind.StructDeclaration,
                                         SyntaxKind.RecordDeclaration,
                                         SyntaxKind.RecordStructDeclaration);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new DiagnosticDescriptor[]
    {
        s_NoSerializationForTypeDescriptor,
        s_DuplicateStrategyForTypeDescriptor
    }.ToImmutableArray();

    private void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax type ||
            context.SemanticModel.Compilation.Options.SyntaxTreeOptionsProvider!.IsGenerated(type.SyntaxTree, CancellationToken.None) is GeneratedKind.MarkedGenerated)
        {
            return;
        }

        SemanticModel semanticModel = context.SemanticModel;
        INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(type);
        if (typeSymbol is null)
        {
            return;
        }

        if (!typeSymbol.IsByteSerializable())
        {
            return;
        }

        this.AnalyzeMembers(context: context,
                            type: type,
                            typeSymbol: typeSymbol);
    }

    private void AnalyzeMembers(SyntaxNodeAnalysisContext context,
                                TypeDeclarationSyntax type,
                                INamedTypeSymbol typeSymbol)
    {
        Dictionary<ITypeSymbol, ITypeSymbol> strategies = __Shared.GetStrategiesFromAttributes(symbol: typeSymbol,
                                                                                               compilation: context.Compilation,
                                                                                               type: type,
                                                                                               duplicates: out Dictionary<ITypeSymbol, List<AttributeData>> duplicates);
        if (duplicates.Count > 0)
        {
            foreach (KeyValuePair<ITypeSymbol, List<AttributeData>> kv in duplicates)
            {
                foreach (AttributeData attribute in kv.Value)
                {
                    SyntaxReference reference = attribute.ApplicationSyntaxReference!;
                    AttributeSyntax syntax = (AttributeSyntax)reference.GetSyntax();
                    context.ReportDiagnostic(CreateDuplicateStrategyForTypeDiagnostic(attribute: syntax,
                                                                                      typename: kv.Key.ToFrameworkString()));
                }
            }
        }

        ImmutableArray<IFieldSymbol> fields = __Shared.GetFieldsToSerialize(typeSymbol);

        foreach (IFieldSymbol field in fields)
        {
            if (field.Type.CanBeSerialized(strategies))
            {
                continue;
            }
            else
            {
                ISymbol target = field;
                if (field.AssociatedSymbol is not null)
                {
                    target = field.AssociatedSymbol;
                }

                context.ReportDiagnostic(CreateNoSerializationForTypeDiagnostic(type: type,
                                                                                membername: target.Name,
                                                                                typename: field.Type.ToFrameworkString()));
            }
        }
    }
}