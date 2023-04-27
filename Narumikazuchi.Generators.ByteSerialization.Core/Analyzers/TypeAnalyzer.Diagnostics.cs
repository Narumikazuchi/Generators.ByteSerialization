using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

public partial class TypeAnalyzer
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
                                                                                          description: "Pointers may not be serialized.",
                                                                                          defaultSeverity: DiagnosticSeverity.Error,
                                                                                          isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_OpenGenericsUnsupportedDescriptor = new(id: "NCG010",
                                                                                           category: "Code Generation",
                                                                                           title: "Open generics are not supported",
                                                                                           messageFormat: "Open generics are currently not supported.",
                                                                                           description: "Open generics are currently not supported.",
                                                                                           defaultSeverity: DiagnosticSeverity.Error,
                                                                                           isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_NoPublicMembersDescriptor = new(id: "NCG011",
                                                                                   category: "Code Generation",
                                                                                   title: "No public members",
                                                                                   messageFormat: "The type '{0}' has no accessible public members to serialize. The object will take up space but will otherwise always be empty.",
                                                                                   description: "The type '{0}' has no accessible public members to serialize. The object will take up space but will otherwise always be empty.",
                                                                                   defaultSeverity: DiagnosticSeverity.Warning,
                                                                                   isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_NoAbstractMembersDescriptor = new(id: "NCG012",
                                                                                     category: "Code Generation",
                                                                                     title: "No public abstract members",
                                                                                     messageFormat: "The type '{0}' has no accessible public members to serialize. Be sure to have some members accessible on derived or implementing types, otherwise the serialized object will always be empty while still taking up space.",
                                                                                     description: "The type '{0}' has no accessible public members to serialize. Be sure to have some members accessible on derived or implementing types, otherwise the serialized object will always be empty while still taking up space.",
                                                                                     defaultSeverity: DiagnosticSeverity.Info,
                                                                                     isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_ConsiderUnmanagedDescriptor = new(id: "NCG013",
                                                                                     category: "Code Generation",
                                                                                     title: "Consider making type unmanaged",
                                                                                     messageFormat: "The type '{0}' consists of unmanaged types only. To increase serialization performance consider changing it into a value type if you don't require a reference type.",
                                                                                     description: "The type '{0}' consists of unmanaged types only. To increase serialization performance consider changing it into a value type if you don't require a reference type.",
                                                                                     defaultSeverity: DiagnosticSeverity.Info,
                                                                                     isEnabledByDefault: true);
}