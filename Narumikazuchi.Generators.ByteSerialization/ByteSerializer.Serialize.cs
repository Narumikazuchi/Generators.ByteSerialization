using Narumikazuchi.InputOutput;

namespace Narumikazuchi.Generators.ByteSerialization;

#pragma warning disable CS8500

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
        if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Byte[] buffer = new Byte[Unsafe.SizeOf<TSerializable>()];
            Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
            return buffer;
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            Byte[] buffer = ArrayPool<Byte>.Shared.Rent(handler.GetExpectedArraySize(graph));
            UInt32 written = handler.Serialize(buffer: buffer,
                                               graph: graph);
            Byte[] result = new Byte[written];
            Array.Copy(sourceArray: buffer,
                       destinationArray: result,
                       length: written);
            ArrayPool<Byte>.Shared.Return(buffer);
            return result;
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
    static public UInt32 Serialize<TSerializable>(Span<Byte> buffer,
                                                  TSerializable? graph)
    {
        if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
            return (UInt32)Unsafe.SizeOf<TSerializable>();
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            UInt32 written = handler.Serialize(buffer: buffer,
                                               graph: graph!);
            return written;
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
    static public unsafe UInt32 Serialize<TSerializable>(Byte* buffer,
                                                         TSerializable? graph)
    {
        if (typeof(TSerializable).IsUnmanagedStruct())
        {
            Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
            return (UInt32)Unsafe.SizeOf<TSerializable>();
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            Int32 size = handler.GetExpectedArraySize(graph);
            UInt32 written = handler.Serialize(buffer: new Span<Byte>(buffer, size),
                                               graph: graph);
            return written;
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
    static public UInt32 Serialize<TSerializable>(Stream stream,
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
    static public UInt32 Serialize<TStream, TSerializable>(TStream stream,
                                                           TSerializable? graph)
        where TStream : IWriteableStream
    {
        Byte[] buffer = Serialize(graph);
        stream.Write(buffer);
        return (UInt32)buffer.Length;
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
    static public Task<UInt32> SerializeAsynchronously<TSerializable>(Stream stream,
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
    static public async Task<UInt32> SerializeAsynchronously<TStream, TSerializable>(TStream stream,
                                                                                     TSerializable? graph,
                                                                                     CancellationToken cancellationToken = default)
        where TStream : IWriteableStream
    {
        Byte[] buffer = Serialize(graph);
        await stream.WriteAsynchronously(buffer: buffer,
                                         cancellationToken: cancellationToken);
        return (UInt32)buffer.Length;
    }
}