namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="DateOnly"/>.
/// </summary>
[FixedSerializationSize(sizeof(Int16) + 2 * sizeof(Byte))]
public readonly struct DateOnlyStrategy : IByteSerializationStrategy<DateOnly>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateOnly Deserialize(ReadOnlySpan<Byte> buffer,
                                       out Int32 read)
    {
        read = sizeof(Int16) + 2 * sizeof(Byte);
        Int16 year = Unsafe.ReadUnaligned<Int16>(ref MemoryMarshal.GetReference(buffer));
        Byte month = buffer[2];
        Byte day = buffer[3];
        return new(year: year,
                   month: month,
                   day: day);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(DateOnly value)
    {
        return sizeof(Int16) + 2 * sizeof(Byte);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  DateOnly value)
    {
        Unsafe.As<Byte, Int16>(ref MemoryMarshal.GetReference(buffer)) = (Int16)value.Year;
        buffer[2] = (Byte)value.Month;
        buffer[3] = (Byte)value.Day;
        return sizeof(Int16) + 2 * sizeof(Byte);
    }
}