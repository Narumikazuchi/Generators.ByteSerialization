namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Half"/>.
/// </summary>
[FixedSerializationSize(2)]
public readonly struct HalfStrategy : IByteSerializationStrategy<Half>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Half Deserialize(ReadOnlySpan<Byte> buffer,
                                   out Int32 read)
    {
        read = 2;
        return Unsafe.ReadUnaligned<Half>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Half value)
    {
        return 2;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Half value)
    {
        Unsafe.As<Byte, Half>(ref MemoryMarshal.GetReference(buffer)) = value;
        return 2;
    }
}