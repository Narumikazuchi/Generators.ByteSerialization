namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents a semi-unique identifier for a type.
/// </summary>
/// <remarks>
/// The identifier is basically a SHA-512 hash, which makes collision as unlikely as possible.
/// This doesn't mean they can't happen, it's just extremely unlikely.
/// </remarks>
public readonly record struct TypeIdentifier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeIdentifier"/> struct.
    /// </summary>
    /// <param name="type">The type to create an identifier for.</param>
    /// <exception cref="ArgumentNullException"/>
    static public TypeIdentifier CreateFrom(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (s_Cached.TryGetValue(key: type,
                                 value: out TypeIdentifier result))
        {
            return result;
        }
        else
        {
            result = new(SHA512.HashData(MemoryMarshal.AsBytes(type.AssemblyQualifiedName.AsSpan())));
            s_Cached.Add(key: type,
                         value: result);
            return result;
        }
    }

    private TypeIdentifier(ReadOnlySpan<Byte> bytes)
    {
        m_First = Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(bytes));
        m_Second = Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(bytes[8..]));
        m_Third = Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(bytes[16..]));
        m_Fourth = Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(bytes[24..]));
    }

    /// <summary>
    /// Returns the string-representation of this identifier.
    /// </summary>
    /// <returns>The string-representation of this identifier.</returns>
    public override String ToString()
    {
        Byte[] bytes = new Byte[8];
        StringBuilder builder = new();
        Unsafe.As<Byte, Int64>(ref bytes[0]) = m_First;
        builder.Append(new String(bytes.SelectMany(b => b.ToString("X2").ToLowerInvariant()).ToArray()));
        builder.Append('-');
        Unsafe.As<Byte, Int64>(ref bytes[0]) = m_Second;
        builder.Append(new String(bytes.SelectMany(b => b.ToString("X2").ToLowerInvariant()).ToArray()));
        builder.Append('-');
        Unsafe.As<Byte, Int64>(ref bytes[0]) = m_Third;
        builder.Append(new String(bytes.SelectMany(b => b.ToString("X2").ToLowerInvariant()).ToArray()));
        builder.Append('-');
        Unsafe.As<Byte, Int64>(ref bytes[0]) = m_Fourth;
        builder.Append(new String(bytes.SelectMany(b => b.ToString("X2").ToLowerInvariant()).ToArray()));
        return builder.ToString();
    }

    static private readonly Dictionary<Type, TypeIdentifier> s_Cached = new();

    private readonly Int64 m_First;
    private readonly Int64 m_Second;
    private readonly Int64 m_Third;
    private readonly Int64 m_Fourth;
}