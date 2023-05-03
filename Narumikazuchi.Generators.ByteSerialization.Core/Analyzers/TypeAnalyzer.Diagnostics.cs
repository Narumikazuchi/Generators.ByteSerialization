using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

public partial class TypeAnalyzer
{
    static public Diagnostic CreateNoDefaultConstructorDiagnostic(TypeDeclarationSyntax type)
    {
        return Diagnostic.Create(descriptor: s_NoDefaultConstructorDescriptor,
                                 location: type.Identifier.GetLocation());
    }

    static public Diagnostic CreateMoreThanOneSerializerDefinedDiagnostic(TypeDeclarationSyntax type,
                                                                          String typename)
    {
        return Diagnostic.Create(descriptor: s_MoreThanOneSerializerDefinedDescriptor,
                                 location: type.Identifier.GetLocation(),
                                 typename);
    }

    static private readonly DiagnosticDescriptor s_NoDefaultConstructorDescriptor = new(id: "NCG017",
                                                                                        category: "Code Generation",
                                                                                        title: "Serialization handler has no default constructor",
                                                                                        messageFormat: "The serialization handler can not be used by the code generator unless it has a default constructor.",
                                                                                        description: null,
                                                                                        defaultSeverity: DiagnosticSeverity.Error,
                                                                                        isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_MoreThanOneSerializerDefinedDescriptor = new(id: "NCG018",
                                                                                                category: "Code Generation",
                                                                                                title: "There already exists a serialization handler for this type",
                                                                                                messageFormat: "More than one serialization handler exists for type '{0}'. The code generator is currently unable to determine the preferred handler for this type.",
                                                                                                description: null,
                                                                                                defaultSeverity: DiagnosticSeverity.Error,
                                                                                                isEnabledByDefault: true);
}