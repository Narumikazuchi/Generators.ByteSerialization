namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents the serialized layout of data.
/// </summary>
public enum DataLayout : Byte
{
    /// <summary>
    /// Data will be serialized in the order the properties/fields appear in the type declaration.
    /// </summary>
    Sequential,
    /// <summary>
    /// Data will be serialized in the order specified by the <see cref="DataLayoutPositionAttribute"/>.
    /// </summary>
    Explicit,
    /// <summary>
    /// Data will be serialized in the default order.
    /// </summary>
    Default = Sequential
}