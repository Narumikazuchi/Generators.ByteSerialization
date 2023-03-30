namespace Narumikazuchi.Generators.ByteSerialization;

static public class Extensions
{
    static public String ToTypename(this ITypeSymbol type)
    {
        if (type.ContainingNamespace.ToDisplayString() is "System")
        {
            return type.Name;
        }
        else
        {
            return type.ToDisplayString();
        }
    }
}