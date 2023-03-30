namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Tells the serialization code generator to use the <typeparamref name="TStrategy"/> when serializing and
/// deserializing members of type <typeparamref name="TSerializable"/>.
/// </summary>
/// <remarks>
/// This attribute will only be effective on types that are decorated with the <see cref="ByteSerializableAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
public sealed class UseByteSerializationStrategyAttribute<TSerializable, TStrategy> : Attribute
    where TStrategy : IByteSerializationStrategy<TSerializable>
{
    /// <summary>
    /// Tells the serialization code generator to use the <typeparamref name="TStrategy"/> when serializing and
    /// deserializing members of type <typeparamref name="TSerializable"/>.
    /// </summary>
    /// <remarks>
    /// This attribute will only be effective on types that are decorated with the <see cref="ByteSerializableAttribute"/>.
    /// </remarks>
    public UseByteSerializationStrategyAttribute()
    { }
}