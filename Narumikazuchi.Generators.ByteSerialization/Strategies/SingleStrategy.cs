namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="Single"/>.
/// </summary>
[FixedSerializationSize(sizeof(Single))]
public readonly struct SingleStrategy : IByteSerializationStrategy<Single>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Single Deserialize(ReadOnlySpan<Byte> buffer,
                                     out Int32 read)
    {
        read = sizeof(Single);
        return Unsafe.ReadUnaligned<Single>(ref MemoryMarshal.GetReference(buffer));
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(Single value)
    {
        return sizeof(Single);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  Single value)
    {
        Unsafe.As<Byte, Single>(ref MemoryMarshal.GetReference(buffer)) = value;
        return sizeof(Single);
    }
}