namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Ignores this member when serializing this object.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class IngoreForSerializationAttribute : Attribute
{
    /// <summary>
    /// Ignores this member when serializing this object.
    /// </summary>
    public IngoreForSerializationAttribute()
    { }
}