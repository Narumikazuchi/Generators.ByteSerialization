namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class DeserializationHelper
{
    static public void GenerateDeserializeMethod(INamedTypeSymbol symbol,
                                                 ImmutableArray<IFieldSymbol> fields,
                                                 StringBuilder builder,
                                                 String indent)
    {
        builder.AppendLine($"{indent}[CompilerGenerated]");
        if (symbol.IsValueType)
        {
            builder.AppendLine($"{indent}Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{symbol.ToFrameworkString()}>.Deserialize(ReadOnlySpan<Byte> buffer, out {symbol.ToFrameworkString()} result)");
        }
        else
        {
            builder.AppendLine($"{indent}Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{symbol.ToFrameworkString()}>.Deserialize(ReadOnlySpan<Byte> buffer, out {symbol.ToFrameworkString()}? result)");
        }

        builder.AppendLine($"{indent}{{");
        indent += "    ";

        builder.AppendLine($"{indent}Int32 read = 0;");
        builder.AppendLine($"{indent}Int32 bytesRead;");
        builder.AppendLine($"{indent}Guid typeId;");

        GenerateDeserializationBody(symbol: symbol,
                                    fields: fields,
                                    builder: builder,
                                    indent: indent);

        builder.AppendLine($"{indent}return read;");

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static public void GenerateDeserializationBody(INamedTypeSymbol symbol,
                                                   ImmutableArray<IFieldSymbol> fields,
                                                   StringBuilder builder,
                                                   String indent)
    {
        StringBuilder constructBuilder = new();
        constructBuilder.Append($"{indent}result = Narumikazuchi.Generated.__Internal_ConstructorGenerator.ConstructorFor_{symbol.ToNameString()}.Invoke(");

        Boolean first = true;
        foreach (IFieldSymbol field in fields)
        {
            ISymbol target = field;
            if (field.AssociatedSymbol is not null)
            {
                target = field.AssociatedSymbol;
            }

            WriteTypeDeserialization(type: field.Type,
                                     builder: builder,
                                     constructBuilder: constructBuilder,
                                     indent: indent,
                                     target: target.Name,
                                     first: ref first);
        }

        constructBuilder.AppendLine(");");
        builder.Append(constructBuilder.ToString());
    }

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
            builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize(buffer[read..], out String? _{target});");
        }
    }

    /**
     * Just copy the memory for arrays of unmanaged types
     * MemoryMarshal.Cast<byte, Vector3>(buffer).CopyTo(dest);
     */

    static public void WriteTypeDeserialization(ITypeSymbol type,
                                                StringBuilder builder,
                                                String indent,
                                                String target,
                                                ref Boolean first,
                                                StringBuilder constructBuilder = default)
    {
        if (Array.IndexOf(array: __Shared.IntrinsicTypes,
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
        else
        {
            if (type.IsValueType ||
                type.IsSealed)
            {
                builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize(buffer[read..], out {type.ToFrameworkString()} _{target});");
            }
            else
            {
                builder.AppendLine($"{indent}typeId = Unsafe.As<Byte, Guid>(ref MemoryMarshal.GetReference(buffer[(read + 4)..]));");
                builder.AppendLine($"{indent}{type.ToFrameworkString()} _{target};");
                Boolean firstType = true;
                foreach (ITypeSymbol derivedType in type.GetDerivedTypes())
                {
                    if (firstType)
                    {
                        firstType = false;
                        builder.AppendLine($"{indent}if (typeId == typeof({derivedType.ToFrameworkString()}).GUID)");
                    }
                    else
                    {
                        builder.AppendLine($"{indent}else if (typeId == typeof({derivedType.ToFrameworkString()}).GUID)");
                    }

                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize(buffer[read..], out {derivedType.ToFrameworkString()} _{derivedType.ToNameString()});");
                    builder.AppendLine($"{indent}    _{target} = _{derivedType.ToNameString()};");
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
                    builder.AppendLine($"{indent}    read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize(buffer[read..], out _{target});");
                    builder.AppendLine($"{indent}}}");
                }
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
    }
}