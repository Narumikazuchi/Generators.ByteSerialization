namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class DeserializationHelper
{
    static public void WriteKnownTypeDeserialization(ITypeSymbol type,
                                                     StringBuilder builder,
                                                     String indent,
                                                     String target)
    {
        String typename = type.ToFrameworkString();
        if (typename == typeof(DateTime).FullName!)
        {
            builder.AppendLine($"{indent}{typename} _{target} = new {typename}(Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer[read..])));");
            builder.AppendLine($"{indent}read += 8;");
        }
        else if (typename == typeof(DateTimeOffset).FullName!)
        {
            builder.AppendLine($"{indent}{typename} _{target} = new {typename}(Unsafe.ReadUnaligned<Int64>(ref MemoryMarshal.GetReference(buffer[read..])), TimeSpan.Zero);");
            builder.AppendLine($"{indent}read += 8;");
        }
        else if (typename is nameof(String))
        {
            builder.AppendLine($"{indent}String _{target} = Narumikazuchi.Generators.ByteSerialization.Strategies.StringStrategy.Deserialize(buffer[read..], out bytesRead);");
            builder.AppendLine($"{indent}read += bytesRead;");
        }
    }

    static public void WriteTypeDeserialization(ITypeSymbol type,
                                                Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                                StringBuilder builder,
                                                String indent,
                                                String target,
                                                ref Boolean first,
                                                StringBuilder constructBuilder = default)
    {
        if (strategies.TryGetValue(key: type,
                                   value: out ITypeSymbol strategyType))
        {
            builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize<{type.ToFrameworkString()}, {strategyType.ToFrameworkString()}>(buffer[read..], out {type.ToFrameworkString()} _{target});");
            
            if (constructBuilder is null)
            {
                return;
            }

            if (first)
            {
                first = false;
            }
            else
            {
                constructBuilder.Append(", ");
            }

            constructBuilder.Append($"_{target}");
        }
        else if (Array.IndexOf(array: __Shared.IntrinsicTypes,
                               value: type.ToFrameworkString()) > -1)
        {
            WriteKnownTypeDeserialization(type: type,
                                          builder: builder,
                                          indent: indent,
                                          target: target);

            if (constructBuilder is null)
            {
                return;
            }

            if (first)
            {
                first = false;
            }
            else
            {
                constructBuilder.Append(", ");
            }

            constructBuilder.Append($"_{target}");
        }
        else if (type.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}{type.ToFrameworkString()} _{target} = Unsafe.As<Byte, {type.ToFrameworkString()}>(ref MemoryMarshal.GetReference(buffer[read..]));");
            builder.AppendLine($"{indent}read += {__Shared.SizeOf(type)};");

            if (constructBuilder is null)
            {
                return;
            }

            if (first)
            {
                first = false;
            }
            else
            {
                constructBuilder.Append(", ");
            }

            constructBuilder.Append($"_{target}");
        }
        else if (type.IsByteSerializable())
        {
            builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize(buffer[read..], out {type.ToFrameworkString()} _{target});");

            if (constructBuilder is null)
            {
                return;
            }

            if (first)
            {
                first = false;
            }
            else
            {
                constructBuilder.Append(", ");
            }

            constructBuilder.Append($"_{target}");
        }
        else if (type.IsEnumerableSerializable(strategies))
        {
            Boolean immutable;
            builder.AppendLine($"{indent}Int32 {target}_count = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[read..]));");
            builder.AppendLine($"{indent}read += 4;");
            if (type.IsDictionaryEnumerable(out INamedTypeSymbol keyValuePair))
            {
                immutable = WriteEnumerableBuilderType(type: type,
                                                       keyType: keyValuePair.TypeArguments[0],
                                                       valueType: keyValuePair.TypeArguments[1],
                                                       builder: builder,
                                                       indent: indent,
                                                       target: target);
                builder.AppendLine($"{indent}for (Int32 {target}_index = 0; {target}_index < {target}_count; {target}_index++)");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeDeserialization(type: keyValuePair.TypeArguments[0],
                                         strategies: strategies,
                                         builder: builder,
                                         indent: indent,
                                         target: $"{target}_item_key",
                                         first: ref first);
                WriteTypeDeserialization(type: keyValuePair.TypeArguments[1],
                                         strategies: strategies,
                                         builder: builder,
                                         indent: indent,
                                         target: $"{target}_item_value",
                                         first: ref first);

                if (immutable)
                {
                    builder.AppendLine($"{indent}{target}_builder.Add(_{target}_item_key, _{target}_item_value);");
                }
                else
                {
                    builder.AppendLine($"{indent}_{target}.Add(_{target}_item_key, _{target}_item_value);");
                }
            }
            else if (type.IsEnumerable(out INamedTypeSymbol elementType))
            {
                immutable = WriteEnumerableBuilderType(type: type,
                                                       elementType: elementType,
                                                       builder: builder,
                                                       indent: indent,
                                                       target: target);
                builder.AppendLine($"{indent}for (Int32 {target}_index = 0; {target}_index < {target}_count; {target}_index++)");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
                WriteTypeDeserialization(type: elementType,
                                         strategies: strategies,
                                         builder: builder,
                                         indent: indent,
                                         target: $"{target}_item",
                                         first: ref first);

                if (type is IArrayTypeSymbol)
                {
                    builder.AppendLine($"{indent}_{target}[{target}_index] = _{target}_item;");
                }
                else if (immutable)
                {
                    builder.AppendLine($"{indent}{target}_builder.Add(_{target}_item);");
                }
                else
                {
                    builder.AppendLine($"{indent}_{target}.Add(_{target}_item);");
                }
            }
            else
            {
                throw new Exception();
            }

            indent = indent.Substring(4);
            builder.AppendLine($"{indent}}}");
            if (immutable)
            {
                builder.AppendLine($"{indent}{type.ToFrameworkString()} _{target} = {target}_builder.ToImmutable();");
            }

            if (constructBuilder is null)
            {
                return;
            }

            if (first)
            {
                first = false;
            }
            else
            {
                constructBuilder.Append(", ");
            }

            constructBuilder.Append($"_{target}");
        }
        else
        {
            throw new Exception();
        }
    }

    static private Boolean WriteEnumerableBuilderType(ITypeSymbol type,
                                                      INamedTypeSymbol elementType,
                                                      StringBuilder builder,
                                                      String indent,
                                                      String target)
    {
        String typename = type.ToFrameworkString();
        if (type is IArrayTypeSymbol)
        {
            builder.AppendLine($"{indent}{typename} _{target} = new {typename.Substring(0, typename.IndexOf('['))}[{target}_count];");
            return false;
        }
        else if (typename.StartsWith("System.Collections.Immutable"))
        {
            builder.AppendLine($"{indent}{typename}.Builder {target}_builder = {typename.Substring(0, typename.IndexOf('<'))}.CreateBuilder<{elementType.ToFrameworkString()}>();");
            return true;
        }
        else
        {
            builder.AppendLine($"{indent}{typename} _{target} = new {typename}();");
            return false;
        }
    }

    static private Boolean WriteEnumerableBuilderType(ITypeSymbol type,
                                                      ITypeSymbol keyType,
                                                      ITypeSymbol valueType,
                                                      StringBuilder builder,
                                                      String indent,
                                                      String target)
    {
        String typename = type.ToFrameworkString();
        if (typename.StartsWith("System.Collections.Immutable"))
        {
            builder.AppendLine($"{indent}{typename}.Builder {target}_builder = {typename.Substring(0, typename.IndexOf('<'))}.CreateBuilder<{keyType.ToFrameworkString()}, {valueType.ToFrameworkString()}>();");
            return true;
        }
        else
        {
            builder.AppendLine($"{indent}{typename} _{target} = new {typename}();");
            return false;
        }
    }
}