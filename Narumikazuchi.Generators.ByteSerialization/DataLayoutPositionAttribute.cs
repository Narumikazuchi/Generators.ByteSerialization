namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Determines the position of this data member in the serialization order.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class DataLayoutPositionAttribute : Attribute
{
    /// <summary>
    /// Determines the position of this data member in the serialization order.
    /// </summary>
    /// <param name="position">The position in the serialization order.</param>
    public DataLayoutPositionAttribute(Byte position)
    {
        this.Position = position;
    }

    /// <summary>
    /// Gets the position in the serialization order.
    /// </summary>
    public Byte Position { get; }
}