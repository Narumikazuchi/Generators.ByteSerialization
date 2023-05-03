namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Tells the code generator, which field or property from the serialization process
/// represents this parameter value in the constructor.
/// </summary>
/// <remarks>
/// Use <see langword="nameof"/> followed by the field- or property-name to specify
/// the member to use. You can also use a raw <see cref="String"/> for this, but 
/// <see langword="nameof"/> is more robust and therefore recommended.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SerializeFromMemberAttribute : Attribute
{
    /// <summary>
    /// Tells the code generator, which field or property from the serialization process
    /// represents this parameter value in the constructor.
    /// </summary>
    /// <param name="fieldOrPropertyName">The name of the field or property to map to the constructor parameter.</param>
    /// <remarks>
    /// Use <see langword="nameof"/> followed by the field- or property-name to specify
    /// the member to use. You can also use a raw <see cref="String"/> for this, but 
    /// <see langword="nameof"/> is more robust and therefore recommended.
    /// </remarks>
    public SerializeFromMemberAttribute(String fieldOrPropertyName)
    { }
}