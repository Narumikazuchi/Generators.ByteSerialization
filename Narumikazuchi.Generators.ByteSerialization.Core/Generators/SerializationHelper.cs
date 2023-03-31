namespace Narumikazuchi.Generators.ByteSerialization.Generators;

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
                builder.AppendLine($"{indent}buffer[pointer++] = value.{target.Name} ? (Byte)0x1 : (Byte)0x0;");
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
                builder.AppendLine($"{indent}Unsafe.As<Byte, {typename}>(ref buffer[pointer]) = value.{target.Name};");
                builder.AppendLine($"{indent}pointer += sizeof({typename});");
                break;
            case "DateOnly":
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int16>(ref buffer[pointer]) = (Int16)value.{target.Name}.Year;");
                builder.AppendLine($"{indent}pointer += 2;");
                builder.AppendLine($"{indent}buffer[pointer++] = (Byte)value.{target.Name}.Month;");
                builder.AppendLine($"{indent}buffer[pointer++] = (Byte)value.{target.Name}.Day;");
                break;
            case nameof(DateTime):
            case nameof(DateTimeOffset):
            case "TimeOnly":
            case nameof(TimeSpan):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int64>(ref buffer[pointer]) = value.{target.Name}.Ticks;");
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

    static public void WriteEnumerableKnownTypeSerialization(ITypeSymbol enumerableType,
                                                             ITypeSymbol elementType,
                                                             ISymbol target,
                                                             StringBuilder builder,
                                                             String indent)
    {
        String typename = elementType.ToTypename();
        switch (typename)
        {
            case nameof(Boolean):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    buffer[pointer++] = item ? (Byte)0x1 : (Byte)0x0;");
                builder.AppendLine($"{indent}}}");
                break;
            case nameof(Byte):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    buffer[pointer++] = item;");
                builder.AppendLine($"{indent}}}");
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
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    Unsafe.As<Byte, {typename}>(ref buffer[pointer]) = item;");
                builder.AppendLine($"{indent}    pointer += sizeof({typename});");
                builder.AppendLine($"{indent}}}");
                break;
            case "DateOnly":
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    Unsafe.As<Byte, Int16>(ref buffer[pointer]) = (Int16)item.Year;");
                builder.AppendLine($"{indent}    pointer += 2;");
                builder.AppendLine($"{indent}    buffer[pointer++] = (Byte)item.Month;");
                builder.AppendLine($"{indent}    buffer[pointer++] = (Byte)item.Day;");
                builder.AppendLine($"{indent}}}");
                break;
            case nameof(DateTime):
            case nameof(DateTimeOffset):
            case "TimeOnly":
            case nameof(TimeSpan):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    Unsafe.As<Byte, Int64>(ref buffer[pointer]) = item.Ticks;");
                builder.AppendLine($"{indent}    pointer += 8;");
                builder.AppendLine($"{indent}}}");
                break;
            case nameof(Guid):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    Unsafe.As<Byte, Guid>(ref buffer[pointer]) = item;");
                builder.AppendLine($"{indent}    pointer += 16;");
                builder.AppendLine($"{indent}}}");
                break;
            case nameof(SByte):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    buffer[pointer++] = (Byte)item;");
                builder.AppendLine($"{indent}}}");
                break;
            case nameof(String):
                builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{enumerableType.EnumerableCount()};");
                builder.AppendLine($"{indent}pointer += 4;");
                builder.AppendLine($"{indent}foreach ({typename} item in value.{target.Name})");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}    pointer += Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy.Serialize(buffer[pointer..], item);");
                builder.AppendLine($"{indent}}}");
                break;
        }
    }
}