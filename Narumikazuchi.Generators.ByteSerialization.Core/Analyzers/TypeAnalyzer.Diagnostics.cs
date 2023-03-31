namespace Narumikazuchi.Generators.ByteSerialization.Analyzers;

public partial class TypeAnalyzer
{
    static public Diagnostic CreateNoSerializationForTypeDiagnostic(TypeDeclarationSyntax type,
                                                                    String membername,
                                                                    String typename)
    {
        return Diagnostic.Create(descriptor: s_NoSerializationForTypeDescriptor,
                                 location: type.GetLocation(),
                                 membername,
                                 typename);
    }

    static public Diagnostic CreateDuplicateStrategyForTypeDiagnostic(AttributeSyntax attribute,
                                                                      String typename)
    {
        return Diagnostic.Create(descriptor: s_DuplicateStrategyForTypeDescriptor,
                                 location: attribute.GetLocation(),
                                 typename);
    }

    static private readonly DiagnosticDescriptor s_NoSerializationForTypeDescriptor = new(id: "NCG009",
                                                                                          category: "Code Generation",
                                                                                          title: "No seralization for member type possible",
                                                                                          messageFormat: "For the member '{0}' of type '{1}' is no serialization possible. If you own the type consider decorating it with 'ByteSerializable' or otherwise implement a strategy to serialize that type.",
                                                                                          description: "For the member '{0}' of type '{1}' is no serialization possible. If you own the type consider decorating it with 'ByteSerializable' or otherwise implement a strategy to serialize that type.",
                                                                                          defaultSeverity: DiagnosticSeverity.Error,
                                                                                          isEnabledByDefault: true);
    static private readonly DiagnosticDescriptor s_DuplicateStrategyForTypeDescriptor = new(id: "NCG010",
                                                                                            category: "Code Generation",
                                                                                            title: "Multiple strategies defined for the same type",
                                                                                            messageFormat: "You already defined a strategy for the serialization of type '{0}'.",
                                                                                            description: "You already defined a strategy for the serialization of type '{0}'.",
                                                                                            defaultSeverity: DiagnosticSeverity.Error,
                                                                                            isEnabledByDefault: true);
}