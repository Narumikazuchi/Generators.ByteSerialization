using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

public partial class DataLayoutAnalyzer
{
    static public Diagnostic CreateDataLayoutOutOfBoundsDiagnostic(AttributeArgumentSyntax argument)
    {
        return Diagnostic.Create(descriptor: s_DataLayoutOutOfBoundsDescriptor,
                                 location: argument.GetLocation());
    }

    static public Diagnostic CreateMemberNotExplicitlyMarkedDiagnostic(MemberDeclarationSyntax member)
    {
        return Diagnostic.Create(descriptor: s_MemberNotExplicitlyMarkedDescriptor,
                                 location: member.GetLocation());
    }
    static public Diagnostic CreateMemberNotExplicitlyMarkedDiagnostic(ParameterSyntax recordParameter)
    {
        return Diagnostic.Create(descriptor: s_MemberNotExplicitlyMarkedDescriptor,
                                 location: recordParameter.GetLocation());
    }

    static public Diagnostic CreateLayoutPositionWithoutEffectDiagnostic(AttributeSyntax attribute)
    {
        return Diagnostic.Create(descriptor: s_LayoutPositionWithoutEffectDescriptor,
                                 location: attribute.GetLocation());
    }

    static public Diagnostic CreateLayoutPositionWithoutEffectNoRecordDiagnostic(AttributeSyntax attribute)
    {
        return Diagnostic.Create(descriptor: s_LayoutPositionWithoutEffectNoRecordDescriptor,
                                 location: attribute.GetLocation());
    }

    static public Diagnostic CreateLayoutPositionWithoutEffectNoConstructorDiagnostic(AttributeSyntax attribute)
    {
        return Diagnostic.Create(descriptor: s_LayoutPositionWithoutEffectNoConstructorDescriptor,
                                 location: attribute.GetLocation());
    }

    static private readonly DiagnosticDescriptor s_DataLayoutOutOfBoundsDescriptor = new(id: "NCG019",
                                                                                         category: "Code Generation",
                                                                                         title: "DataLayout value is invalid",
                                                                                         messageFormat: "The provided DataLayout value is not valid.",
                                                                                         description: null,
                                                                                         defaultSeverity: DiagnosticSeverity.Error,
                                                                                         isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_MemberNotExplicitlyMarkedDescriptor = new(id: "NCG020",
                                                                                             category: "Code Generation",
                                                                                             title: "Member has no position in explicit data layout",
                                                                                             messageFormat: "All public mutable data members of a type with explicit data layout require the 'DataLayoutPosition' attribute.",
                                                                                             description: null,
                                                                                             defaultSeverity: DiagnosticSeverity.Error,
                                                                                             isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_LayoutPositionWithoutEffectNoRecordDescriptor = new(id: "NCG021",
                                                                                                       category: "Code Generation",
                                                                                                       title: "'DataLayoutPosition' attribute has no effect",
                                                                                                       messageFormat: "'DataLayoutPosition' attribute has no effect due to the type not being a record.",
                                                                                                       description: null,
                                                                                                       defaultSeverity: DiagnosticSeverity.Warning,
                                                                                                       isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_LayoutPositionWithoutEffectNoConstructorDescriptor = new(id: "NCG022",
                                                                                                            category: "Code Generation",
                                                                                                            title: "'DataLayoutPosition' attribute has no effect",
                                                                                                            messageFormat: "'DataLayoutPosition' attribute has no effect due to the method not being a constructor.",
                                                                                                            description: null,
                                                                                                            defaultSeverity: DiagnosticSeverity.Warning,
                                                                                                            isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_LayoutPositionWithoutEffectDescriptor = new(id: "NCG023",
                                                                                               category: "Code Generation",
                                                                                               title: "'DataLayoutPosition' attribute has no effect",
                                                                                               messageFormat: "'DataLayoutPosition' attribute has no effect when the containing type does not have an explicit data layout.",
                                                                                               description: null,
                                                                                               defaultSeverity: DiagnosticSeverity.Warning,
                                                                                               isEnabledByDefault: true);
}