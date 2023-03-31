namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SizeHelper
{
    static public void WriteKnownTypeSize(ISymbol target,
                                          StringBuilder builder,
                                          String typename,
                                          String indent,
                                          ref Int32 expectedSize)
    {
        switch (typename)
        {
            case nameof(Boolean):
            case nameof(Byte):
            case nameof(SByte):
                expectedSize++;
                break;
            case nameof(Char):
            case "Half":
            case nameof(Int16):
            case nameof(UInt16):
                expectedSize += 2;
                break;
            case "DateOnly":
            case nameof(Int32):
            case nameof(Single):
            case nameof(UInt32):
                expectedSize += 4;
                break;
            case nameof(DateTime):
            case nameof(DateTimeOffset):
            case nameof(Double):
            case nameof(Int64):
            case "TimeOnly":
            case nameof(TimeSpan):
            case nameof(UInt64):
                expectedSize += 8;
                break;
            case nameof(Decimal):
            case nameof(Guid):
                expectedSize += 16;
                break;
            case nameof(String):
                builder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<String, Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy>(value.{target.Name});");
                break;
        }
    }

    static public void WriteKnownTypeSizeEnumerable(ISymbol target,
                                                    ITypeSymbol type,
                                                    StringBuilder builder,
                                                    String indent,
                                                    ref Int32 expectedSize)
    {
        String typename = type.ToTypename();
        switch (typename)
        {
            case nameof(Boolean):
            case nameof(Byte):
            case nameof(SByte):
                builder.AppendLine($"{indent}expectedSize += 4 + value.{type.EnumerableCount()};");
                break;
            case nameof(Char):
            case "Half":
            case nameof(Int16):
            case nameof(UInt16):
                builder.AppendLine($"{indent}expectedSize += 4 + 2 * value.{type.EnumerableCount()};");
                break;
            case "DateOnly":
            case nameof(Int32):
            case nameof(Single):
            case nameof(UInt32):
                builder.AppendLine($"{indent}expectedSize += 4 + 4 * value.{type.EnumerableCount()};");
                break;
            case nameof(DateTime):
            case nameof(DateTimeOffset):
            case nameof(Double):
            case nameof(Int64):
            case "TimeOnly":
            case nameof(TimeSpan):
            case nameof(UInt64):
                builder.AppendLine($"{indent}expectedSize += 4 + 8 * value.{type.EnumerableCount()};");
                break;
            case nameof(Decimal):
            case nameof(Guid):
                builder.AppendLine($"{indent}expectedSize += 4 + 16 * value.{type.EnumerableCount()};");
                break;
            case nameof(String):
                builder.AppendLine($"{indent}expectedSize += 4;");
                builder.AppendLine($"{indent}foreach (String item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<String, Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy>(item);");
                builder.AppendLine($"{indent}}}");
                break;
        }
    }
}