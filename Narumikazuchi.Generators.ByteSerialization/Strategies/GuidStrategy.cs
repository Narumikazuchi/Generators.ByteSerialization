namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Guid"/>.
/// </summary>
[FixedSerializationSize(16)]
public readonly struct GuidStrategy : IByteSerializationStrategy<Guid>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid Deserialize(ReadOnlySpan<Byte> buffer,
                                   out Int32 read)
    {
        read = 16;
        return new(buffer[..16]);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Guid value)
    {
        return 16;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Guid value)
    {
        Byte[] array = value.ToByteArray();
        Int32 index = -1;
        while (++index < 16)
        {
            buffer[index] = array[index];
        }

        return 16;
    }
}