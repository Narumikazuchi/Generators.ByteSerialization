namespace Narumikazuchi.Generators.ByteSerialization;

public partial interface IByteSerializer : ISerializationHandler<DateTimeOffset>
{
    Int32 ISerializationHandler<DateTimeOffset>.Deserialize(ReadOnlySpan<Byte> buffer,
                                                            out DateTimeOffset result)
    {
        result = new(Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer)), TimeSpan.Zero);
        return sizeof(Int64);
    }

     Int32 ISerializationHandler<DateTimeOffset>.GetExpectedSize(DateTimeOffset graph)
    {
        return sizeof(Int64);
    }

    Int32 ISerializationHandler<DateTimeOffset>.Serialize(Span<Byte> buffer,
                                                          DateTimeOffset graph)
    {
        Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(buffer)) = graph.Ticks;
        return sizeof(Int64);
    }
}