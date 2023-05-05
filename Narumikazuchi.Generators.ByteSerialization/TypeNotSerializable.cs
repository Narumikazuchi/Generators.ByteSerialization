namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents the failure of the <see cref="ByteSerializer"/> to serialize or deserialize a type.
/// </summary>
public sealed class TypeNotSerializable : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeNotSerializable"/> exception.
    /// </summary>
    /// <param name="type">The type that failed to serialize.</param>
    public TypeNotSerializable(Type type)
        : base($"No serialization code could be found for the type '${type.FullName}'.")
    { }
}