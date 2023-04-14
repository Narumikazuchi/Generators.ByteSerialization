﻿using System.Reflection;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SerializationHelper
{
    static public void GenerateSerializeMethod(INamedTypeSymbol symbol,
                                               ImmutableArray<IFieldSymbol> fields,
                                               StringBuilder builder,
                                               String indent)
    {
        builder.AppendLine($"{indent}[CompilerGenerated]");
        if (symbol.IsValueType)
        {
            builder.AppendLine($"{indent}Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{symbol.ToFrameworkString()}>.Serialize(Span<Byte> buffer, {symbol.ToFrameworkString()} value)");
        }
        else
        {
            builder.AppendLine($"{indent}Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{symbol.ToFrameworkString()}>.Serialize(Span<Byte> buffer, {symbol.ToFrameworkString()}? value)");
        }

        builder.AppendLine($"{indent}{{");
        indent += "    ";

        if (!symbol.IsValueType)
        {
            builder.AppendLine($"{indent}if (value is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    return 0;");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            indent += "    ";
        }

        builder.AppendLine($"{indent}Int32 pointer = 0;");

        GenerateSerializationBody(fields: fields,
                                  builder: builder,
                                  indent: indent);

        builder.AppendLine($"{indent}return pointer;");

        if (!symbol.IsValueType)
        {
            indent = indent.Substring(4);
            builder.AppendLine($"{indent}}}");
        }

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static public void GenerateSerializationBody(ImmutableArray<IFieldSymbol> fields,
                                                 StringBuilder builder,
                                                 String indent)
    {
        foreach (IFieldSymbol field in fields)
        {
            ISymbol target = field;
            if (field.AssociatedSymbol is not null)
            {
                target = field.AssociatedSymbol;
            }

            WriteTypeSerialization(type: field.Type,
                                   builder: builder,
                                   indent: indent,
                                   target: $"value.{target.Name}");
        }
    }

    static public void WriteKnownTypeSerialization(ITypeSymbol type,
                                                   StringBuilder builder,
                                                   String indent,
                                                   String target)
    {
        String typename = type.ToFrameworkString();
        if (typename == typeof(DateTime).FullName! ||
            typename == typeof(DateTimeOffset).FullName!)
        {
            builder.AppendLine($"{indent}Unsafe.As<Byte, Int64>(ref buffer[pointer]) = {target}.Ticks;");
            builder.AppendLine($"{indent}pointer += 8;");
        }
        else if (typename is nameof(String))
        {
            builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], {target});");
        }
    }

    /**
     * Just copy the memory for arrays of unmanaged types
     * MemoryMarshal.AsBytes(value.AsSpan()).CopyTo(buffer.AsSpan(0, offset))
     */

    static public void WriteTypeSerialization(ITypeSymbol type,
                                              StringBuilder builder,
                                              String indent,
                                              String target)
    {
        if (Array.IndexOf(array: __Shared.IntrinsicTypes,
                          value: type.ToFrameworkString()) > -1)
        {
            WriteKnownTypeSerialization(type: type,
                                        builder: builder,
                                        indent: indent,
                                        target: target);
        }
        else if (type.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}Unsafe.As<Byte, {type.ToFrameworkString()}>(ref buffer[pointer]) = {target};");
            builder.AppendLine($"{indent}pointer += {__Shared.SizeOf(type)};");
        }
        else if (type.IsEnumerableSerializable())
        {
            String trimmed = target.Substring(target.LastIndexOf('.') + 1);
            builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = {target}{type.EnumerableCount()};");
            builder.AppendLine($"{indent}pointer += 4;");
            if (type.IsDictionaryEnumerable(out INamedTypeSymbol keyValuePair))
            {
                builder.AppendLine($"{indent}foreach ({keyValuePair.ToFrameworkString()} {trimmed}_item in {target})");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeSerialization(type: keyValuePair.TypeArguments[0],
                                       builder: builder,
                                       indent: indent,
                                       target: $"{trimmed}_item.Key");
                WriteTypeSerialization(type: keyValuePair.TypeArguments[1],
                                       builder: builder,
                                       indent: indent,
                                       target: $"{trimmed}_item.Value");
            }
            else if (type.IsEnumerable(out ITypeSymbol elementType))
            {
                builder.AppendLine($"{indent}foreach ({elementType.ToFrameworkString()} {trimmed}_item in {target})");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeSerialization(type: elementType,
                                       builder: builder,
                                       indent: indent,
                                       target: $"{trimmed}_item");
            }
            else
            {
                throw new Exception();
            }

            indent = indent.Substring(4);
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            if (type.IsValueType ||
                type.IsSealed)
            {
                builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], {target});");
            }
            else
            {
                Boolean first = true;
                foreach (ITypeSymbol derivedType in type.GetDerivedTypes())
                {
                    if (first)
                    {
                        first = false;
                        builder.AppendLine($"{indent}if ({target} is {derivedType.ToFrameworkString()} _{derivedType.ToNameString()})");
                    }
                    else
                    {
                        builder.AppendLine($"{indent}else if ({target} is {derivedType.ToFrameworkString()} _{derivedType.ToNameString()})");
                    }

                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], ({derivedType.ToFrameworkString()}){target});");
                    builder.AppendLine($"{indent}}}");
                }

                if (type.IsAbstract)
                {
                    builder.AppendLine($"{indent}else");
                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    throw new Exception();");
                    builder.AppendLine($"{indent}}}");
                }
                else
                {
                    builder.AppendLine($"{indent}else");
                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], {target});");
                    builder.AppendLine($"{indent}}}");
                }
            }
        }
    }
}