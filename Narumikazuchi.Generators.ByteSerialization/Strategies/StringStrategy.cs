namespace Narumikazuchi.Generators.ByteSerialization.Strategies;

/// <summary>
/// Provides Methods to serialize a runtime object of type <see cref="String"/>.
/// </summary>
public readonly struct StringStrategy : IByteSerializationStrategy<String?>
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static String? Deserialize(ReadOnlySpan<Byte> buffer,
                                      out Int32 read)
    {
        Int32 length = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer));
        if (length == 0)
        {
            read = sizeof(Int32);
            return null;
        }
        else
        {
            String result = Encoding.UTF8.GetString(buffer[4..(4 + length)]);
            read = sizeof(Int32) + length;
            return result;
        }
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 GetExpectedByteSize(String? value)
    {
        if (value is null)
        {
            return sizeof(Int32);
        }
        else
        {
            return sizeof(Int32) + Encoding.UTF8.GetByteCount(value);
        }
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Serialize(Span<Byte> buffer,
                                  String? value)
    {
        if (value is null)
        {
            Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer)) = 0;
            return sizeof(Int32);
        }
        else
        {
            Int32 length = Encoding.UTF8.GetBytes(chars: value,
                                                  bytes: buffer[4..]);
            Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer)) = length;
            return sizeof(Int32) + length;
        }
    }
}