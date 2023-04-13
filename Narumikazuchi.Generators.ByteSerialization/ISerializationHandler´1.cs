namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides Methods to serialize a runtime object of type <typeparamref name="TSerializable"/>.
/// </summary>
public interface ISerializationHandler<TSerializable>
{
    /// <summary>
    /// Deserializes the contents of the <paramref name="buffer"/> into a runtime object of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <param name="buffer">The buffer to read from.</param>
    /// <param name="result">The <typeparamref name="TSerializable"/> represented by the buffer.</param>
    /// <returns>The amount of bytes read from the buffer.</returns>
    public Int32 Deserialize(ReadOnlySpan<Byte> buffer,
                             out TSerializable? result);

    /// <summary>
    /// Calculates the expected amount of bytes the specified <paramref name="graph"/> will occupy once serialized. 
    /// </summary>
    /// <param name="graph">The value to use as base for calculation.</param>
    /// <returns>The expected amount of bytes the value will occupy in it's serialized state.</returns>
    public Int32 GetExpectedSize(TSerializable? graph);

    /// <summary>
    /// Serializes the specified <paramref name="graph"/> into the <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">The buffer into which to store the serialized value.</param>
    /// <param name="graph">The value to serialize.</param>
    /// <returns>The amount of bytes written to the buffer.</returns>
    public Int32 Serialize(Span<Byte> buffer,
                           TSerializable? graph);
}