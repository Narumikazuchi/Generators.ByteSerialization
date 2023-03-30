namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Double"/>.
/// </summary>
[FixedSerializationSize(sizeof(Double))]
public readonly struct DoubleStrategy : IByteSerializationStrategy<Double>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Double Deserialize(ReadOnlySpan<Byte> buffer,
                                     out Int32 read)
    {
        read = sizeof(Double);
        return Unsafe.ReadUnaligned<Double>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Double value)
    {
        return sizeof(Double);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Double value)
    {
        Unsafe.As<Byte, Double>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(Double);
    }
}