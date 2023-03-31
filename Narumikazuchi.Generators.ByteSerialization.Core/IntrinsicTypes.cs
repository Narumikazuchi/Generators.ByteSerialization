namespace Narumikazuchi.Generators.ByteSerialization;

static public class IntrinsicTypes
{
    static public String[] SerializedTypes { get; } = new[]
    {
        nameof(Boolean),
        nameof(Byte),
        nameof(Char),
        "DateOnly",
        nameof(DateTime),
        nameof(DateTimeOffset),
        nameof(Decimal),
        nameof(Double),
        nameof(Guid),
        "Half",
        nameof(Int16),
        nameof(Int32),
        nameof(Int64),
        nameof(SByte),
        nameof(Single),
        nameof(String),
        "TimeOnly",
        nameof(TimeSpan),
        nameof(UInt16),
        nameof(UInt32),
        nameof(UInt64),
        nameof(Version)
    };
}