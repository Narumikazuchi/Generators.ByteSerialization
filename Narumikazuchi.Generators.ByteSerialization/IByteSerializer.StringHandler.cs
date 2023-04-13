namespace Narumikazuchi.Generators.ByteSerialization;

public partial interface IByteSerializer : ISerializationHandler<String>
{
    Int32 ISerializationHandler<String>.Deserialize(ReadOnlySpan<Byte> buffer,
                                                    out String? result)
    {
        result = new String(MemoryMarshal.Cast<Byte, Char>(buffer));
        return buffer.Length;
    }

    Int32 ISerializationHandler<String>.GetExpectedSize(String? graph)
    {
        if (graph is null)
        {
            return 0;
        }
        else
        {
            return 4 * graph.Length;
        }
    }

    Int32 ISerializationHandler<String>.Serialize(Span<Byte> buffer,
                                                  String? graph)
    {
        if (graph is null)
        {
            return 0;
        }
        else
        {
            ReadOnlySpan<Byte> bytes = MemoryMarshal.AsBytes(graph.AsSpan());
            bytes.CopyTo(buffer);
            return bytes.Length;
        }
    }
}