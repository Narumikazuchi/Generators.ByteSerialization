namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Int32"/>.
/// </summary>
[FixedSerializationSize(sizeof(Int32))]
public readonly struct Int32Strategy : IByteSerializationStrategy<Int32>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Deserialize(ReadOnlySpan<Byte> buffer,
                                    out Int32 read)
    {
        read = sizeof(Int32);
        return Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Int32 value)
    {
        return sizeof(Int32);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Int32 value)
    {
        Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(Int32);
    }
}