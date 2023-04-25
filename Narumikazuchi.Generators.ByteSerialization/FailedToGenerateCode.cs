namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents the failure of the generator to generate serialization code.
/// </summary>
public sealed class FailedToGenerateCode : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedToGenerateCode"/> exception.
    /// </summary>
    public FailedToGenerateCode()
        : base("The 'ByteSerialize' could not find the generated serialization code. The generator most likely failed to generate serialization code.")
    { }
}