﻿namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class DeserializationHelper
{
    static public void WriteKnownTypeDeserialization(IFieldSymbol field,
                                                     ISymbol target,
                                                     StringBuilder builder,
                                                     String indent)
    {
        String typename = field.Type.ToTypename();
        switch (typename)
        {
            case nameof(Boolean):
                builder.AppendLine($"{indent}Boolean _{target.Name} = buffer[read++] != 0x0;");
                break;
            case nameof(Byte):
                builder.AppendLine($"{indent}Byte _{target.Name} = buffer[read++];");
                break;
            case nameof(Char):
            case nameof(Decimal):
            case nameof(Double):
            case nameof(Int16):
            case nameof(Int32):
            case nameof(Int64):
            case nameof(Single):
            case nameof(UInt16):
            case nameof(UInt32):
            case nameof(UInt64):
                builder.AppendLine($"{indent}{typename} _{target.Name} = Unsafe.ReadUnaligned<{typename}>(ref MemoryMarshal.GetReference(buffer[read..]));");
                builder.AppendLine($"{indent}read += sizeof({typename});");
                break;
            case "DateOnly":
                builder.AppendLine($"{indent}DateOnly _{target.Name} = new DateOnly(Unsafe.ReadUnaligned<Int16>(ref MemoryMarshal.GetReference(buffer[read..])), buffer[read + 2], buffer[read + 3]);");
                builder.AppendLine($"{indent}read += 4;");
                break;
            case nameof(DateTime):
            case "TimeOnly":
            case nameof(TimeSpan):
                builder.AppendLine($"{indent}{typename} _{target.Name} = new {typename}(Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer[read..])));");
                builder.AppendLine($"{indent}read += 8;");
                break;
            case nameof(DateTimeOffset):
                builder.AppendLine($"{indent}{typename} _{target.Name} = new {typename}(Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer[read..])), TimeSpan.Zero);");
                builder.AppendLine($"{indent}read += 8;");
                break;
            case nameof(Guid):
                builder.AppendLine($"{indent}Guid _{target.Name} = new Guid(buffer[read..(read + 16)]);");
                builder.AppendLine($"{indent}read += 16;");
                break;
            case "Half":
                builder.AppendLine($"{indent}{typename} _{target.Name} = Unsafe.ReadUnaligned<{typename}>(ref MemoryMarshal.GetReference(buffer[read..]));");
                builder.AppendLine($"{indent}read += Marshal.SizeOf<{typename}>();");
                break;
            case nameof(SByte):
                builder.AppendLine($"{indent}SByte _{target.Name} = (SByte)buffer[read++];");
                break;
            case nameof(String):
                builder.AppendLine($"{indent}String _{target.Name} = Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy.Deserialize(buffer[read..], out bytesRead);");
                builder.AppendLine($"{indent}read += bytesRead;");
                break;
        }
    }
}