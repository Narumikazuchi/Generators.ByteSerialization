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
    static public Byte[] Serialize<TSerializable>(TSerializable? graph)
    {
        if (graph is null)
        {
            Byte[] buffer = new Byte[20];
            Unsafe.As<Byte, Int32>(ref buffer[0]) = 0;
            Unsafe.As<Byte, Guid>(ref buffer[4]) = typeof(TSerializable).GUID;
            return buffer;
        }
        else
        {
            if (graph.GetType()
                     .IsUnmanagedStruct())
            {
                Byte[] buffer = new Byte[Unsafe.SizeOf<TSerializable>()];
                Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph;
                return buffer;
            }
            else if (Handlers is ISerializationHandler<TSerializable> handler)
            {
                Byte[] buffer = ArrayPool<Byte>.Shared.Rent(handler.GetExpectedSize(graph) + 20);
                Unsafe.As<Byte, Guid>(ref buffer[4]) = TypeCache.GetIdOfType<TSerializable>();
                Int32 written = handler.Serialize(buffer: buffer.AsSpan()[20..],
                                                  graph: graph);
                Unsafe.As<Byte, Int32>(ref buffer[0]) = written + 16;
                written += 20;
                if (written < buffer.Length)
                {
                    Byte[] result = buffer[..written];
                    ArrayPool<Byte>.Shared.Return(buffer);
                    return result;
                }
                else
                {
                    Byte[] result = new Byte[buffer.Length];
                    Array.Copy(sourceArray: buffer,
                               destinationArray: result,
                               length: buffer.Length);
                    ArrayPool<Byte>.Shared.Return(buffer);
                    return result;
                }
            }
            else
            {
                throw new TypeNotSerializable(graph.GetType());
            }
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
    static public Int32 Serialize<TSerializable>(Span<Byte> buffer,
                                                 TSerializable graph)
    {
        if (graph is null)
        {
            Unsafe.As<Byte, Int32>(ref buffer[0]) = 16;
            Unsafe.As<Byte, Guid>(ref buffer[4]) = TypeCache.GetIdOfType<TSerializable>();
            return 20;
        }
        else
        {
            if (graph.GetType()
                     .IsUnmanagedStruct())
            {
                Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph;
                return Unsafe.SizeOf<TSerializable>();
            }
            else if (Handlers is ISerializationHandler<TSerializable> handler)
            {
                Unsafe.As<Byte, Guid>(ref buffer[4]) = TypeCache.GetIdOfType<TSerializable>();
                Int32 written = handler.Serialize(buffer: buffer[20..],
                                                  graph: graph);
                Unsafe.As<Byte, Int32>(ref buffer[0]) = written + 16;
                return written + 20;
            }
            else
            {
                throw new TypeNotSerializable(graph.GetType());
            }
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
    static public unsafe Int32 Serialize<TSerializable>(Byte* buffer,
                                                        TSerializable graph)
    {
        if (graph is null)
        {
            *(Int32*)buffer = 16;
            *(Guid*)(buffer + 4) = TypeCache.GetIdOfType<TSerializable>();
            return 20;
        }
        else
        {
            if (graph.GetType()
                     .IsUnmanagedStruct())
            {
                Unsafe.As<Byte, TSerializable>(ref Unsafe.AsRef<Byte>(buffer)) = graph;
                return Unsafe.SizeOf<TSerializable>();
            }
            else if (Handlers is ISerializationHandler<TSerializable> handler)
            {
                *(Guid*)(buffer + 4) = TypeCache.GetIdOfType<TSerializable>();
                Int32 expectedSize = GetExpectedSerializedSize(graph);
                Int32 written = handler.Serialize(buffer: new Span<Byte>(buffer + 20, expectedSize),
                                                  graph: graph);
                *(Int32*)buffer = written + 16;
                return written + 20;
            }
            else
            {
                throw new TypeNotSerializable(graph.GetType());
            }
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
    static public Int32 Serialize<TSerializable>(Stream stream,
                                                 TSerializable graph)
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
    static public Int32 Serialize<TStream, TSerializable>(TStream stream,
                                                          TSerializable graph)
        where TStream : IWriteableStream
    {
        Byte[] buffer = Serialize(graph);
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
    static public Task<Int32> SerializeAsynchronously<TSerializable>(Stream stream,
                                                                     TSerializable graph,
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
    static public async Task<Int32> SerializeAsynchronously<TStream, TSerializable>(TStream stream,
                                                                                    TSerializable graph,
                                                                                    CancellationToken cancellationToken = default)
        where TStream : IWriteableStream
    {
        Byte[] buffer = Serialize(graph);
        await stream.WriteAsynchronously(buffer: buffer,
                                         cancellationToken: cancellationToken);
        return buffer.Length;
    }
}