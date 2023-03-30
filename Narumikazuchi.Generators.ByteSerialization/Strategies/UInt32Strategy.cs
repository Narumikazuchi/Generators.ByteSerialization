namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="UInt32"/>.
/// </summary>
[FixedSerializationSize(sizeof(UInt32))]
public readonly struct UInt32Strategy : IByteSerializationStrategy<UInt32>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 Deserialize(ReadOnlySpan<Byte> buffer,
                                     out Int32 read)
    {
        read = sizeof(UInt32);
        return Unsafe.ReadUnaligned<UInt32>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(UInt32 value)
    {
        return sizeof(UInt32);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  UInt32 value)
    {
        Unsafe.As<Byte, UInt32>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(UInt32);
    }
}