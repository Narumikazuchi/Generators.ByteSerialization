namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="DateTimeOffset"/>.
/// </summary>
[FixedSerializationSize(sizeof(Int64))]
public readonly struct DateTimeOffsetStrategy : IByteSerializationStrategy<DateTimeOffset>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset Deserialize(ReadOnlySpan<Byte> buffer,
                                             out Int32 read)
    {
        read = sizeof(Int64);
        return new(ticks: Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer)),
                   offset: default);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(DateTimeOffset value)
    {
        return sizeof(Int64);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  DateTimeOffset value)
    {
        Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(buffer)) = value.Ticks;
        return sizeof(Int64);
    }
}