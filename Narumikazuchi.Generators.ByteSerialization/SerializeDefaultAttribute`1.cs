namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Tells the generator to use the supplied default value for this parameter, when using
/// the constructor for initializing the object during deserialization.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SerializeDefaultAttribute<TParameter> : Attribute
{
    /// <summary>
    /// Tells the generator to use the supplied default value for this parameter, when using
    /// the constructor for initializing the object during deserialization.
    /// </summary>
    /// <param name="defaultValue">The default value to always use for the constructor parameter.</param>
    public SerializeDefaultAttribute(TParameter defaultValue)
    { }
}
