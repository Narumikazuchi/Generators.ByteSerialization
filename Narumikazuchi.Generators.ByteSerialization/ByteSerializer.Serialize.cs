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
    static public unsafe Byte[] Serialize<TSerializable>(TSerializable? graph)
    {
        Type? type = graph?.GetType();
        if (type is null)
        {
            Byte[] buffer = new Byte[21];
            Unsafe.As<Byte, Int32>(ref buffer[0]) = 0;
            Unsafe.As<Byte, TypeIdentifier>(ref buffer[4]) = Handlers.Types.GetLeftPartner(typeof(TSerializable));
            buffer[36] = 0x0;
            return buffer;
        }
        else if (type.IsValueType ||
                 type.IsSealed)
        {
            if (type.IsUnmanagedStruct())
            {
                Byte[] buffer = new Byte[Unsafe.SizeOf<TSerializable>()];
                Unsafe.As<Byte, TSerializable>(ref buffer[0]) = graph!;
                return buffer;
            }
            else if (Handlers is ISerializationHandler<TSerializable> handler)
            {
                Byte[] buffer = ArrayPool<Byte>.Shared.Rent(handler.GetExpectedArraySize(graph!) + 37);
                fixed (Byte* pointer = buffer)
                {
                    Int32 written = Serialize(buffer: pointer,
                                              graph: graph);
                    Byte[] result = new Byte[written];
                    Array.Copy(sourceArray: buffer,
                               destinationArray: result,
                               length: written);
                    ArrayPool<Byte>.Shared.Return(buffer);
                    return result;
                }
            }
            else
            {
                throw new TypeNotSerializable(type);
            }
        }
        else
        {
            if (type == typeof(TSerializable) &&
                Handlers is ISerializationHandler<TSerializable> handler)
            {
                Byte[] buffer = ArrayPool<Byte>.Shared.Rent(handler.GetExpectedArraySize(graph!) + 37);
                fixed (Byte* pointer = buffer)
                {
                    Int32 written = Serialize(buffer: pointer,
                                              graph: graph);
                    Byte[] result = new Byte[written];
                    Array.Copy(sourceArray: buffer,
                               destinationArray: result,
                               length: written);
                    ArrayPool<Byte>.Shared.Return(buffer);
                    return result;
                }
            }
            else
            {
                return SerializeAs(graph!);
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
    static public unsafe Int32 Serialize<TSerializable>(Span<Byte> buffer,
                                                        TSerializable? graph)
    {
        if (graph is null)
        {
            Unsafe.As<Byte, Int32>(ref buffer[0]) = 0;
            Unsafe.As<Byte, TypeIdentifier>(ref buffer[4]) = Handlers.Types.GetLeftPartner(typeof(TSerializable));
            buffer[36] = 0x0;
            return 37;
        }
        else
        {
            return Serialize(buffer: (Byte*)Unsafe.AsPointer(ref buffer[0]),
                             graph: graph);
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
                                                        TSerializable? graph)
    {
        Type? type = graph?.GetType();
        if (type is null)
        {
            *(Int32*)buffer = 0;
            *(TypeIdentifier*)(buffer + 4) = Handlers.Types.GetLeftPartner(typeof(TSerializable));
            *(buffer + 36) = 0x0;
            return 37;
        }
        else if (type.IsValueType ||
                 type.IsSealed)
        {
            if (type.IsUnmanagedStruct())
            {
                Unsafe.As<Byte, TSerializable>(ref Unsafe.AsRef<Byte>(buffer)) = graph!;
                return Unsafe.SizeOf<TSerializable>();
            }
            else if (Handlers is ISerializationHandler<TSerializable> handler)
            {
                *(TypeIdentifier*)(buffer + 4) = Handlers.Types.GetLeftPartner(typeof(TSerializable));
                *(buffer + 36) = 0x1;
                Int32 expectedSize = GetExpectedSerializedSize(graph);
                Int32 written = handler.Serialize(buffer: buffer + 37,
                                                  graph: graph!);
                *(Int32*)buffer = written;
                return written + 37;
            }
            else
            {
                throw new TypeNotSerializable(type);
            }
        }
        else
        {
            if (type == typeof(TSerializable) &&
                Handlers is ISerializationHandler<TSerializable> handler)
            {
                *(TypeIdentifier*)(buffer + 4) = Handlers.Types.GetLeftPartner(typeof(TSerializable));
                *(buffer + 36) = 0x1;
                Int32 expectedSize = GetExpectedSerializedSize(graph);
                Int32 written = handler.Serialize(buffer: buffer + 37,
                                                  graph: graph!);
                *(Int32*)buffer = written;
                return written + 37;
            }
            else
            {
                return SerializeAs(buffer: (IntPtr)buffer,
                                   graph: graph!);
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
    static public Int32 Serialize<TStream, TSerializable>(TStream stream,
                                                          TSerializable? graph)
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
    static public async Task<Int32> SerializeAsynchronously<TStream, TSerializable>(TStream stream,
                                                                                    TSerializable? graph,
                                                                                    CancellationToken cancellationToken = default)
        where TStream : IWriteableStream
    {
        Byte[] buffer = Serialize(graph);
        await stream.WriteAsynchronously(buffer: buffer,
                                         cancellationToken: cancellationToken);
        return buffer.Length;
    }

    static private Byte[] SerializeAs(Object graph)
    {
        Type type = graph.GetType();
        if (s_PolymorphicSimpleSerializers.TryGetValue(key: type,
                                                       value: out DynamicMethod? method))
        {
            return (Byte[])method.Invoke(obj: null,
                                         parameters: new Object[] { graph })!;
        }
        else
        {
            method = new DynamicMethod(name: "<Polymorphic_SimpleSerialize_Overload>",
                                       returnType: typeof(Byte).MakeArrayType(),
                                       parameterTypes: new Type[] { type },
                                       owner: typeof(ByteSerializer));
            ILGenerator generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, s_PolymorphicSimpleSerializerMethod.Value.MakeGenericMethod(type));
            generator.Emit(OpCodes.Ret);

            s_PolymorphicSimpleSerializers.Add(key: type,
                                               value: method);

            return (Byte[])method.Invoke(obj: null,
                                         parameters: new Object[] { graph })!;
        }
    }

    static private Int32 SerializeAs(IntPtr buffer,
                                     Object graph)
    {
        Type type = graph.GetType();
        if (s_PolymorphicSerializers.TryGetValue(key: type,
                                                 value: out DynamicMethod? method))
        {
            return (Int32)method.Invoke(obj: null,
                                        parameters: new Object[] { buffer, graph })!;
        }
        else
        {
            method = new DynamicMethod(name: "<Polymorphic_Serialize_Overload>",
                                       returnType: typeof(Int32),
                                       parameterTypes: new Type[] { typeof(IntPtr), type },
                                       owner: typeof(ByteSerializer));
            ILGenerator generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, s_PolymorphicSerializerMethod.Value.MakeGenericMethod(type));
            generator.Emit(OpCodes.Ret);

            s_PolymorphicSerializers.Add(key: type,
                                         value: method);

            return (Int32)method.Invoke(obj: null,
                                        parameters: new Object[] { buffer, graph })!;
        }
    }

    static private MethodInfo GetPolymorphicSerializerMethod()
    {
        return typeof(ByteSerializer).GetMethods()
                                     .Where(method => method.Name is nameof(Serialize))
                                     .Where(method => method.IsStatic)
                                     .Where(method => method.IsPublic)
                                     .First(method => method.GetParameters()[0].ParameterType == typeof(Byte).MakePointerType());
    }

    static private MethodInfo GetPolymorphicSimpleSerializerMethod()
    {
        return typeof(ByteSerializer).GetMethods()
                                     .Where(method => method.Name is nameof(Serialize))
                                     .Where(method => method.IsStatic)
                                     .Where(method => method.IsPublic)
                                     .First(method => method.GetParameters().Length is 1);
    }

    static private readonly Dictionary<Type, DynamicMethod> s_PolymorphicSerializers = new();
    static private readonly Dictionary<Type, DynamicMethod> s_PolymorphicSimpleSerializers = new();
    static private readonly Lazy<MethodInfo> s_PolymorphicSerializerMethod = new(valueFactory: GetPolymorphicSerializerMethod,
                                                                                 mode: LazyThreadSafetyMode.ExecutionAndPublication);
    static private readonly Lazy<MethodInfo> s_PolymorphicSimpleSerializerMethod = new(valueFactory: GetPolymorphicSimpleSerializerMethod,
                                                                                       mode: LazyThreadSafetyMode.ExecutionAndPublication);
}