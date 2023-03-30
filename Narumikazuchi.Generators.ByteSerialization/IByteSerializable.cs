namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides methods necessary for serialization of the type <typeparamref name="TSerializable"/>.
/// </summary>
/// <remarks>
/// While you can implement this interface yourself and you can then use it with the <see cref="ByteSerializer"/>
/// methods, this interface is mainly used by the code generator. The code generator is really efficient in 
/// writing serialization code, so try to use the code generator if possible.
/// </remarks>
public interface IByteSerializable<TSerializable>
    where TSerializable : IByteSerializable<TSerializable>
{
    /// <summary>
    /// Serializes a runtime object of type <typeparamref name="TSerializable"/> into it's <see cref="Byte"/>-representation.
    /// </summary>
    /// <param name="buffer">The buffer to serialize the value into.</param>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The amount of bytes written to the buffer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static public abstract Int32 Serialize(Span<Byte> buffer,
                                           TSerializable value);

    /// <summary>
    /// Deserializes the <paramref name="buffer"/> into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The buffer to read the bytes from.</param>
    /// <param name="read">The amount of bytes read from the buffer.</param>
    /// <returns>The runtime object represented by the buffer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static public abstract TSerializable Deserialize(ReadOnlySpan<Byte> buffer,
                                                     out Int32 read);

    /// <summary>
    /// Calculates the expected size of the serialized object.
    /// </summary>
    /// <param name="value">The object the calculation is based on.</param>
    /// <returns>The expected size of the serialized object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static public abstract Int32 GetExpectedByteSize(TSerializable value);
}