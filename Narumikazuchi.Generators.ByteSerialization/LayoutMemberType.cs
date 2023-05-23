namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents the type of the member for a <see cref="ByteSerialization.TypeLayout"/>.
/// </summary>
public enum LayoutMemberType : Byte
{
#pragma warning disable CS1591
    Unknown,
    TypeLayout,
    Boolean,
    UnsignedInteger8Bits,
    Integer8Bits,
    Char,
    Integer16Bits,
    UnsignedInteger16Bits,
    SinglePrecisionFloat,
    Integer32Bits,
    UnsignedInteger32Bits,
    DoublePrecisionFloat,
    Integer64Bits,
    UnsignedInteger64Bits,
    Decimal,
    String,
    Object
}