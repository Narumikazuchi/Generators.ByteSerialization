using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class TypeAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(action: this.Analyze,
                                         SyntaxKind.InvocationExpression);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new DiagnosticDescriptor[]
    {
        s_PointerNotSerializableDescriptor,
        s_OpenGenericsUnsupportedDescriptor,
        s_NoPublicMembersDescriptor,
        s_NoAbstractMembersDescriptor,
        s_ConsiderUnmanagedDescriptor
    }.ToImmutableArray();

    private void Analyze(SyntaxNodeAnalysisContext context)
    {
        InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)context.Node;
        ImmutableArray<ITypeSymbol> types = MethodToTypeReferenceFinder.FindTypes(compilation: context.SemanticModel.Compilation,
                                                                                  invocation: invocation);
        if (types.IsEmpty)
        {
            return;
        }

        foreach (INamedTypeSymbol type in types.OfType<INamedTypeSymbol>())
        {
            if (type.IsOpenGenericType())
            {
                context.ReportDiagnostic(CreateOpenGenericsUnsupportedDiagnostic(invocation));
            }

            if (type.SpecialType is SpecialType.System_IntPtr
                                 or SpecialType.System_UIntPtr
                                 or SpecialType.System_Delegate
                                 or SpecialType.System_MulticastDelegate)
            {
                context.ReportDiagnostic(CreatePointerNotSerializableDiagnostic(invocation));
            }

            ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
            if (members.IsEmpty &&
                !type.IsCollection(out _) &&
                !type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
            {
                if (type.IsAbstract)
                {
                    context.ReportDiagnostic(CreateNoAbstractMembersDiagnostic(method: invocation,
                                                                               typename: type.Name));
                }
                else
                {
                    context.ReportDiagnostic(CreateNoPublicMemberDiagnostic(method: invocation,
                                                                            typename: type.Name));
                }
            }

            if (type.ContainingAssembly is not null &&
                SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, context.SemanticModel.Compilation.Assembly) &&
                !type.IsValueType)
            {
                members = type.GetMembers()
                              .Where(member => member is IFieldSymbol
                                                      or IPropertySymbol)
                              .ToImmutableArray();
                if (members.Length > 0 &&
                    members.All(MemberIsUnmanaged))
                {
                    context.ReportDiagnostic(CreateConsiderUnmanagedDiagnostic(method: invocation,
                                                                               typename: type.Name));
                }
            }
        }
    }

    static private Boolean MemberIsUnmanaged(ISymbol member)
    {
        if (member is IFieldSymbol field)
        {
            return field.Type.IsUnmanagedSerializable();
        }
        else if (member is IPropertySymbol property)
        {
            return property.Type.IsUnmanagedSerializable();
        }
        else
        {
            return false;
        }
    }
}