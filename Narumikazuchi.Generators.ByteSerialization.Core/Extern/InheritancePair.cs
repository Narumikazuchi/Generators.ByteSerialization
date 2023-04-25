using Microsoft.CodeAnalysis;

namespace Narumikazuchi.CodeAnalysis;

public readonly struct InheritancePair : IEquatable<InheritancePair>
{
    public InheritancePair(INamedTypeSymbol derived,
                           INamedTypeSymbol @base)
    {
        m_Base = @base;
        m_Derived = derived;
    }

    public Boolean Equals(InheritancePair other)
    {
        return SymbolEqualityComparer.Default.Equals(m_Base, other.m_Base) &&
               SymbolEqualityComparer.Default.Equals(m_Derived, other.m_Derived);
    }

    public override Boolean Equals(Object obj)
    {
        return obj is InheritancePair other &&
               this.Equals(other);
    }

    public override Int32 GetHashCode()
    {
        return SymbolEqualityComparer.Default.GetHashCode(m_Base) ^
               SymbolEqualityComparer.Default.GetHashCode(m_Derived);
    }

    private readonly INamedTypeSymbol m_Base;
    private readonly INamedTypeSymbol m_Derived;
}