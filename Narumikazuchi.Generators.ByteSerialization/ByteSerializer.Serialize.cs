using Narumikazuchi.InputOutput;
using System.Reflection;

namespace Narumikazuchi.Generators.ByteSerialization;

public partial class ByteSerializer
{
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
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The <see cref="Byte"/>-representation of the specified runtime object.</returns>
    static public Byte[] Serialize<TSerializable>(IEnumerable<TSerializable> graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Byte[] buffer = new Byte[GetExpectedSerializedSize(graph)];
        if (graph is TSerializable[] array)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = array.Length;
        }
        else if (graph is ImmutableArray<TSerializable> immutableArray)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = immutableArray.Length;
        }
        else if (graph is ICollection<TSerializable> collection)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = collection.Count;
        }
        else if (graph is IReadOnlyCollection<TSerializable> readOnlyCollection)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = readOnlyCollection.Count;
        }
        else
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = graph.Count();
        }

        Int32 pointer = 2 * sizeof(Int32);
        foreach (TSerializable element in graph)
        {
            pointer += TSerializable.Serialize(buffer: buffer.AsSpan()[pointer..],
                                               value: element);
        }

        Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer - sizeof(Int32);
        if (pointer < buffer.Length)
        {
            return buffer[..pointer];
        }
        else
        {
            return buffer;
        }
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
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="buffer">The buffer into which the <see cref="Byte"/>-representation of the specified runtime object will be stored.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable>(Span<Byte> buffer,
                                                 IEnumerable<TSerializable> graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        if (graph is TSerializable[] array)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = array.Length;
        }
        else if (graph is ImmutableArray<TSerializable> immutableArray)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = immutableArray.Length;
        }
        else if (graph is ICollection<TSerializable> collection)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = collection.Count;
        }
        else if (graph is IReadOnlyCollection<TSerializable> readOnlyCollection)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = readOnlyCollection.Count;
        }
        else
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = graph.Count();
        }

        Int32 pointer = 2 * sizeof(Int32);
        foreach (TSerializable element in graph)
        {
            pointer += TSerializable.Serialize(buffer: buffer[pointer..],
                                               value: element);
        }

        Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer - sizeof(Int32);
        return pointer;
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
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="buffer">The buffer into which the <see cref="Byte"/>-representation of the specified runtime object will be stored.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable, TStrategy>(Span<Byte> buffer,
                                                            IEnumerable<TSerializable> graph)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        if (graph is TSerializable[] array)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = array.Length;
        }
        else if (graph is ImmutableArray<TSerializable> immutableArray)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = immutableArray.Length;
        }
        else if (graph is ICollection<TSerializable> collection)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = collection.Count;
        }
        else if (graph is IReadOnlyCollection<TSerializable> readOnlyCollection)
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = readOnlyCollection.Count;
        }
        else
        {
            Unsafe.As<Byte, Int32>(ref buffer[4]) = graph.Count();
        }

        Int32 pointer = 2 * sizeof(Int32);
        foreach (TSerializable element in graph)
        {
            pointer += TStrategy.Serialize(buffer: buffer[pointer..],
                                           value: element);
        }

        Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer - sizeof(Int32);
        return pointer;
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
        if (stream.CanSeek)
        {
            return Serialize(stream: new __StreamWrapper(stream),
                             graph: graph);
        }
        else
        {
            return Serialize(stream: stream.AsWriteableStream(),
                             graph: graph);
        }
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable>(Stream stream,
                                                 IEnumerable<TSerializable> graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        if (stream.CanSeek)
        {
            return Serialize(stream: new __StreamWrapper(stream),
                             graph: graph);
        }
        else
        {
            return Serialize(stream: stream.AsWriteableStream(),
                             graph: graph);
        }
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
        if (stream.CanSeek)
        {
            return Serialize<__StreamWrapper, TSerializable, TStrategy>(stream: new __StreamWrapper(stream),
                                                                        graph: graph);
        }
        else
        {
            return Serialize<WriteableStreamWrapper, TSerializable, TStrategy>(stream: stream.AsWriteableStream(),
                                                                               graph: graph);
        }
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TSerializable, TStrategy>(Stream stream,
                                                            IEnumerable<TSerializable> graph)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        if (stream.CanSeek)
        {
            return Serialize<__StreamWrapper, TSerializable, TStrategy>(stream: new __StreamWrapper(stream),
                                                                        graph: graph);
        }
        else
        {
            return Serialize<WriteableStreamWrapper, TSerializable, TStrategy>(stream: stream.AsWriteableStream(),
                                                                               graph: graph);
        }
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
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TStream, TSerializable>(TStream stream,
                                                          IEnumerable<TSerializable> graph)
        where TStream : IWriteableStream
        where TSerializable : IByteSerializable<TSerializable>
    {
        if (stream is ISeekableStream seekable)
        {
            Int64 start = seekable.Position;
            Byte[] buffer = new Byte[4];
            if (graph is TSerializable[] array)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = array.Length;
            }
            else if (graph is ImmutableArray<TSerializable> immutableArray)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = immutableArray.Length;
            }
            else if (graph is ICollection<TSerializable> collection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = collection.Count;
            }
            else if (graph is IReadOnlyCollection<TSerializable> readOnlyCollection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = readOnlyCollection.Count;
            }
            else
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = graph.Count();
            }

            seekable.Position = start + sizeof(Int32);
            stream.Write(buffer);

            foreach (TSerializable element in graph)
            {
                buffer = new Byte[TSerializable.GetExpectedByteSize(element)];
                Int32 used = TSerializable.Serialize(buffer: buffer.AsSpan(),
                                                     value: element);
                stream.Write(buffer.AsSpan()[..used]);
            }

            buffer = new Byte[4];
            Int32 written = (Int32)(seekable.Position - start - sizeof(Int32));
            Unsafe.As<Byte, Int32>(ref buffer[0]) = written;
            seekable.Position = start;
            stream.Write(buffer);
            return sizeof(Int32) + written;
        }
        else
        {
            Byte[] buffer = new Byte[GetExpectedSerializedSize(graph)];
            if (graph is TSerializable[] array)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = array.Length;
            }
            else if (graph is ImmutableArray<TSerializable> immutableArray)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = immutableArray.Length;
            }
            else if (graph is ICollection<TSerializable> collection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = collection.Count;
            }
            else if (graph is IReadOnlyCollection<TSerializable> readOnlyCollection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = readOnlyCollection.Count;
            }
            else
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = graph.Count();
            }

            Int32 pointer = 2 * sizeof(Int32);
            foreach (TSerializable element in graph)
            {
                pointer += TSerializable.Serialize(buffer: buffer.AsSpan()[pointer..],
                                                   value: element);
            }

            Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer - sizeof(Int32);
            stream.Write(buffer.AsSpan()[..pointer]);
            return pointer;
        }
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
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    static public Int32 Serialize<TStream, TSerializable, TStrategy>(TStream stream,
                                                                     IEnumerable<TSerializable> graph)
        where TStream : IWriteableStream
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        if (stream is ISeekableStream seekable)
        {
            Int64 start = seekable.Position;
            Byte[] buffer = new Byte[4];
            if (graph is TSerializable[] array)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = array.Length;
            }
            else if (graph is ImmutableArray<TSerializable> immutableArray)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = immutableArray.Length;
            }
            else if (graph is ICollection<TSerializable> collection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = collection.Count;
            }
            else if (graph is IReadOnlyCollection<TSerializable> readOnlyCollection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = readOnlyCollection.Count;
            }
            else
            {
                Unsafe.As<Byte, Int32>(ref buffer[0]) = graph.Count();
            }

            seekable.Position = start + sizeof(Int32);
            stream.Write(buffer);

            foreach (TSerializable element in graph)
            {
                buffer = new Byte[TStrategy.GetExpectedByteSize(element)];
                Int32 used = TStrategy.Serialize(buffer: buffer.AsSpan(),
                                                 value: element);
                stream.Write(buffer.AsSpan()[..used]);
            }

            buffer = new Byte[4];
            Int32 written = (Int32)(seekable.Position - start - sizeof(Int32));
            Unsafe.As<Byte, Int32>(ref buffer[0]) = written;
            seekable.Position = start;
            stream.Write(buffer);
            return sizeof(Int32) + written;
        }
        else
        {
            Byte[] buffer = new Byte[GetExpectedSerializedSize<TSerializable, TStrategy>(graph)];
            if (graph is TSerializable[] array)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = array.Length;
            }
            else if (graph is ImmutableArray<TSerializable> immutableArray)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = immutableArray.Length;
            }
            else if (graph is ICollection<TSerializable> collection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = collection.Count;
            }
            else if (graph is IReadOnlyCollection<TSerializable> readOnlyCollection)
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = readOnlyCollection.Count;
            }
            else
            {
                Unsafe.As<Byte, Int32>(ref buffer[4]) = graph.Count();
            }

            Int32 pointer = 2 * sizeof(Int32);
            foreach (TSerializable element in graph)
            {
                pointer += TStrategy.Serialize(buffer: buffer.AsSpan()[pointer..],
                                               value: element);
            }

            Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer - sizeof(Int32);
            stream.Write(buffer.AsSpan()[..pointer]);
            return pointer;
        }
    }
}