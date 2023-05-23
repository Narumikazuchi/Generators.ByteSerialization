using System.Diagnostics.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

public partial struct TypeLayout : IEquatable<TypeLayout>
{
    /// <inheritdoc/>
    public Boolean Equals(TypeLayout other)
    {
        if (m_Type != other.m_Type)
        {
            return false;
        }

        if (m_Members is null &&
            other.m_Members is null)
        {
            return true;
        }

        if (m_Members is null &&
            other.m_Members is not null)
        {
            return false;
        }

        if (m_Members is not null &&
            other.m_Members is null)
        {
            return false;
        }

        if (m_Members!.Length != other.m_Members!.Length)
        {
            return false;
        }

        for (Int32 counter = 0;
             counter < m_Members.Length;
             counter++)
        {
            if (m_Members[counter] != other.m_Members[counter])
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override Boolean Equals([NotNullWhen(true)] Object? obj)
    {
        return obj is TypeLayout other &&
               this.Equals(other);
    }

    /// <inheritdoc/>
    public override Int32 GetHashCode()
    {
        return HashCode.Combine(m_Type, m_Members);
    }
}