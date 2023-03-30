namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Tells the code generator that this strategy will always occupy the same amount of bytes after serialization.
/// </summary>
/// <remarks>
/// Mark <see cref="IByteSerializationStrategy{TSerializable}"/> implementations with this attribute to
/// optimize the generated serialization code. Has no effect on any other class or struct.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class FixedSerializationSizeAttribute : Attribute
{
    /// <summary>
    /// Tells the code generator that this strategy will always occupy the same amount of bytes after serialization.
    /// </summary>
    /// <remarks>
    /// Mark <see cref="IByteSerializationStrategy{TSerializable}"/> implementations with this attribute to
    /// optimize the generated serialization code. Has no effect on any other class or struct.
    /// </remarks>
    public FixedSerializationSizeAttribute(Int32 size)
    { }
}