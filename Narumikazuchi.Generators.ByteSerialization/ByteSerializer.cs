namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides methods to serialize runtime objects into and from <see cref="Byte"/>[] arrays or streams.
/// </summary>
/// <remarks>
/// To make a runtime object serializable you can either decorate it with the <see cref="ByteSerializableAttribute"/>, in which case
/// the serialization code will be generated automatically, or write a <see cref="IByteSerializationStrategy{TSerializable}"/> for
/// the type.
/// </remarks>
static public partial class ByteSerializer
{
    /// <summary>
    /// Calculates the expected size of the <see cref="Byte"/>[] array after serialization of the sepcified runtime object.
    /// </summary>
    /// <param name="graph">The runtime object to calculate the expected size of.</param>
    /// <returns>The expected size of the <see cref="Byte"/>[] array after serialization.</returns>
    static public Int32 GetExpectedSerializedSize<TSerializable>(TSerializable graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        return TSerializable.GetExpectedByteSize(graph);
    }
    /// <summary>
    /// Calculates the expected size of the <see cref="Byte"/>[] array after serialization of the sepcified runtime object.
    /// </summary>
    /// <param name="graph">The runtime object to calculate the expected size of.</param>
    /// <returns>The expected size of the <see cref="Byte"/>[] array after serialization.</returns>
    static public Int32 GetExpectedSerializedSize<TSerializable>(IEnumerable<TSerializable> graph)
        where TSerializable : IByteSerializable<TSerializable>
    {
        Int32 expectedSize = sizeof(Int32);
        foreach (TSerializable element in graph)
        {
            expectedSize += TSerializable.GetExpectedByteSize(element);
        }

        return expectedSize;
    }
    /// <summary>
    /// Calculates the expected size of the <see cref="Byte"/>[] array after serialization of the sepcified runtime object using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="graph">The runtime object to calculate the expected size of.</param>
    /// <returns>The expected size of the <see cref="Byte"/>[] array after serialization.</returns>
    static public Int32 GetExpectedSerializedSize<TSerializable, TStrategy>(TSerializable graph)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        return TStrategy.GetExpectedByteSize(graph);
    }
    /// <summary>
    /// Calculates the expected size of the <see cref="Byte"/>[] array after serialization of the sepcified runtime object using the specified <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <param name="graph">The runtime object to calculate the expected size of.</param>
    /// <returns>The expected size of the <see cref="Byte"/>[] array after serialization.</returns>
    static public Int32 GetExpectedSerializedSize<TSerializable, TStrategy>(IEnumerable<TSerializable> graph)
        where TStrategy : IByteSerializationStrategy<TSerializable>
    {
        Int32 expectedSize = sizeof(Int32);
        foreach (TSerializable element in graph)
        {
            expectedSize += TStrategy.GetExpectedByteSize(element);
        }

        return expectedSize;
    }
}