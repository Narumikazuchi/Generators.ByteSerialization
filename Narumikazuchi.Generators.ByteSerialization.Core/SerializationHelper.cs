namespace Narumikazuchi.Generators.ByteSerialization;

static public class SerializationHelper
{
    static public void WriteKnownTypeSerialization(IFieldSymbol field,
                                                   ISymbol target,
                                                   StringBuilder builder,
                                                   String indent)
    {
        String typename = field.Type.ToTypename();
        switch (typename)
        {
            case nameof(Boolean):
                builder.AppendLine($"{indent}buffer[pointer++] = value.{target.Name} ? 0x1 : 0x0;");
                break;
            case nameof(Byte):
                builder.AppendLine($"{indent}buffer[pointer++] = value.{target.Name};");
                break;
            case nameof(Char):
            case nameof(Decimal):
            case nameof(Double):
            case "Half":
            case nameof(Int16):
            case nameof(Int32):
            case nameof(Int64):
            case nameof(Single):
            case nameof(UInt16):
            case nameof(UInt32):
            case nameof(UInt64):
                builder.AppendLine($"{indent}Unsafe.As<Byte, {typename}>(ref MemoryMarshal.GetReference(buffer[pointer..])) = value.{target.Name};");
                builder.AppendLine($"{indent}pointer += sizeof({typename});");
                break;
            case "DateOnly":
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int16>(ref MemoryMarshal.GetReference(buffer[pointer..])) = (Int16)value.{target.Name}.Year;");
                builder.AppendLine($"{indent}pointer += 2;");
                builder.AppendLine($"{indent}buffer[pointer++] = (Byte)value.{target.Name}.Month;");
                builder.AppendLine($"{indent}buffer[pointer++] = (Byte)value.{target.Name}.Day;");
                break;
            case nameof(DateTime):
            case nameof(DateTimeOffset):
            case "TimeOnly":
            case nameof(TimeSpan):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int64>(ref MemoryMarshal.GetReference(buffer[pointer..])) = value.{target.Name}.Ticks;");
                builder.AppendLine($"{indent}pointer += 8;");
                break;
            case nameof(Guid):
                builder.AppendLine($"{indent}_ = value.{target.Name}.TryWriteBytes(buffer[pointer..(pointer + 16)]);");
                builder.AppendLine($"{indent}pointer += 16;");
                break;
            case nameof(SByte):
                builder.AppendLine($"{indent}buffer[pointer++] = (Byte)value.{target.Name};");
                break;
            case nameof(String):
                builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy.Serialize(buffer[pointer..], value.{target.Name});");
                break;
        }
    }

    static public void WriteEnumTypeSerialization(IFieldSymbol field,
                                                  ISymbol target,
                                                  ITypeSymbol baseType,
                                                  StringBuilder builder,
                                                  String indent)
    {
        String typename = baseType.ToTypename();
        builder.AppendLine($"{indent}Unsafe.As<Byte, {typename}>(ref MemoryMarshal.GetReference(buffer[pointer..])) = ({typename})value.{target.Name};");
        builder.AppendLine($"{indent}pointer += sizeof({typename});");
    }
}