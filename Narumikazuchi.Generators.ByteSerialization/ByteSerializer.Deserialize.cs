using Narumikazuchi.InputOutput;

namespace Narumikazuchi.Generators.ByteSerialization;

public partial class ByteSerializer
{
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable>(ReadOnlySpan<Byte> buffer,
                                                   out TSerializable result)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Int32 size = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer));
        result = TSerializable.Deserialize(buffer: buffer[4..(4 + size)],
                                           read: out Int32 read);
        read += sizeof(Int32);
        return read;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="result">The runtime collection of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable>(ReadOnlySpan<Byte> buffer,
                                                   out ImmutableArray<TSerializable> result)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Int32 size = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer));
        ReadOnlySpan<Byte> collectionBuffer = buffer[4..(4 + size)];
        Int32 elements = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(collectionBuffer));
        ImmutableArray<TSerializable>.Builder builder = ImmutableArray.CreateBuilder<TSerializable>(elements);
        Int32 read = 2 * sizeof(Int32);
        Int32 pointer = sizeof(Int32);
        for (Int32 index = 0;
             index < elements;
             index++)
        {
            TSerializable element = TSerializable.Deserialize(buffer: collectionBuffer[pointer..],
                                                              read: out Int32 bytesRead);
            read += bytesRead;
            pointer += bytesRead;
            builder.Add(element);
        }

        result = builder.ToImmutable();
        return read;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified strategy <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable, TStrategy>(ReadOnlySpan<Byte> buffer,
                                                              out TSerializable result)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        result = TStrategy.Deserialize(buffer: buffer,
                                       read: out Int32 read);
        return read;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified strategy <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="result">The runtime collection of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable, TStrategy>(ReadOnlySpan<Byte> buffer,
                                                              out ImmutableArray<TSerializable> result)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        Int32 size = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer));
        ReadOnlySpan<Byte> collectionBuffer = buffer[4..(4 + size)];
        Int32 elements = Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(collectionBuffer));
        ImmutableArray<TSerializable>.Builder builder = ImmutableArray.CreateBuilder<TSerializable>(elements);
        Int32 read = 2 * sizeof(Int32);
        Int32 pointer = sizeof(Int32);
        for (Int32 index = 0;
             index < elements;
             index++)
        {
            TSerializable element = TStrategy.Deserialize(buffer: collectionBuffer[pointer..],
                                                          read: out Int32 bytesRead);
            read += bytesRead;
            pointer += bytesRead;
            builder.Add(element);
        }

        result = builder.ToImmutable();
        return read;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Stream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable>(Stream stream,
                                                   out TSerializable result)
        where TSerializable : IByteSerializable<TSerializable>
    {
        return Deserialize<ReadableStreamWrapper, TSerializable>(stream: stream,
                                                                 result: out result);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Stream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable>(Stream stream,
                                                   out ImmutableArray<TSerializable> result)
        where TSerializable : IByteSerializable<TSerializable>
    {
        return Deserialize<ReadableStreamWrapper, TSerializable>(stream: stream,
                                                                 result: out result);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Stream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable, TStrategy>(Stream stream,
                                                              out TSerializable result)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        return Deserialize<ReadableStreamWrapper, TSerializable, TStrategy>(stream: stream,
                                                                            result: out result);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Stream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TSerializable, TStrategy>(Stream stream,
                                                              out ImmutableArray<TSerializable> result)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        return Deserialize<ReadableStreamWrapper, TSerializable, TStrategy>(stream: stream,
                                                                            result: out result);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <typeparamref name="TStream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TStream, TSerializable>(TStream stream,
                                                            out TSerializable result)
        where TStream : IReadableStream
        where TSerializable : IByteSerializable<TSerializable>
    {
        Byte[] buffer = new Byte[4];
        stream.Read(buffer);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size];
        stream.Read(buffer);
        result = TSerializable.Deserialize(buffer: buffer,
                                           read: out Int32 read);
        read += sizeof(Int32);
        return read;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <typeparamref name="TStream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TStream, TSerializable>(TStream stream,
                                                            out ImmutableArray<TSerializable> result)
        where TStream : IReadableStream
        where TSerializable : IByteSerializable<TSerializable>
    {
        Byte[] buffer = new Byte[4];
        stream.Read(buffer);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size];
        stream.Read(buffer);

        Int32 elements = Unsafe.ReadUnaligned<Int32>(ref buffer[0]);
        ImmutableArray<TSerializable>.Builder builder = ImmutableArray.CreateBuilder<TSerializable>(elements);
        Int32 read = 2 * sizeof(Int32);
        Int32 pointer = sizeof(Int32);
        for (Int32 index = 0;
             index < elements;
             index++)
        {
            TSerializable element = TSerializable.Deserialize(buffer: buffer.AsSpan()[pointer..],
                                                              read: out Int32 bytesRead);
            read += bytesRead;
            pointer += bytesRead;
            builder.Add(element);
        }

        result = builder.ToImmutable();
        return read;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <typeparamref name="TStream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TStream, TSerializable, TStrategy>(TStream stream,
                                                                       out TSerializable result)
        where TStream : IReadableStream
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        Byte[] buffer = new Byte[4];
        stream.Read(buffer);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size];
        stream.Read(buffer);
        result = TStrategy.Deserialize(buffer: buffer,
                                       read: out Int32 read);
        read += sizeof(Int32);
        return read;
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/> using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <typeparamref name="TStream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    static public Int32 Deserialize<TStream, TSerializable, TStrategy>(TStream stream,
                                                                       out ImmutableArray<TSerializable> result)
        where TStream : IReadableStream
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        Byte[] buffer = new Byte[4];
        stream.Read(buffer);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size];
        stream.Read(buffer);

        Int32 elements = Unsafe.ReadUnaligned<Int32>(ref buffer[0]);
        ImmutableArray<TSerializable>.Builder builder = ImmutableArray.CreateBuilder<TSerializable>(elements);
        Int32 read = 2 * sizeof(Int32);
        Int32 pointer = sizeof(Int32);
        for (Int32 index = 0;
             index < elements;
             index++)
        {
            TSerializable element = TStrategy.Deserialize(buffer: buffer.AsSpan()[pointer..],
                                                          read: out Int32 bytesRead);
            read += bytesRead;
            pointer += bytesRead;
            builder.Add(element);
        }

        result = builder.ToImmutable();
        return read;
    }
}