namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Generates serialization code for this type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ByteSerializableAttribute : Attribute
{
    /// <summary>
    /// Generates serialization code for this type.
    /// </summary>
    public ByteSerializableAttribute()
    { }
}