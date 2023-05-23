using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class DataLayoutAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        if (!Debugger.IsAttached)
        {
            context.EnableConcurrentExecution();
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(action: this.AnalyzeTypeDeclaration,
                                         SyntaxKind.ClassDeclaration,
                                         SyntaxKind.StructDeclaration,
                                         SyntaxKind.RecordDeclaration,
                                         SyntaxKind.RecordStructDeclaration);
        context.RegisterSyntaxNodeAction(action: this.AnalyzeAttribute,
                                         SyntaxKind.AttributeList);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new DiagnosticDescriptor[]
    {
        s_DataLayoutOutOfBoundsDescriptor,
        s_MemberNotExplicitlyMarkedDescriptor,
        s_LayoutPositionWithoutEffectNoRecordDescriptor,
        s_LayoutPositionWithoutEffectNoConstructorDescriptor,
        s_LayoutPositionWithoutEffectDescriptor
    }.ToImmutableArray();

    private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        INamedTypeSymbol dataLayoutAttribute = context.Compilation.GetTypeByMetadataName($"{GlobalNames.NAMESPACE}.DataLayoutAttribute");
        INamedTypeSymbol dataLayoutPositionAttribute = context.Compilation.GetTypeByMetadataName($"{GlobalNames.NAMESPACE}.DataLayoutPositionAttribute");

        if (context.Node is TypeDeclarationSyntax typeDeclaration)
        {
            foreach (AttributeListSyntax attributeList in typeDeclaration.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol constructor)
                    {
                        continue;
                    }

                    if (!SymbolEqualityComparer.Default.Equals(constructor.ContainingType, dataLayoutAttribute))
                    {
                        continue;
                    }

                    INamedTypeSymbol type = context.SemanticModel.GetDeclaredSymbol(typeDeclaration);
                    if (type is null ||
                        type.IsCompilerGenerated())
                    {
                        continue;
                    }

                    this.AnalyzeType(type: type,
                                     dataLayoutPositionAttribute: dataLayoutPositionAttribute,
                                     context: context,
                                     attribute: attribute);
                }
            }
        }
    }

    private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        INamedTypeSymbol dataLayoutPositionAttribute = context.Compilation.GetTypeByMetadataName($"{GlobalNames.NAMESPACE}.DataLayoutPositionAttribute");

        if (context.Node is AttributeListSyntax attributeList)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol constructor)
                {
                    continue;
                }

                if (!SymbolEqualityComparer.Default.Equals(dataLayoutPositionAttribute, constructor.ContainingType))
                {
                    continue;
                }

                this.AnalyzeAttribute(context: context,
                                      attribute: attribute);
            }
        }
    }

    private void AnalyzeType(INamedTypeSymbol type,
                             INamedTypeSymbol dataLayoutPositionAttribute,
                             SyntaxNodeAnalysisContext context,
                             AttributeSyntax attribute)
    {
        SyntaxReference syntaxReference = attribute.GetReference();
        AttributeData data = type.GetAttributes()
                                 .First(data => data.ApplicationSyntaxReference is not null &&
                                                data.ApplicationSyntaxReference.Span == syntaxReference.Span &&
                                                data.ApplicationSyntaxReference.SyntaxTree == syntaxReference.SyntaxTree);

        Byte layout = (Byte)data.ConstructorArguments[0].Value;
        if (layout > 2)
        {
            context.ReportDiagnostic(CreateDataLayoutOutOfBoundsDiagnostic(attribute.ArgumentList.Arguments[0]));
        }

        ImmutableArray<ISymbol> members = type.GetMembersToSerialize();

        foreach (ISymbol member in members)
        {
            if (member is IFieldSymbol field)
            {
                AttributeData layoutPosition = field.GetAttributes()
                                                    .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                            SymbolEqualityComparer.Default.Equals(data.AttributeClass, dataLayoutPositionAttribute));
                if (layout is 1 &&
                    layoutPosition is null)
                {
                    SyntaxReference reference = field.DeclaringSyntaxReferences[0];
                    MemberDeclarationSyntax syntax = reference.GetSyntax() as MemberDeclarationSyntax;
                    context.ReportDiagnostic(CreateMemberNotExplicitlyMarkedDiagnostic(syntax));
                }
            }
            else if (member is IPropertySymbol property)
            {
                AttributeData propertyLayoutPosition = property.GetAttributes()
                                                               .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                                       SymbolEqualityComparer.Default.Equals(data.AttributeClass, dataLayoutPositionAttribute));
                IParameterSymbol parameter = default;
                if (property.ContainingType.IsRecord)
                {
                    parameter = property.ContainingType.InstanceConstructors[0].Parameters.FirstOrDefault(parameter => parameter.Name == property.Name);
                }

                AttributeData parameterLayoutPosition = default;
                if (parameter is not null)
                {
                    parameterLayoutPosition = parameter.GetAttributes()
                                                       .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                               SymbolEqualityComparer.Default.Equals(data.AttributeClass, dataLayoutPositionAttribute));
                }

                if (layout is 1 &&
                    propertyLayoutPosition is null &&
                    parameterLayoutPosition is null)
                {
                    SyntaxReference reference = property.DeclaringSyntaxReferences[0];
                    SyntaxNode syntax = reference.GetSyntax();
                    if (syntax is MemberDeclarationSyntax typeMember)
                    {
                        context.ReportDiagnostic(CreateMemberNotExplicitlyMarkedDiagnostic(typeMember));
                    }
                    else if (syntax is ParameterSyntax recordParameter)
                    {
                        context.ReportDiagnostic(CreateMemberNotExplicitlyMarkedDiagnostic(recordParameter));
                    }
                }
            }
        }
    }
    private void AnalyzeAttribute(SyntaxNodeAnalysisContext context,
                                  AttributeSyntax attribute)
    {
        if (attribute.Parent is not AttributeListSyntax attributeList)
        {
            throw new InvalidOperationException();
        }

        if (attributeList.Parent is null)
        {
            throw new InvalidOperationException();
        }

        ISymbol decorated = context.SemanticModel.GetSymbolInfo(attributeList.Parent).Symbol;
        if (decorated is null)
        {
            decorated = context.SemanticModel.GetDeclaredSymbol(attributeList.Parent);
        }

        if (decorated is null)
        {
            throw new InvalidOperationException();
        }

        SyntaxReference syntaxReference = attribute.GetReference();
        if (decorated is IPropertySymbol property)
        {
            AttributeData data = property.GetAttributes()
                                         .FirstOrDefault(data => data.ApplicationSyntaxReference is not null &&
                                                                 data.ApplicationSyntaxReference.Span == syntaxReference.Span &&
                                                                 data.ApplicationSyntaxReference.SyntaxTree == syntaxReference.SyntaxTree);

            AttributeData dataLayout = property.ContainingType.GetAttributes()
                                                              .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                                      data.AttributeClass.ToFrameworkString() is GlobalNames.DATALAYOUTATTRIBUTE);
            if (dataLayout is null &&
                data is not null)
            {
                context.ReportDiagnostic(CreateLayoutPositionWithoutEffectDiagnostic(attribute));
            }
            else if (dataLayout is not null &&
                     data is not null)
            {
                Byte layout = (Byte)dataLayout.ConstructorArguments[0].Value;
                if (layout is 0)
                {
                    context.ReportDiagnostic(CreateLayoutPositionWithoutEffectDiagnostic(attribute));
                }
            }
        }
        else if (decorated is IParameterSymbol parameter)
        {
            if (parameter.ContainingSymbol is not IMethodSymbol constructor ||
                constructor.MethodKind is not MethodKind.Constructor)
            {
                context.ReportDiagnostic(CreateLayoutPositionWithoutEffectNoConstructorDiagnostic(attribute));
                return;
            }

            if (!parameter.ContainingType.IsRecord)
            {
                context.ReportDiagnostic(CreateLayoutPositionWithoutEffectNoRecordDiagnostic(attribute));
                return;
            }

            AttributeData data = parameter.GetAttributes()
                                          .FirstOrDefault(data => data.ApplicationSyntaxReference is not null &&
                                                                  data.ApplicationSyntaxReference.Span == syntaxReference.Span &&
                                                                  data.ApplicationSyntaxReference.SyntaxTree == syntaxReference.SyntaxTree);

            AttributeData dataLayout = parameter.ContainingType.GetAttributes()
                                                               .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                                       data.AttributeClass.ToFrameworkString() is GlobalNames.DATALAYOUTATTRIBUTE);
            if (dataLayout is null &&
                data is not null)
            {
                context.ReportDiagnostic(CreateLayoutPositionWithoutEffectDiagnostic(attribute));
            }
            else if (dataLayout is not null &&
                     data is not null)
            {
                Byte layout = (Byte)dataLayout.ConstructorArguments[0].Value;
                if (layout is 0)
                {
                    context.ReportDiagnostic(CreateLayoutPositionWithoutEffectDiagnostic(attribute));
                }
            }
        }
        else if (decorated is IFieldSymbol field)
        {
            AttributeData data = field.GetAttributes()
                                      .FirstOrDefault(data => data.ApplicationSyntaxReference is not null &&
                                                              data.ApplicationSyntaxReference.Span == syntaxReference.Span &&
                                                              data.ApplicationSyntaxReference.SyntaxTree == syntaxReference.SyntaxTree);

            AttributeData dataLayout = field.ContainingType.GetAttributes()
                                                           .FirstOrDefault(data => data.AttributeClass is not null &&
                                                                                   data.AttributeClass.ToFrameworkString() is GlobalNames.DATALAYOUTATTRIBUTE);
            if (dataLayout is null &&
                data is not null)
            {
                context.ReportDiagnostic(CreateLayoutPositionWithoutEffectDiagnostic(attribute));
            }
            else if (dataLayout is not null &&
                     data is not null)
            {
                Byte layout = (Byte)dataLayout.ConstructorArguments[0].Value;
                if (layout is 0)
                {
                    context.ReportDiagnostic(CreateLayoutPositionWithoutEffectDiagnostic(attribute));
                }
            }
        }
    }
}