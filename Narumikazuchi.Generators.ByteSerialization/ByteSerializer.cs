using Narumikazuchi.InputOutput;

namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides methods to serialize runtime objects into and from <see cref="Byte"/>[] arrays or streams.
/// </summary>
/// <remarks>
/// To make a runtime object serializable you can either decorate it with the <see cref="ByteSerializableAttribute"/>, in which case
/// the serialization code will be generated automatically, or write a <see cref="IByteSerializationStrategy{TSerializable}"/> for
/// the type.
/// </remarks>
static public class ByteSerializer
{
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <returns>The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</returns>
    static public TSerializable Deserialize<TSerializable>(ReadOnlySpan<Byte> buffer)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Int32 size = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer));
        return TSerializable.Deserialize(buffer: buffer[4..(4 + size)],
                                         read: out _);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="read">The number of bytes read from the buffer.</param>
    /// <returns>The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</returns>
    static public TSerializable Deserialize<TSerializable>(ReadOnlySpan<Byte> buffer,
                                                           out Int32 read)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Int32 size = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer));
        TSerializable result = TSerializable.Deserialize(buffer: buffer[4..(4 + size)],
                                                         read: out read);
        read += sizeof(Int32);
        return result;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified strategy <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="read">The number of bytes read from the buffer.</param>
    /// <returns>The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</returns>
    static public TSerializable Deserialize<TSerializable, TStrategy>(ReadOnlySpan<Byte> buffer,
                                                                      out Int32 read)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        TSerializable result = TStrategy.Deserialize(buffer: buffer,
                                                     read: out read);
        return result;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="read">The number of bytes read from the buffer.</param>
    /// <returns>The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Stream"/> contains.</returns>
    static public TSerializable Deserialize<TSerializable>(Stream stream,
                                                           out Int32 read)
        where TSerializable : IByteSerializable<TSerializable>
    {
        return Deserialize<ReadableStreamWrapper, TSerializable>(stream: stream,
                                                                 read: out read);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="read">The number of bytes read from the buffer.</param>
    /// <returns>The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Stream"/> contains.</returns>
    static public TSerializable Deserialize<TSerializable, TStrategy>(Stream stream,
                                                                      out Int32 read)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        return Deserialize<ReadableStreamWrapper, TSerializable, TStrategy>(stream: stream,
                                                                            read: out read);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="read">The number of bytes read from the buffer.</param>
    /// <returns>The runtime object of type <typeparamref name="TSerializable"/> that the <typeparamref name="TStream"/> contains.</returns>
    static public TSerializable Deserialize<TStream, TSerializable>(TStream stream,
                                                                    out Int32 read)
        where TStream : IReadableStream
        where TSerializable : IByteSerializable<TSerializable>
    {
        Byte[] buffer = new Byte[4];
        stream.Read(buffer);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size];
        stream.Read(buffer);
        TSerializable result = TSerializable.Deserialize(buffer: buffer,
                                                         read: out read);
        read += sizeof(Int32);
        return result;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="read">The number of bytes read from the buffer.</param>
    /// <returns>The runtime object of type <typeparamref name="TSerializable"/> that the <typeparamref name="TStream"/> contains.</returns>
    static public TSerializable Deserialize<TStream, TSerializable, TStrategy>(TStream stream,
                                                                               out Int32 read)
        where TStream : IReadableStream
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        Byte[] buffer = new Byte[4];
        stream.Read(buffer);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size];
        stream.Read(buffer);
        TSerializable result = TStrategy.Deserialize(buffer: buffer,
                                                     read: out read);
        read += sizeof(Int32);
        return result;
    }

    /// <summary>
    /// Calculates the expected size of the <see cref="Byte"/>[] array after serialization of the sepcified runtime object.
    /// </summary>
    /// <param name="graph">The runtime object to calculate the expected size of.</param>
    /// <returns>The expected size of the <see cref="Byte"/>[] array after serialization.</returns>
    static public Int32 GetExpectedSerializedSize<TSerializable>(TSerializable graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        return TSerializable.GetExpectedByteSize(graph);
    }
    /// <summary>
    /// Calculates the expected size of the <see cref="Byte"/>[] array after serialization of the sepcified runtime object using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="graph">The runtime object to calculate the expected size of.</param>
    /// <returns>The expected size of the <see cref="Byte"/>[] array after serialization.</returns>
    static public Int32 GetExpectedSerializedSize<TSerializable, TStrategy>(TSerializable graph)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        return TStrategy.GetExpectedByteSize(graph);
    }

    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The <see cref="Byte"/>-representation of the specified runtime object.</returns>
    static public Byte[] Serialize<TSerializable>(TSerializable graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Byte[] buffer = new Byte[TSerializable.GetExpectedByteSize(graph)];
        Int32 written = TSerializable.Serialize(buffer: buffer.AsSpan()[4..],
                                                value: graph);
        Unsafe.As<Byte, Int32>(ref buffer[0]) = written;
        return buffer;
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="buffer">The buffer into which the <see cref="Byte"/>-representation of the specified runtime object will be stored.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable>(Span<Byte> buffer,
                                                 TSerializable graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Int32 written = TSerializable.Serialize(buffer: buffer[4..],
                                                value: graph);
        Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer)) = written;
        return sizeof(Int32) + written;
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="buffer">The buffer into which the <see cref="Byte"/>-representation of the specified runtime object will be stored.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable, TStrategy>(Span<Byte> buffer,
                                                            TSerializable graph)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        Int32 written = TStrategy.Serialize(buffer: buffer,
                                            value: graph);
        return written;
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable>(Stream stream,
                                                 TSerializable graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        return Serialize(stream: stream.AsWriteableStream(),
                         graph: graph);
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable, TStrategy>(Stream stream,
                                                            TSerializable graph)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        return Serialize<WriteableStreamWrapper, TSerializable, TStrategy>(stream: stream.AsWriteableStream(),
                                                                           graph: graph);
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TStream, TSerializable>(TStream stream,
                                                          TSerializable graph)
        where TStream : IWriteableStream
        where TSerializable : IByteSerializable<TSerializable>
    {
        Byte[] buffer = new Byte[TSerializable.GetExpectedByteSize(graph)];
        Int32 written = TSerializable.Serialize(buffer: buffer.AsSpan()[4..],
                                                value: graph);
        Unsafe.As<Byte, Int32>(ref buffer[0]) = written;
        stream.Write(buffer);
        return sizeof(Int32) + written;
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TStream, TSerializable, TStrategy>(TStream stream,
                                                                     TSerializable graph)
        where TStream : IWriteableStream
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        Byte[] buffer = new Byte[TStrategy.GetExpectedByteSize(graph)];
        Int32 written = TStrategy.Serialize(buffer: buffer.AsSpan()[4..],
                                            value: graph);
        Unsafe.As<Byte, Int32>(ref buffer[0]) = written;
        stream.Write(buffer);
        return sizeof(Int32) + written;
    }
}