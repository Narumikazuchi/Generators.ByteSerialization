namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SizeHelper
{
    static public void WriteKnownTypeSize(StringBuilder builder,
                                          String typename,
                                          String indent,
                                          String target,
                                          ref Int32 expectedSize)
    {
        if (typename == typeof(DateTime).FullName! ||
            typename == typeof(DateTimeOffset).FullName!)
        {
            expectedSize += 8;
        }
        else if (typename is nameof(String))
        {
            builder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<String, Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy>({target});");
        }
    }

    static public void WriteTypeSize(ITypeSymbol type,
                                     Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                     StringBuilder builder,
                                     String indent,
                                     String target,
                                     ref Int32 expectedSize)
    {
        if (strategies.TryGetValue(key: type,
                                   value: out ITypeSymbol strategyType))
        {
            AttributeData attribute = strategyType.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass is not null &&
                                                                                               attribute.AttributeClass.ToFrameworkString() is SerializableGenerator.FIXEDSERIALIZATIONSIZE_ATTRIBUTE);
            if (attribute is null)
            {
                builder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<{type.Name}, {strategyType.ToFrameworkString()}>({target});");
            }
            else
            {
                expectedSize += (Int32)attribute.ConstructorArguments[0].Value;
            }
        }
        else if (Array.IndexOf(array: __Shared.IntrinsicTypes,
                               value: type.ToFrameworkString()) > -1)
        {
            WriteKnownTypeSize(target: target,
                               builder: builder,
                               typename: type.ToFrameworkString(),
                               indent: indent,
                               expectedSize: ref expectedSize);
        }
        else if (type.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}expectedSize += {__Shared.SizeOf(type)};");
        }
        else if (type.IsByteSerializable())
        {
            builder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize({target});");
        }
        else if (type.IsEnumerableSerializable(strategies))
        {
            String trimmed = target.Substring(target.LastIndexOf('.') + 1);
            builder.AppendLine($"{indent}expectedSize += 4;");
            if (type.IsDictionaryEnumerable(out INamedTypeSymbol keyValuePair))
            {
                builder.AppendLine($"{indent}foreach ({keyValuePair.ToFrameworkString()} {trimmed}_item in {target})");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeSize(type: keyValuePair.TypeArguments[0],
                              strategies: strategies,
                              builder: builder,
                              indent: indent,
                              target: $"{trimmed}_item_key",
                              expectedSize: ref expectedSize);
                WriteTypeSize(type: keyValuePair.TypeArguments[1],
                              strategies: strategies,
                              builder: builder,
                              indent: indent,
                              target: $"{trimmed}_item_value",
                              expectedSize: ref expectedSize);
            }
            else if (type.IsEnumerable(out INamedTypeSymbol elementType))
            {
                builder.AppendLine($"{indent}foreach ({elementType.ToFrameworkString()} {trimmed}_item in {target})");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeSize(type: elementType,
                              strategies: strategies,
                              builder: builder,
                              indent: indent,
                              target: $"{trimmed}_item",
                              expectedSize: ref expectedSize);
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