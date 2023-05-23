namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Specifies the serialized data layout for this type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DataLayoutAttribute : Attribute
{
    /// <summary>
    /// Specifies the serialized data layout for this type.
    /// </summary>
    /// <param name="layout">The layout style to use for serialization.</param>
    public DataLayoutAttribute(DataLayout layout = DataLayout.Default)
    {
        this.Layout = layout;
    }

    /// <summary>
    /// Gets the layout style.
    /// </summary>
    public DataLayout Layout { get; }
}