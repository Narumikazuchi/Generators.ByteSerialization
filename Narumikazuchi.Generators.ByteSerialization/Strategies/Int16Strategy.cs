namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Int16"/>.
/// </summary>
[FixedSerializationSize(sizeof(Int16))]
public readonly struct Int16Strategy : IByteSerializationStrategy<Int16>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int16 Deserialize(ReadOnlySpan<Byte> buffer,
                                    out Int32 read)
    {
        read = sizeof(Int16);
        return Unsafe.ReadUnaligned<Int16>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Int16 value)
    {
        return sizeof(Int16);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Int16 value)
    {
        Unsafe.As<Byte, Int16>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(Int16);
    }
}