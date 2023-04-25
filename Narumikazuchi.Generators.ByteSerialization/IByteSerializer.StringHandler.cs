namespace Narumikazuchi.Generators.ByteSerialization;

public unsafe partial interface IByteSerializer : ISerializationHandler<String>
{
    Int32 ISerializationHandler<String>.Deserialize(Byte* buffer,
                                                    out String result)
    {
        Int32 size = *(Int32*)buffer;
        Span<Byte> source = new(buffer + 4, size);
        result = new String(MemoryMarshal.Cast<Byte, Char>(source));
        return source.Length + sizeof(Int32);
    }

    Int32 ISerializationHandler<String>.GetExpectedArraySize(String? graph)
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

    Int32 ISerializationHandler<String>.Serialize(Byte* buffer,
                                                  String? graph)
    {
        if (graph is null)
        {
            return 0;
        }
        else
        {
            ReadOnlySpan<Byte> bytes = MemoryMarshal.AsBytes(graph.AsSpan());
            Span<Byte> destination = new(buffer, bytes.Length + sizeof(Int32));
            bytes.CopyTo(destination[4..]);
            *(Int32*)buffer = bytes.Length;
            return destination.Length;
        }
    }

    TypeIdentifier ISerializationHandler<String>.TypeIdentifier
    {
        get
        {
            return s_Identifier;
        }
    }

    static private readonly TypeIdentifier s_Identifier = new(typeof(String));
}