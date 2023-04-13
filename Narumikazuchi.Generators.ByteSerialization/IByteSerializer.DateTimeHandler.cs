namespace Narumikazuchi.Generators.ByteSerialization;

public partial interface IByteSerializer : ISerializationHandler<DateTime>
{
    Int32 ISerializationHandler<DateTime>.Deserialize(ReadOnlySpan<Byte> buffer,
                                                      out DateTime result)
    {
        result = new(Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer)));
        return sizeof(Int64);
    }

    Int32 ISerializationHandler<DateTime>.GetExpectedSize(DateTime graph)
    {
        return sizeof(Int64);
    }

    Int32 ISerializationHandler<DateTime>.Serialize(Span<Byte> buffer,
                                                    DateTime graph)
    {
        Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(buffer)) = graph.Ticks;
        return sizeof(Int64);
    }
}