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
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Deserialize<TSerializable>(ReadOnlySpan<Byte> buffer,
                                                                  out TSerializable? result)
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            result = Unsafe.As<Byte, TSerializable>(ref MemoryMarshal.GetReference(buffer));
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Int32 size = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer));
            if (buffer.Length < size)
            {
                throw new AccessViolationException("Buffer does not match recorded size.");
            }

            Int32 read = sizeof(Int32);
            if (buffer[read++] == 0x0)
            {
                throw new NullReferenceException("The object serialized in the buffer was marked as null.");
            }

            read += (Int32)TypeLayoutSerializationHandler.Default.Deserialize(buffer: buffer[read..],
                                                                              result: out TypeLayout layout);
            if (layout != TypeLayout.CreateFrom(typeof(TSerializable)))
            {
                throw new WrongTypeDeserialization(typeof(TSerializable));
            }

            result = Unsafe.As<Byte, TSerializable>(ref MemoryMarshal.GetReference(buffer[read..]));
            return read;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            return handler.Deserialize(buffer: buffer,
                                       result: out result);
        }
        else
        {
            throw new TypeNotSerializable(typeof(TSerializable));
        }
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Deserialize<TSerializable>(ReadOnlyMemory<Byte> buffer,
                                                                  out TSerializable? result)
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            result = Unsafe.As<Byte, TSerializable>(ref MemoryMarshal.GetReference(buffer.Span));
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Int32 size = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer.Span));
            if (buffer.Length < size)
            {
                throw new AccessViolationException("Buffer does not match recorded size.");
            }

            Int32 read = sizeof(Int32);
            if (buffer.Span[read++] == 0x0)
            {
                throw new NullReferenceException("The object serialized in the buffer was marked as null.");
            }

            read += (Int32)TypeLayoutSerializationHandler.Default.Deserialize(buffer: buffer.Span[read..],
                                                                              result: out TypeLayout layout);
            if (layout != TypeLayout.CreateFrom(typeof(TSerializable)))
            {
                throw new WrongTypeDeserialization(typeof(TSerializable));
            }

            result = Unsafe.As<Byte, TSerializable>(ref MemoryMarshal.GetReference(buffer.Span[read..]));
            return read;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            return handler.Deserialize(buffer: buffer.Span,
                                       result: out result);
        }
        else
        {
            throw new TypeNotSerializable(typeof(TSerializable));
        }
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="Byte"/>-representation of the object.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Byte"/>[] array represents.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public unsafe Unsigned31BitInteger Deserialize<TSerializable>(Byte* buffer,
                                                                         out TSerializable? result)
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            result = Unsafe.As<Byte, TSerializable>(ref Unsafe.AsRef<Byte>(buffer));
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Int32 size = *(Int32*)buffer;
            Span<Byte> wrapper = new(pointer: buffer,
                                     length: size);

            Int32 read = sizeof(Int32);
            if (wrapper[read++] == 0x0)
            {
                throw new NullReferenceException("The object serialized in the buffer was marked as null.");
            }

            read += (Int32)TypeLayoutSerializationHandler.Default.Deserialize(buffer: wrapper[read..],
                                                                              result: out TypeLayout layout);
            if (layout != TypeLayout.CreateFrom(typeof(TSerializable)))
            {
                throw new WrongTypeDeserialization(typeof(TSerializable));
            }

            result = Unsafe.As<Byte, TSerializable>(ref wrapper[read]);
            return read;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            Int32 size = *(Int32*)buffer;
            return handler.Deserialize(buffer: new ReadOnlySpan<Byte>(buffer, size),
                                       result: out result);
        }
        else
        {
            throw new TypeNotSerializable(typeof(TSerializable));
        }
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <see cref="Stream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Deserialize<TSerializable>(Stream stream,
                                                                  out TSerializable? result)
    {
        return Deserialize<ReadableStreamWrapper, TSerializable?>(stream: stream,
                                                                  result: out result);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="result">The runtime object of type <typeparamref name="TSerializable"/> that the <typeparamref name="TStream"/> contains.</param>
    /// <returns>The number of bytes read from the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Deserialize<TStream, TSerializable>(TStream stream,
                                                                           out TSerializable? result)
        where TStream : IReadableStream
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            Byte[] buffer = new Byte[Unsafe.SizeOf<TSerializable>()];
            stream.Read(buffer);
            result = Unsafe.As<Byte, TSerializable>(ref buffer[0]);
            return Unsafe.SizeOf<TSerializable>();
        }
        else
        {
            Span<Byte> buffer = stackalloc Byte[4];
            stream.Read(buffer);
            Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
            buffer = stackalloc Byte[size];
            Unsafe.As<Byte, Int32>(ref buffer[0]) = size;
            stream.Read(buffer[4..]);
            return Deserialize(buffer: buffer,
                               result: out result);
        }
    }

    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>The result of the deserialization.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Task<AsynchronousDeserializationResult<TSerializable?>> DeserializeAsynchronously<TSerializable>(Stream stream,
                                                                                                                   CancellationToken cancellationToken = default)
    {
        return DeserializeAsynchronously<ReadableStreamWrapper, TSerializable?>(stream: stream,
                                                                                cancellationToken: cancellationToken);
    }
    /// <summary>
    /// Deserializes the object from it's <see cref="Byte"/>-representation back into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="stream">The stream containing the raw <see cref="Byte"/>[] array.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>The result of the deserialization.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public async Task<AsynchronousDeserializationResult<TSerializable?>> DeserializeAsynchronously<TStream, TSerializable>(TStream stream,
                                                                                                                                  CancellationToken cancellationToken = default)
        where TStream : IReadableStream
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            Memory<Byte> buffer = new Byte[Unsafe.SizeOf<TSerializable>()];
            await stream.ReadAsynchronously(buffer: buffer,
                                            cancellationToken: cancellationToken);
            Unsigned31BitInteger read = Deserialize(buffer: buffer,
                                                    result: out TSerializable? result);
            return new AsynchronousDeserializationResult<TSerializable?>
            {
                BytesRead = read,
                Result = result
            };
        }
        else
        {
            Memory<Byte> buffer = new Byte[4];
            await stream.ReadAsynchronously(buffer: buffer,
                                            cancellationToken: cancellationToken);
            Int32 size = Unsafe.As<Byte, Int32>(ref buffer.Span[0]);
            buffer = new Byte[size];
            Unsafe.As<Byte, Int32>(ref buffer.Span[0]) = size;
            await stream.ReadAsynchronously(buffer: buffer[4..],
                                            cancellationToken: cancellationToken);
            Unsigned31BitInteger read = Deserialize(buffer: buffer,
                                                    result: out TSerializable? result);
            return new AsynchronousDeserializationResult<TSerializable?>
            {
                BytesRead = read,
                Result = result
            };
        }
    }
}