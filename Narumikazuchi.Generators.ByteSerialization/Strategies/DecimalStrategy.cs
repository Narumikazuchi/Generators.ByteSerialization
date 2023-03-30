namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Decimal"/>.
/// </summary>
[FixedSerializationSize(4 * sizeof(Int32))]
public readonly struct DecimalStrategy : IByteSerializationStrategy<Decimal>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Decimal Deserialize(ReadOnlySpan<Byte> buffer,
                                      out Int32 read)
    {
        read = 4 * sizeof(Int32);
        return new(new Int32[]
        {
            Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer)),
            Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer[4..])),
            Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer[8..])),
            Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer[12..]))
        });
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Decimal value)
    {
        return 4 * sizeof(Int32);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Decimal value)
    {
        Int32[] bits = Decimal.GetBits(value);
        Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer)) = bits[0];
        Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[4..])) = bits[1];
        Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[8..])) = bits[2];
        Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[12..])) = bits[3];
        return 4 * sizeof(Int32);
    }
}