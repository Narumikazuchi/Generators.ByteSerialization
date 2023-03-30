namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Byte"/>.
/// </summary>
[FixedSerializationSize(sizeof(Byte))]
public readonly struct ByteStrategy : IByteSerializationStrategy<Byte>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte Deserialize(ReadOnlySpan<Byte> buffer,
                                   out Int32 read)
    {
        Byte @byte = buffer[0];
        read = 1;
        return @byte;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Byte value)
    {
        return sizeof(Byte);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Byte value)
    {
        buffer[0] = value;
        return sizeof(Byte);
    }
}