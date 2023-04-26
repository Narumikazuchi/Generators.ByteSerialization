namespace Narumikazuchi.Generators.ByteSerialization;

public unsafe partial interface IByteSerializer : ISerializationHandler<String>
{
    UInt32 ISerializationHandler<String>.Deserialize(Byte* buffer,
                                                     out String? result)
    {
        Int32 size = *(Int32*)buffer;
        result = null;
        if ((size & 0x80000000) == 0x80000000)
        {
            Span<Byte> source = new(buffer + 4, (Int32)(size & 0x0FFFFFFF));
            result = new String(MemoryMarshal.Cast<Byte, Char>(source));
            return (UInt32)(source.Length + sizeof(Int32));
        }
        else
        {
            return sizeof(Int32);
        }
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

    UInt32 ISerializationHandler<String>.Serialize(Byte* buffer,
                                                   String? graph)
    {
        if (graph is null)
        {
            *(Int32*)buffer = 0;
            return sizeof(Int32);
        }
        else
        {
            ReadOnlySpan<Byte> bytes = MemoryMarshal.AsBytes(graph.AsSpan());
            Span<Byte> destination = new(buffer + sizeof(Int32), bytes.Length);
            bytes.CopyTo(destination);
            *(UInt32*)buffer = (UInt32)bytes.Length | 0x80000000;
            return (UInt32)destination.Length + sizeof(Int32);
        }
    }

    TypeIdentifier ISerializationHandler<String>.TypeIdentifier
    {
        get
        {
            return s_Identifier;
        }
    }

    static private readonly TypeIdentifier s_Identifier = TypeIdentifier.CreateFrom(typeof(String));
}