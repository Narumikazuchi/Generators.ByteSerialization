namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Char"/>.
/// </summary>
[FixedSerializationSize(sizeof(Char))]
public readonly struct CharStrategy : IByteSerializationStrategy<Char>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Char Deserialize(ReadOnlySpan<Byte> buffer,
                                   out Int32 read)
    {
        read = sizeof(Char);
        return Unsafe.ReadUnaligned<Char>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Char value)
    {
        return sizeof(Char);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Char value)
    {
        Unsafe.As<Byte, Char>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(Char);
    }
}