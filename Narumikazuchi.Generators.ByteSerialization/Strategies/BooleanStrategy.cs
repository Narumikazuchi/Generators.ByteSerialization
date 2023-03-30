namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Boolean"/>.
/// </summary>
[FixedSerializationSize(sizeof(Byte))]
public readonly struct BooleanStrategy : IByteSerializationStrategy<Boolean>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean Deserialize(ReadOnlySpan<Byte> buffer,
                                      out Int32 read)
    {
        Byte @byte = buffer[0];
        read = sizeof(Byte);
        return @byte != 0x0;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Boolean value)
    {
        return sizeof(Byte);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Boolean value)
    {
        Byte @byte = 0x0;
        if (value)
        {
            @byte = 0x1;
        }

        buffer[0] = @byte;
        return sizeof(Byte);
    }
}