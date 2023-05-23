namespace Narumikazuchi.Generators.ByteSerialization;

#pragma warning disable CS1591
public partial struct TypeLayout : IEqualityOperators<TypeLayout, TypeLayout, Boolean>
{
    static public Boolean operator ==(TypeLayout left,
                                      TypeLayout right)
    {
        return left.Equals(right);
    }

    static public Boolean operator !=(TypeLayout left,
                                      TypeLayout right)
    {
        return !left.Equals(right);
    }
}