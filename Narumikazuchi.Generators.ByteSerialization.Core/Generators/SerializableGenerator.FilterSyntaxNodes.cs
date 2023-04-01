namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private Boolean IsEligableTypeSyntax(SyntaxNode syntaxNode,
                                                CancellationToken cancellationToken = default)
    {
        if (syntaxNode is TypeDeclarationSyntax type)
        {
            if (type.AttributeLists.Count is 0)
            {
                return false;
            }

            foreach (AttributeListSyntax attributeListSyntax in type.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (attributeSyntax.ToFullString().StartsWith("ByteSerializable"))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static private TypeDeclarationSyntax TransformToType(GeneratorSyntaxContext context,
                                                         CancellationToken cancellationToken = default)
    {
        TypeDeclarationSyntax type = (TypeDeclarationSyntax)context.Node;

        foreach (AttributeListSyntax attributeListSyntax in type.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax: attributeSyntax,
                                                        cancellationToken: cancellationToken)
                                         .Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                String fullName = attributeContainingTypeSymbol.ToFrameworkString();
                if (fullName is BYTESERIALIZABLE_ATTRIBUTE)
                {
                    return type;
                }
            }
        }

        return default;
    }
}