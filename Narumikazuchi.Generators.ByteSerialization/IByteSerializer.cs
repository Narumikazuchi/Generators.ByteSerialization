namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// -- Used for internal code generation. --
/// </summary>
public interface IByteSerializer
{
    /// <summary>
    /// -- Used for internal code generation. --
    /// </summary>
    public UInt32 Variant { get; }

    /// <summary>
    /// -- Used for internal code generation. --
    /// </summary>
    public MirroredMap<TypeIdentifier, Type> Types { get; }
}