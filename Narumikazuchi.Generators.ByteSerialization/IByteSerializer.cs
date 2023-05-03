using System.ComponentModel;

namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// -- Used for internal code generation. --
/// </summary>
[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IByteSerializer
{
    /// <summary>
    /// -- Used for internal code generation. --
    /// </summary>
    public UInt32 Variant { get; }
}