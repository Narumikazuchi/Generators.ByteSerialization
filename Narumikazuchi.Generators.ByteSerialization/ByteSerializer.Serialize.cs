using Narumikazuchi.InputOutput;

namespace Narumikazuchi.Generators.ByteSerialization;

public partial class ByteSerializer
{
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The <see cref="Byte"/>-representation of the specified runtime object.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public ReadOnlySpan<Byte> Serialize<TSerializable>(TSerializable? graph)
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            Byte[] buffer = new Byte[Unsafe.SizeOf<TSerializable>()];
            Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
            return buffer;
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Span<Byte> buffer = stackalloc Byte[GetExpectedSerializedSize(graph)];
            buffer[4] = 0x1;
            Int32 written = 5;
            written += (Int32)TypeLayoutSerializationHandler.Default.Serialize(buffer: buffer[written..],
                                                                               graph: TypeLayout.CreateFrom(typeof(TSerializable)));
            Unsafe.As<Byte, TSerializable>(ref buffer[written]) = graph!;
            written += Unsafe.SizeOf<TSerializable>();
            Unsafe.As<Byte, Int32>(ref buffer[0]) = written;
            return buffer[..written].ToArray();
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            Span<Byte> buffer = stackalloc Byte[handler.GetExpectedArraySize(graph)];
            Int32 written = (Int32)handler.Serialize(buffer: buffer,
                                                     graph: graph);
            return buffer[..written].ToArray();
        }
        else
        {
            throw new TypeNotSerializable(graph?.GetType() ?? typeof(TSerializable));
        }
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="buffer">The buffer into which the <see cref="Byte"/>-representation of the specified runtime object will be stored.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Serialize<TSerializable>(Span<Byte> buffer,
                                                                TSerializable? graph)
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            buffer[4] = 0x1;
            Int32 written = 5;
            written += (Int32)TypeLayoutSerializationHandler.Default.Serialize(buffer: buffer[written..],
                                                                               graph: TypeLayout.CreateFrom(typeof(TSerializable)));
            Unsafe.As<Byte, TSerializable>(ref buffer[written]) = graph!;
            written += Unsafe.SizeOf<TSerializable>();
            Unsafe.As<Byte, Int32>(ref buffer[0]) = written;
            return written;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            return handler.Serialize(buffer: buffer,
                                     graph: graph!);
        }
        else
        {
            throw new TypeNotSerializable(graph?.GetType() ?? typeof(TSerializable));
        }
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="buffer">The buffer into which the <see cref="Byte"/>-representation of the specified runtime object will be stored.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Serialize<TSerializable>(Memory<Byte> buffer,
                                                                TSerializable? graph)
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            Unsafe.As<Byte, TSerializable>(ref buffer.Span[0]) = graph!;
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            buffer.Span[4] = 0x1;
            Int32 written = 5;
            written += (Int32)TypeLayoutSerializationHandler.Default.Serialize(buffer: buffer.Span[written..],
                                                                               graph: TypeLayout.CreateFrom(typeof(TSerializable)));
            Unsafe.As<Byte, TSerializable>(ref buffer.Span[written]) = graph!;
            written += Unsafe.SizeOf<TSerializable>();
            Unsafe.As<Byte, Int32>(ref buffer.Span[0]) = written;
            return written;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            return handler.Serialize(buffer: buffer.Span,
                                     graph: graph!);
        }
        else
        {
            throw new TypeNotSerializable(graph?.GetType() ?? typeof(TSerializable));
        }
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="buffer">The buffer into which the <see cref="Byte"/>-representation of the specified runtime object will be stored.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public unsafe Unsigned31BitInteger Serialize<TSerializable>(Byte* buffer,
                                                                       TSerializable? graph)
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Span<Byte> wrapper = new(pointer: buffer,
                                     length: GetExpectedSerializedSize(graph));
            wrapper[4] = 0x1;
            Int32 written = 5;
            written += (Int32)TypeLayoutSerializationHandler.Default.Serialize(buffer: wrapper[written..],
                                                                               graph: TypeLayout.CreateFrom(typeof(TSerializable)));
            Unsafe.As<Byte, TSerializable>(ref wrapper[written]) = graph!;
            written += Unsafe.SizeOf<TSerializable>();
            Unsafe.As<Byte, Int32>(ref wrapper[0]) = written;
            return written;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            Int32 size = handler.GetExpectedArraySize(graph);
            return handler.Serialize(buffer: new Span<Byte>(buffer, size),
                                     graph: graph);
        }
        else
        {
            throw new TypeNotSerializable(graph?.GetType() ?? typeof(TSerializable));
        }
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Serialize<TSerializable>(Stream stream,
                                                                TSerializable? graph)
    {
        return Serialize(stream: stream.AsWriteableStream(),
                         graph: graph);
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Unsigned31BitInteger Serialize<TStream, TSerializable>(TStream stream,
                                                                         TSerializable? graph)
        where TStream : IWriteableStream
    {
        ReadOnlySpan<Byte> buffer = Serialize(graph);
        stream.Write(buffer);
        return buffer.Length;
    }

    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Task<Unsigned31BitInteger> SerializeAsynchronously<TSerializable>(Stream stream,
                                                                                    TSerializable? graph,
                                                                                    CancellationToken cancellationToken = default)
    {
        return SerializeAsynchronously(stream: stream.AsWriteableStream(),
                                       graph: graph,
                                       cancellationToken: cancellationToken);
    }
    /// <summary>
    /// Serializes the specified runtime object into it's raw <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="stream">The stream into which the <see cref="Byte"/>-representation of the specified runtime object will be written.</param>
    /// <param name="graph">The runtime object to serialize.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>The amount bytes written to the buffer.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public async Task<Unsigned31BitInteger> SerializeAsynchronously<TStream, TSerializable>(TStream stream,
                                                                                                   TSerializable? graph,
                                                                                                   CancellationToken cancellationToken = default)
        where TStream : IWriteableStream
    {
        if (typeof(TSerializable).IsUnmanagedSerializable())
        {
            Byte[] buffer = new Byte[Unsafe.SizeOf<TSerializable>()];
            Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
            await stream.WriteAsynchronously(buffer: buffer,
                                             cancellationToken: cancellationToken);
            return buffer.Length;
        }
        else if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Memory<Byte> buffer = ArrayPool<Byte>.Shared.Rent(GetExpectedSerializedSize(graph));
            Int32 written = (Int32)Serialize(buffer: buffer,
                                             graph: graph);
            await stream.WriteAsynchronously(buffer: buffer[..written],
                                             cancellationToken: cancellationToken);
            return written;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            Byte[] buffer = ArrayPool<Byte>.Shared.Rent(handler.GetExpectedArraySize(graph));
            Int32 written = (Int32)handler.Serialize(buffer: buffer,
                                                     graph: graph);
            await stream.WriteAsynchronously(buffer: buffer.AsMemory()[..written],
                                             cancellationToken: cancellationToken);
            return written;
        }
        else
        {
            throw new TypeNotSerializable(graph?.GetType() ?? typeof(TSerializable));
        }
    }
}