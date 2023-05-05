namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents the exception for when the deserialization of a type fails due to the serialized type being different.
/// </summary>
public sealed class WrongTypeDeserialization : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WrongTypeDeserialization"/> exception.
    /// </summary>
    /// <param name="type">The type that failed to deserialize.</param>
    public WrongTypeDeserialization(Type type)
        : base($"Deserialization of type '${type.FullName}' failed, because the serialized object is of a different type.")
    { }
}