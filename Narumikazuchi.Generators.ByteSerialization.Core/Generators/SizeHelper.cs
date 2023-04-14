namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SizeHelper
{
    static public void GenerateGetExpectedByteSizeMethod(INamedTypeSymbol symbol,
                                                         ImmutableArray<IFieldSymbol> fields,
                                                         StringBuilder builder,
                                                         String indent)
    {
        builder.AppendLine($"{indent}[CompilerGenerated]");
        if (symbol.IsValueType)
        {
            builder.AppendLine($"{indent}Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{symbol.ToFrameworkString()}>.GetExpectedSize({symbol.ToFrameworkString()} value)");
        }
        else
        {
            builder.AppendLine($"{indent}Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{symbol.ToFrameworkString()}>.GetExpectedSize({symbol.ToFrameworkString()}? value)");
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

        GenerateArraySize(fields: fields,
                          builder: builder,
                          indent: indent);

        if (!symbol.IsValueType)
        {
            indent = indent.Substring(4);
            builder.AppendLine($"{indent}}}");
        }

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static public void GenerateArraySize(ImmutableArray<IFieldSymbol> fields,
                                         StringBuilder builder,
                                         String indent)
    {
        Int32 expectedSize = 0;
        StringBuilder sizeBuilder = new();
        foreach (IFieldSymbol field in fields)
        {
            ISymbol target = field;
            if (field.AssociatedSymbol is not null)
            {
                target = field.AssociatedSymbol;
            }

            WriteTypeSize(type: field.Type,
                          builder: sizeBuilder,
                          indent: indent,
                          target: $"value.{target.Name}",
                          expectedSize: ref expectedSize);
        }

        if (sizeBuilder.Length > 0)
        {
            builder.AppendLine($"{indent}Int32 expectedSize = {expectedSize};");
            builder.Append(sizeBuilder.ToString());
            builder.AppendLine($"{indent}return expectedSize;");
        }
        else
        {
            builder.AppendLine($"{indent}return {expectedSize};");
        }
    }

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
            builder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize({target});");
        }
    }

    static public void WriteTypeSize(ITypeSymbol type,
                                     StringBuilder builder,
                                     String indent,
                                     String target,
                                     ref Int32 expectedSize)
    {
        if (Array.IndexOf(array: __Shared.IntrinsicTypes,
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
        else if (type.IsEnumerableSerializable())
        {
            String trimmed = target.Substring(target.LastIndexOf('.') + 1);
            builder.AppendLine($"{indent}expectedSize += 4;");
            if (type.IsDictionaryEnumerable(out INamedTypeSymbol keyValuePair))
            {
                builder.AppendLine($"{indent}foreach ({keyValuePair.ToFrameworkString()} {trimmed}_item in {target})");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeSize(type: keyValuePair.TypeArguments[0],
                              builder: builder,
                              indent: indent,
                              target: $"{trimmed}_item_key",
                              expectedSize: ref expectedSize);
                WriteTypeSize(type: keyValuePair.TypeArguments[1],
                              builder: builder,
                              indent: indent,
                              target: $"{trimmed}_item_value",
                              expectedSize: ref expectedSize);
            }
            else if (type.IsEnumerable(out ITypeSymbol elementType))
            {
                builder.AppendLine($"{indent}foreach ({elementType.ToFrameworkString()} {trimmed}_item in {target})");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeSize(type: elementType,
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
            if (type.IsValueType ||
                type.IsSealed)
            {
                builder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize({target});");
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
                    builder.AppendLine($"{indent}    expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize(({derivedType.ToFrameworkString()}){target});");
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
                    builder.AppendLine($"{indent}    expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize({target});");
                    builder.AppendLine($"{indent}}}");
                }
            }
        }
    }
}