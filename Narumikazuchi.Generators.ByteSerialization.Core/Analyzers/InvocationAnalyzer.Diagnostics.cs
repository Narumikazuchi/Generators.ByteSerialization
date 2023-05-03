using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

public partial class InvocationAnalyzer
{
    static public Diagnostic CreatePointerNotSerializableDiagnostic(InvocationExpressionSyntax method)
    {
        return Diagnostic.Create(descriptor: s_PointerNotSerializableDescriptor,
                                 location: method.GetLocation());
    }

    static public Diagnostic CreateOpenGenericsUnsupportedDiagnostic(InvocationExpressionSyntax method)
    {
        return Diagnostic.Create(descriptor: s_OpenGenericsUnsupportedDescriptor,
                                 location: method.GetLocation());
    }

    static public Diagnostic CreateNoImplementationDiagnostic(InvocationExpressionSyntax method,
                                                              String typename)
    {
        return Diagnostic.Create(descriptor: s_NoImplementationDescriptor,
                                 location: method.GetLocation(),
                                 typename);
    }

    static public Diagnostic CreateTypeNotSerializableDiagnostic(InvocationExpressionSyntax method,
                                                                 String typename)
    {
        return Diagnostic.Create(descriptor: s_TypeNotSerializableDescriptor,
                                 location: method.GetLocation(),
                                 typename);
    }

    static public Diagnostic CreateMemberNotSerializableDiagnostic(InvocationExpressionSyntax method,
                                                                   String typename,
                                                                   String memberName,
                                                                   String memberType)
    {
        return Diagnostic.Create(descriptor: s_MemberNotSerializableDescriptor,
                                 location: method.GetLocation(),
                                 typename,
                                 memberName,
                                 memberType);
    }

    static public Diagnostic CreateNoPublicMemberDiagnostic(InvocationExpressionSyntax method,
                                                            String typename)
    {
        return Diagnostic.Create(descriptor: s_NoPublicMembersDescriptor,
                                 location: method.GetLocation(),
                                 typename);
    }

    static public Diagnostic CreateNoAbstractMembersDiagnostic(InvocationExpressionSyntax method,
                                                               String typename)
    {
        return Diagnostic.Create(descriptor: s_NoAbstractMembersDescriptor,
                                 location: method.GetLocation(),
                                 typename);
    }

    static public Diagnostic CreateConsiderUnmanagedDiagnostic(InvocationExpressionSyntax method,
                                                               String typename)
    {
        return Diagnostic.Create(descriptor: s_ConsiderUnmanagedDescriptor,
                                 location: method.GetLocation(),
                                 typename);
    }

    static private readonly DiagnosticDescriptor s_PointerNotSerializableDescriptor = new(id: "NCG009",
                                                                                          category: "Code Generation",
                                                                                          title: "Pointers may not be serialized",
                                                                                          messageFormat: "Pointers may not be serialized.",
                                                                                          description: null,
                                                                                          defaultSeverity: DiagnosticSeverity.Error,
                                                                                          isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_OpenGenericsUnsupportedDescriptor = new(id: "NCG010",
                                                                                           category: "Code Generation",
                                                                                           title: "Open generics are not supported",
                                                                                           messageFormat: "Open generics are currently not supported.",
                                                                                           description: null,
                                                                                           defaultSeverity: DiagnosticSeverity.Error,
                                                                                           isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_NoImplementationDescriptor = new(id: "NCG011",
                                                                                    category: "Code Generation",
                                                                                    title: "No implementation found",
                                                                                    messageFormat: "There is no deriving or implementing type for the interface or abstract class '{0}' defined.",
                                                                                    description: null,
                                                                                    defaultSeverity: DiagnosticSeverity.Error,
                                                                                    isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_MemberNotSerializableDescriptor = new(id: "NCG012",
                                                                                         category: "Code Generation",
                                                                                         title: "Type not serializable",
                                                                                         messageFormat: "The type '{0}' has a public member '{1}' of type '{2}', which can not be serialized.",
                                                                                         description: "If you do not own the type, consider implementing a 'ISerializationHandler<T>' of the respective type.",
                                                                                         defaultSeverity: DiagnosticSeverity.Error,
                                                                                         isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_TypeNotSerializableDescriptor = new(id: "NCG013",
                                                                                       category: "Code Generation",
                                                                                       title: "Type not serializable",
                                                                                       messageFormat: "The type '{0}' has no accessible and usable constructor.",
                                                                                       description: "If you own the type consider implementing a parameterless constructor or decorate the parameters of your constructor with the 'SerializeDefaultAttribute<T>' or 'SerializeFromMemberAttribute'.",
                                                                                       defaultSeverity: DiagnosticSeverity.Error,
                                                                                       isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_NoPublicMembersDescriptor = new(id: "NCG014",
                                                                                   category: "Code Generation",
                                                                                   title: "No public members",
                                                                                   messageFormat: "The type '{0}' has no accessible public members to serialize. The object will take up space but will otherwise always be empty.",
                                                                                   description: null,
                                                                                   defaultSeverity: DiagnosticSeverity.Warning,
                                                                                   isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_NoAbstractMembersDescriptor = new(id: "NCG015",
                                                                                     category: "Code Generation",
                                                                                     title: "No public abstract members",
                                                                                     messageFormat: "The type '{0}' has no accessible public members to serialize. Be sure to have some members accessible on derived or implementing types, otherwise the serialized object will always be empty while still taking up space.",
                                                                                     description: null,
                                                                                     defaultSeverity: DiagnosticSeverity.Info,
                                                                                     isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_ConsiderUnmanagedDescriptor = new(id: "NCG016",
                                                                                     category: "Code Generation",
                                                                                     title: "Consider making type unmanaged",
                                                                                     messageFormat: "The type '{0}' consists of unmanaged types only. To increase serialization performance consider changing it into a value type if you don't require a reference type.",
                                                                                     description: "Unmanaged types can be directly serialized to a byte representation through marshalling, which is faster than serializing fields and properties step-by-step.",
                                                                                     defaultSeverity: DiagnosticSeverity.Info,
                                                                                     isEnabledByDefault: true);
}