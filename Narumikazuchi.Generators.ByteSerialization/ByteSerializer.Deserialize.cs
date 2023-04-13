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
    static public Int32 Deserialize<TSerializable>(ReadOnlySpan<Byte> buffer,
                                                   out TSerializable? result)
    {
        Type type = typeof(TSerializable);
        if (type.IsUnmanagedStruct())
        {
            result = Unsafe.As<Byte, TSerializable>(ref MemoryMarshal.GetReference(buffer));
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (type.IsValueType ||
                 type.IsSealed)
        {
            Guid guid = TypeCache.GetIdOfType(type);

            Int32 size = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer));
            Guid serialized = Unsafe.As<Byte, Guid>(ref MemoryMarshal.GetReference(buffer[4..]));
            if (size is 16)
            {
                result = default;
                return 20;
            }
            else if (guid == serialized &&
                     Handlers is ISerializationHandler<TSerializable> handler)
            {
                return 20 + handler.Deserialize(buffer: buffer[20..(size + 4)],
                                                result: out result);
            }
            else
            {
                throw new TypeNotSerializable(type);
            }
        }
        else
        {
            Guid guid = TypeCache.GetIdOfType(type);

            Int32 size = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer));
            Guid serialized = Unsafe.As<Byte, Guid>(ref MemoryMarshal.GetReference(buffer[4..]));

            if (size is 16)
            {
                result = default;
                return 20;
            }
            else if (guid == serialized &&
                     Handlers is ISerializationHandler<TSerializable> handler)
            {
                return 20 + handler.Deserialize(buffer: buffer[20..(size + 4)],
                                                result: out result);
            }
            else
            {
                foreach (Type candidate in TypeCache.GetTypeCandidatesForId(serialized))
                {
                    if (!candidate.IsAssignableTo(type))
                    {
                        continue;
                    }

                    Int32 read = DeserializeAs(type: candidate,
                                               buffer: PointerFromBuffer(buffer),
                                               result: out Object? boxed);
                    result = (TSerializable?)boxed;
                    return read;
                }

                throw new TypeNotSerializable(type);
            }
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
    static public unsafe Int32 Deserialize<TSerializable>(Byte* buffer,
                                                          out TSerializable? result)
    {
        Type type = typeof(TSerializable);
        if (type.IsUnmanagedStruct())
        {
            result = Unsafe.As<Byte, TSerializable>(ref Unsafe.AsRef<Byte>(buffer));
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (type.IsValueType ||
                 type.IsSealed)
        {
            Guid guid = TypeCache.GetIdOfType(type);

            Int32 size = *(Int32*)buffer;
            Guid serialized = *(Guid*)(buffer + 4);
            if (size is 16)
            {
                result = default;
                return 20;
            }
            else if (guid == serialized &&
                     Handlers is ISerializationHandler<TSerializable> handler)
            {
                return 20 + handler.Deserialize(buffer: new ReadOnlySpan<Byte>(buffer + 20, size),
                                                result: out result);
            }
            else
            {
                throw new TypeNotSerializable(type);
            }
        }
        else
        {
            Guid guid = TypeCache.GetIdOfType(type);

            Int32 size = *(Int32*)buffer;
            Guid serialized = *(Guid*)(buffer + 4);
            if (size is 16)
            {
                result = default;
                return 20;
            }
            else if (guid == serialized &&
                     Handlers is ISerializationHandler<TSerializable> handler)
            {
                return 20 + handler.Deserialize(buffer: new ReadOnlySpan<Byte>(buffer + 20, size),
                                                result: out result);
            }
            else
            {
                foreach (Type candidate in TypeCache.GetTypeCandidatesForId(serialized))
                {
                    if (!candidate.IsAssignableTo(type))
                    {
                        continue;
                    }

                    Int32 read = DeserializeAs(type: candidate,
                                               buffer: (IntPtr)buffer,
                                               result: out Object? boxed);
                    result = (TSerializable?)boxed;
                    return read;
                }

                throw new TypeNotSerializable(type);
            }
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
    static public Int32 Deserialize<TSerializable>(Stream stream,
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
    static public Int32 Deserialize<TStream, TSerializable>(TStream stream,
                                                            out TSerializable? result)
        where TStream : IReadableStream
    {
        Byte[] buffer = new Byte[4];
        stream.Read(buffer);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size + 4];
        Unsafe.As<Byte, Int32>(ref buffer[0]) = size;
        stream.Read(buffer.AsSpan()[4..]);
        Int32 read = Deserialize(buffer: buffer,
                                 result: out result);
        read += sizeof(Int32);
        return read;
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
        Byte[] buffer = new Byte[4];
        await stream.ReadAsynchronously(buffer: buffer,
                                        cancellationToken: cancellationToken);
        Int32 size = Unsafe.As<Byte, Int32>(ref buffer[0]);
        buffer = new Byte[size + 4];
        Unsafe.As<Byte, Int32>(ref buffer[0]) = size;
        await stream.ReadAsynchronously(buffer: buffer.AsMemory()[4..],
                                        cancellationToken: cancellationToken);
        Int32 read = Deserialize(buffer: buffer,
                                 result: out TSerializable? result);
        read += sizeof(Int32);
        return new AsynchronousDeserializationResult<TSerializable?>
        {
            BytesRead = read,
            Result = result
        };
    }

    static private unsafe IntPtr PointerFromBuffer(ReadOnlySpan<Byte> buffer)
    {
        ref Byte first = ref MemoryMarshal.GetReference(buffer);
        return (IntPtr)Unsafe.AsPointer(ref first);
    }

    static private Int32 DeserializeAs(Type type,
                                       IntPtr buffer,
                                       out Object? result)
    {
        if (s_PolymorphicDeserializers.TryGetValue(key: type,
                                                   value: out DynamicMethod? method))
        {
            Object?[] parameters = new Object?[] { buffer, null };
            Int32 read = (Int32)method.Invoke(obj: null,
                                              parameters: parameters)!;
            result = parameters[1];
            return read;
        }
        else
        {
            method = new DynamicMethod(name: "<Internal_Deserialize_Overload>",
                                       returnType: typeof(Int32),
                                       parameterTypes: new Type[] { typeof(IntPtr), type.MakeByRefType() },
                                       owner: typeof(ByteSerializer));
            ILGenerator generator = method.GetILGenerator();
            generator.DeclareLocal(typeof(Int32));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, s_PolymorphicDeserializerMethod.MakeGenericMethod(type));
            generator.Emit(OpCodes.Ret);

            s_PolymorphicDeserializers.Add(key: type,
                                           value: method);

            Object?[] parameters = new Object?[] { buffer, null };
            Int32 read = (Int32)method.Invoke(obj: null,
                                              parameters: parameters)!;
            result = parameters[1];
            return read;
        }
    }

    static private readonly Dictionary<Type, DynamicMethod> s_PolymorphicDeserializers = new();
    static private readonly MethodInfo s_PolymorphicDeserializerMethod;
}