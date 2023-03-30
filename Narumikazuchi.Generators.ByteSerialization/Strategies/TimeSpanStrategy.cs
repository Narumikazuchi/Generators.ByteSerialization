namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="TimeSpan"/>.
/// </summary>
[FixedSerializationSize(sizeof(Int64))]
public readonly struct TimeSpanStrategy : IByteSerializationStrategy<TimeSpan>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan Deserialize(ReadOnlySpan<Byte> buffer,
                                       out Int32 read)
    {
        read = sizeof(Int64);
        return new(Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer)));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(TimeSpan value)
    {
        return sizeof(Int64);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  TimeSpan value)
    {
        Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(buffer)) = value.Ticks;
        return sizeof(Int64);
    }
}