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
                                         SyntaxKind.MethodDeclaration);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new DiagnosticDescriptor[]
    {
        s_PointerNotSerializableDescriptor,
        s_OpenGenericsUnsupportedDescriptor,
        s_NoPublicMembersDescriptor,
        s_NoInterfaceMembersDescriptor,
        s_ConsiderUnmanagedDescriptor
    }.ToImmutableArray();

    private void Analyze(SyntaxNodeAnalysisContext context)
    {
        INamedTypeSymbol compilerGenerated = context.SemanticModel.Compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.CompilerGeneratedAttribute");

        MethodDeclarationSyntax method = (MethodDeclarationSyntax)context.Node;
        if (method.AttributeLists.SelectMany(list => list.Attributes)
                                 .Any(attribute => SymbolEqualityComparer.Default.Equals(compilerGenerated, context.SemanticModel.GetSymbolInfo(attribute).Symbol.ContainingType)))
        {
            return;
        }

        ImmutableArray<ITypeSymbol> types = MethodToTypeReferenceFinder.FindTypes(compilation: context.SemanticModel.Compilation,
                                                                                  methodSyntax: method);
        if (types.IsEmpty)
        {
            return;
        }

        foreach (INamedTypeSymbol type in types.OfType<INamedTypeSymbol>())
        {
            ImmutableArray<InvocationExpressionSyntax> invocations = FindInvocationsForType(parent: method,
                                                                                            semanticModel: context.SemanticModel,
                                                                                            type: type);

            if (type.IsOpenGenericType())
            {
                foreach (InvocationExpressionSyntax invocation in invocations)
                {
                    context.ReportDiagnostic(CreateOpenGenericsUnsupportedDiagnostic(invocation));
                }
            }

            if (type.SpecialType is SpecialType.System_IntPtr
                                 or SpecialType.System_UIntPtr
                                 or SpecialType.System_Delegate
                                 or SpecialType.System_MulticastDelegate)
            {
                foreach (InvocationExpressionSyntax invocation in invocations)
                {
                    context.ReportDiagnostic(CreatePointerNotSerializableDiagnostic(invocation));
                }
            }

            ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
            if (members.IsEmpty &&
                !type.IsCollection(out _) &&
                !type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
            {
                if (type.IsAbstract)
                {
                    foreach (InvocationExpressionSyntax invocation in invocations)
                    {
                        context.ReportDiagnostic(CreateNoInterfaceMembersDiagnostic(method: invocation,
                                                                                    typename: type.Name));
                    }
                }
                else
                {
                    foreach (InvocationExpressionSyntax invocation in invocations)
                    {
                        context.ReportDiagnostic(CreateNoPublicMemberDiagnostic(method: invocation,
                                                                                typename: type.Name));
                    }
                }
            }

            members = type.GetMembers()
                          .Where(member => member is IFieldSymbol
                                                  or IPropertySymbol)
                          .ToImmutableArray();
            if (!type.ToFrameworkString().StartsWith("System.") &&
                !type.IsValueType &&
                members.Length > 0 &&
                members.All(MemberIsUnmanaged))
            {
                foreach (InvocationExpressionSyntax invocation in invocations)
                {
                    context.ReportDiagnostic(CreateConsiderUnmanagedDiagnostic(method: invocation,
                                                                               typename: type.Name));
                }
            }
        }
    }

    static private ImmutableArray<InvocationExpressionSyntax> FindInvocationsForType(INamedTypeSymbol type,
                                                                                     SemanticModel semanticModel,
                                                                                     SyntaxNode parent)
    {
        ImmutableArray<InvocationExpressionSyntax>.Builder builder = ImmutableArray.CreateBuilder<InvocationExpressionSyntax>();
        foreach (SyntaxNode child in parent.ChildNodes())
        {
            if (child is InvocationExpressionSyntax invocation)
            {
                IMethodSymbol method = (IMethodSymbol)semanticModel.GetSymbolInfo(invocation).Symbol;
                if (MethodToTypeReferenceFinder.IsByteSerializerMethod(method: method,
                                                                       compilation: semanticModel.Compilation))
                {
                    ITypeSymbol methodType = method.TypeArguments.Last();
                    if (TypeContainsType(parentType: methodType,
                                         type: type))
                    {
                        builder.Add(invocation);
                    }
                }
            }

            ImmutableArray<InvocationExpressionSyntax> result = FindInvocationsForType(type: type,
                                                                                       semanticModel: semanticModel,
                                                                                       parent: child);
            builder.AddRange(result);
        }

        return builder.ToImmutable();
    }

    static private Boolean TypeContainsType(ITypeSymbol parentType,
                                            ITypeSymbol type)
    {
        if (SymbolEqualityComparer.Default.Equals(parentType, type))
        {
            return true;
        }
        else if (parentType is INamedTypeSymbol named &&
                 named.IsGenericType)
        {
            foreach (ITypeSymbol typeArgument in named.TypeArguments)
            {
                if (SymbolEqualityComparer.Default.Equals(type, typeArgument))
                {
                    return true;
                }
            }

            return false;
        }
        else if (parentType is IArrayTypeSymbol array)
        {
            return TypeContainsType(parentType: array.ElementType,
                                    type: type);
        }
        else
        {
            return false;
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