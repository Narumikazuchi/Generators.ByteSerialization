namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="UInt64"/>.
/// </summary>
[FixedSerializationSize(sizeof(UInt64))]
public readonly struct UInt64Strategy : IByteSerializationStrategy<UInt64>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 Deserialize(ReadOnlySpan<Byte> buffer,
                                     out Int32 read)
    {
        read = sizeof(UInt64);
        return Unsafe.ReadUnaligned<UInt64>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(UInt64 value)
    {
        return sizeof(UInt64);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  UInt64 value)
    {
        Unsafe.As<Byte, UInt64>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(UInt64);
    }
}