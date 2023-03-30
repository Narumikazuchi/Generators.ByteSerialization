namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Int64"/>.
/// </summary>
[FixedSerializationSize(sizeof(Int64))]
public readonly struct Int64Strategy : IByteSerializationStrategy<Int64>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 Deserialize(ReadOnlySpan<Byte> buffer,
                                    out Int32 read)
    {
        read = sizeof(Int64);
        return Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Int64 value)
    {
        return sizeof(Int64);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Int64 value)
    {
        Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(Int64);
    }
}