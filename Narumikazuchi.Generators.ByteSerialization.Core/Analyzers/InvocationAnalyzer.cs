using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class InvocationAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        if (!Debugger.IsAttached)
        {
            context.EnableConcurrentExecution();
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(action: this.Analyze,
                                         SyntaxKind.InvocationExpression);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new DiagnosticDescriptor[]
    {
        s_PointerNotSerializableDescriptor,
        s_OpenGenericsUnsupportedDescriptor,
        s_NoImplementationDescriptor,
        s_TypeNotSerializableDescriptor,
        s_MemberNotSerializableDescriptor,
        s_NoPublicMembersDescriptor,
        s_NoAbstractMembersDescriptor,
        s_ConsiderUnmanagedDescriptor
    }.ToImmutableArray();

    private void Analyze(SyntaxNodeAnalysisContext context)
    {
        InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)context.Node;
        ITypeSymbol type = MethodToTypeReferenceFinder.FilterType(compilation: context.SemanticModel.Compilation,
                                                                  invocation: invocation);
        if (type is null)
        {
            return;
        }

        ReportDiagnosticsForType(type: type,
                                 context: context,
                                 invocation: invocation);
    }

    static private void ReportDiagnosticsForType(ITypeSymbol type,
                                                 SyntaxNodeAnalysisContext context,
                                                 InvocationExpressionSyntax invocation)
    {
        if (type.IsUnmanagedSerializable() ||
            type.SpecialType is SpecialType.System_String)
        {
            return;
        }

        ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> customSerializers = CustomHandlerFinder.FindTypesWithCustomHandlerIn(context.Compilation);

        if (customSerializers.TryGetValue(key: type,
                                          value: out ImmutableHashSet<INamedTypeSymbol> serializers) &&
            serializers.Count is 1)
        {
            return;
        }
        else if (type is INamedTypeSymbol named)
        {
            if (named.SpecialType is SpecialType.System_IntPtr
                                  or SpecialType.System_UIntPtr
                                  or SpecialType.System_Delegate
                                  or SpecialType.System_MulticastDelegate)
            {
                context.ReportDiagnostic(CreatePointerNotSerializableDiagnostic(invocation));
                return;
            }

            if (named.IsOpenGenericType())
            {
                context.ReportDiagnostic(CreateOpenGenericsUnsupportedDiagnostic(invocation));
            }

            if (named.IsAbstract &&
                named.GetDerivedTypes().Length is 0)
            {
                context.ReportDiagnostic(CreateNoImplementationDiagnostic(method: invocation,
                                                                          typename: named.Name));
            }

            ImmutableArray<ISymbol> members = named.GetMembersToSerialize();
            if (members.IsEmpty &&
                !named.IsCollection(out _) &&
                !named.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
            {
                if (named.IsAbstract)
                {
                    context.ReportDiagnostic(CreateNoAbstractMembersDiagnostic(method: invocation,
                                                                               typename: named.Name));
                }
                else
                {
                    context.ReportDiagnostic(CreateNoPublicMemberDiagnostic(method: invocation,
                                                                            typename: named.Name));
                }
            }

            ImmutableArray<ISymbol> notSerializable = members.Where(member => !member.CanBeSerialized(customSerializers))
                                                             .ToImmutableArray();
            if (named.HasDefaultConstructor())
            {
                if (notSerializable.Length > 0)
                {
                    foreach (ISymbol member in notSerializable)
                    {
                        if (member is IFieldSymbol field)
                        {
                            context.ReportDiagnostic(CreateMemberNotSerializableDiagnostic(method: invocation,
                                                                                           typename: named.Name,
                                                                                           memberName: field.Name,
                                                                                           memberType: field.Type.Name));
                        }
                        else if (member is IPropertySymbol property)
                        {
                            context.ReportDiagnostic(CreateMemberNotSerializableDiagnostic(method: invocation,
                                                                                           typename: named.Name,
                                                                                           memberName: property.Name,
                                                                                           memberType: property.Type.Name));
                        }
                    }
                }
            }

            if (named.IsRecord)
            {
                if (notSerializable.Length > 0)
                {
                    foreach (ISymbol member in notSerializable)
                    {
                        if (member is IFieldSymbol field)
                        {
                            context.ReportDiagnostic(CreateMemberNotSerializableDiagnostic(method: invocation,
                                                                                           typename: named.Name,
                                                                                           memberName: field.Name,
                                                                                           memberType: field.Type.Name));
                        }
                        else if (member is IPropertySymbol property)
                        {
                            context.ReportDiagnostic(CreateMemberNotSerializableDiagnostic(method: invocation,
                                                                                           typename: named.Name,
                                                                                           memberName: property.Name,
                                                                                           memberType: property.Type.Name));
                        }
                    }
                }
            }

            if (named.ParameterizedConstructor() is not null)
            {
                if (notSerializable.Length > 0)
                {
                    foreach (ISymbol member in notSerializable)
                    {
                        if (member is IFieldSymbol field)
                        {
                            context.ReportDiagnostic(CreateMemberNotSerializableDiagnostic(method: invocation,
                                                                                           typename: named.Name,
                                                                                           memberName: field.Name,
                                                                                           memberType: field.Type.Name));
                        }
                        else if (member is IPropertySymbol property)
                        {
                            context.ReportDiagnostic(CreateMemberNotSerializableDiagnostic(method: invocation,
                                                                                           typename: named.Name,
                                                                                           memberName: property.Name,
                                                                                           memberType: property.Type.Name));
                        }
                    }
                }
            }

            if (!named.IsAbstract &&
                !named.HasDefaultConstructor() &&
                !named.IsRecord &&
                named.ParameterizedConstructor() is null)
            {
                context.ReportDiagnostic(CreateTypeNotSerializableDiagnostic(method: invocation,
                                                                             typename: named.Name));
            }

            if (named.ContainingAssembly is not null &&
                SymbolEqualityComparer.Default.Equals(named.ContainingAssembly, context.SemanticModel.Compilation.Assembly) &&
                !named.IsValueType)
            {
                members = named.GetMembers()
                               .Where(member => member is IFieldSymbol
                                                       or IPropertySymbol)
                               .ToImmutableArray();
                if (members.Length > 0 &&
                    members.All(MemberIsUnmanaged))
                {
                    context.ReportDiagnostic(CreateConsiderUnmanagedDiagnostic(method: invocation,
                                                                               typename: named.Name));
                }
            }
        }
        else if (type is IArrayTypeSymbol array)
        {
            ReportDiagnosticsForType(type: array.ElementType,
                                     context: context,
                                     invocation: invocation);
        }
        else if (type is ITypeParameterSymbol typeParameter &&
                 !typeParameter.HasUnmanagedTypeConstraint)
        {
            context.ReportDiagnostic(CreateOpenGenericsUnsupportedDiagnostic(invocation));
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