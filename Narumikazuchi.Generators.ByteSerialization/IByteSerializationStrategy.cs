namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides Methods to serialize a runtime object of type <typeparamref name="TSerializable"/>.
/// </summary>
public interface IByteSerializationStrategy<TSerializable>
{
    /// <summary>
    /// Deserializes the contents of the <paramref name="buffer"/> into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The buffer to read from.</param>
    /// <param name="read">The amount of bytes read from the buffer.</param>
    /// <returns>The <typeparamref name="TSerializable"/> represented by the buffer.</returns>
    static public abstract TSerializable Deserialize(ReadOnlySpan<Byte> buffer,
                                                     out Int32 read);

    /// <summary>
    /// Calculatees the expected amount of bytes the specified <paramref name="value"/> will occupy once serialized. 
    /// </summary>
    /// <param name="value">The value to use as base for calculation.</param>
    /// <returns>The expected amount of bytes the value will occupy in it's serialized state.</returns>
    static public abstract Int32 GetExpectedByteSize(TSerializable value);

    /// <summary>
    /// Serializes the specified <paramref name="value"/> into the <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">The buffer into which to store the serialized value.</param>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The amount of bytes written to the buffer.</returns>
    static public abstract Int32 Serialize(Span<Byte> buffer,
                                           TSerializable value);
}