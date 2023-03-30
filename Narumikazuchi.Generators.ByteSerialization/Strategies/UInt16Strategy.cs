namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="UInt16"/>.
/// </summary>
[FixedSerializationSize(sizeof(UInt16))]
public readonly struct UInt16Strategy : IByteSerializationStrategy<UInt16>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt16 Deserialize(ReadOnlySpan<Byte> buffer,
                                     out Int32 read)
    {
        read = sizeof(UInt16);
        return Unsafe.ReadUnaligned<UInt16>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(UInt16 value)
    {
        return sizeof(UInt16);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  UInt16 value)
    {
        Unsafe.As<Byte, UInt16>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(UInt16);
    }
}