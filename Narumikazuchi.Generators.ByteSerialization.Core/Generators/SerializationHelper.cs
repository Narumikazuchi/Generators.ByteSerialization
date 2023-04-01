namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SerializationHelper
{
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
            builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy.Serialize(buffer[pointer..], {target});");
        }
    }

    static public void WriteTypeSerialization(ITypeSymbol type,
                                              Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                              StringBuilder builder,
                                              String indent,
                                              String target)
    {
        if (strategies.TryGetValue(key: type,
                                   value: out ITypeSymbol strategyType))
        {
            builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize<{type.ToFrameworkString()}, {strategyType.ToFrameworkString()}>(buffer[pointer..], {target});");
        }
        else if (Array.IndexOf(array: __Shared.IntrinsicTypes,
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
        else if (type.IsByteSerializable())
        {
            builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], {target});");
        }
        else if (type.IsEnumerableSerializable(strategies))
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
                                       strategies: strategies,
                                       builder: builder,
                                       indent: indent,
                                       target: $"{trimmed}_item.Key");
                WriteTypeSerialization(type: keyValuePair.TypeArguments[1],
                                       strategies: strategies,
                                       builder: builder,
                                       indent: indent,
                                       target: $"{trimmed}_item.Value");
            }
            else if (type.IsEnumerable(out INamedTypeSymbol elementType))
            {
                builder.AppendLine($"{indent}foreach ({elementType.ToFrameworkString()} {trimmed}_item in {target})");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeSerialization(type: elementType,
                                       strategies: strategies,
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
            throw new Exception();
        }
    }
}