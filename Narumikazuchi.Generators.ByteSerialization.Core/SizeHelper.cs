namespace Narumikazuchi.Generators.ByteSerialization;

static public class SizeHelper
{
    static public void WriteKnownTypeSize(IFieldSymbol field,
                                          ISymbol target,
                                          StringBuilder builder,
                                          String indent,
                                          ref Int32 expectedSize)
    {
        String typename = field.Type.ToTypename();
        WriteKnownTypeSize(target: target,
                           builder: builder,
                           typename: typename,
                           indent: indent,
                           expectedSize: ref expectedSize);
    }

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
}