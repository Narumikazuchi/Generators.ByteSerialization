namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="SByte"/>.
/// </summary>
[FixedSerializationSize(sizeof(SByte))]
public readonly struct SByteStrategy : IByteSerializationStrategy<SByte>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SByte Deserialize(ReadOnlySpan<Byte> buffer,
                                    out Int32 read)
    {
        SByte @byte = unchecked((SByte)buffer[0]);
        read = 1;
        return @byte;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(SByte value)
    {
        return sizeof(SByte);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  SByte value)
    {
        buffer[0] = unchecked((Byte)value);
        return sizeof(SByte);
    }
}