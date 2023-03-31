namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private void GenerateDeserializeMethod(INamedTypeSymbol symbol,
                                                  ImmutableArray<IFieldSymbol> fields,
                                                  Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                                  SemanticModel semanticModel,
                                                  StringBuilder builder,
                                                  String indent)
    {
        builder.AppendLine($"{indent}[CompilerGenerated]");
        builder.AppendLine($"{indent}[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"{indent}static {symbol.Name} Narumikazuchi.Generators.ByteSerialization.IByteSerializable<{symbol.Name}>.Deserialize(ReadOnlySpan<Byte> buffer, out Int32 read)");
        builder.AppendLine($"{indent}{{");
        indent += "    ";

        builder.AppendLine($"{indent}read = 0;");
        builder.AppendLine($"{indent}Int32 bytesRead;");
        builder.AppendLine($"{indent}Int32 count;");

        GenerateDeserializationBody(symbol: symbol,
                                    fields: fields,
                                    strategies: strategies,
                                    semanticModel: semanticModel,
                                    builder: builder,
                                    indent: indent);

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static private void GenerateDeserializationBody(INamedTypeSymbol symbol,
                                                    ImmutableArray<IFieldSymbol> fields,
                                                    Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                                    SemanticModel semanticModel,
                                                    StringBuilder builder,
                                                    String indent)
    {
        StringBuilder constructBuilder = new();
        constructBuilder.Append($"{indent}return Narumikazuchi.Generated.ConstructorGenerator.ConstructorFor_{symbol.ToDisplayString().Replace(".", "")}.Invoke(");

        Boolean first = true;
        foreach (IFieldSymbol field in fields)
        {
            ISymbol target = field;
            if (field.AssociatedSymbol is not null)
            {
                target = field.AssociatedSymbol;
            }

            if (strategies.TryGetValue(key: field.Type,
                                       value: out ITypeSymbol strategyType))
            {
                builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize<{field.Type.ToTypename()}, {strategyType.ToDisplayString()}>(buffer[read..], out {field.Type.ToTypename()} _{target.Name});");
                if (first)
                {
                    first = false;
                }
                else
                {
                    constructBuilder.Append(", ");
                }

                constructBuilder.Append($"_{target.Name}");
            }
            else if (Array.IndexOf(array: IntrinsicTypes.SerializedTypes,
                                   value: field.Type.ToTypename()) > -1)
            {
                DeserializationHelper.WriteKnownTypeDeserialization(field: field,
                                                                    target: target,
                                                                    builder: builder,
                                                                    indent: indent);
                if (first)
                {
                    first = false;
                }
                else
                {
                    constructBuilder.Append(", ");
                }

                constructBuilder.Append($"_{target.Name}");
            }
            else if (field.Type.IsUnmanagedType &&
                     field.Type.TypeKind is not TypeKind.Pointer &&
                     field.Type.Name is not "IntPtr"
                                     and not "UIntPtr")
            {
                builder.AppendLine($"{indent}{field.Type.ToTypename()} _{target.Name} = Unsafe.As<Byte, {field.Type.ToTypename()}>(ref MemoryMarshal.GetReference(buffer[read..]));");
                if (semanticModel.Compilation.Options is CSharpCompilationOptions compilationOptions &&
                    compilationOptions.AllowUnsafe)
                {
                    builder.AppendLine($"{indent}read += sizeof({field.Type.ToTypename()});");
                }
                else
                {
                    builder.AppendLine($"{indent}read += {field.Type.UnmanagedSize()};");
                }

                if (first)
                {
                    first = false;
                }
                else
                {
                    constructBuilder.Append(", ");
                }

                constructBuilder.Append($"_{target.Name}");
            }
            else if (field.Type.IsSerializable())
            {
                builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize<{field.Type.ToTypename()}>(buffer[read..], out {field.Type.ToTypename()} _{target.Name});");

                if (first)
                {
                    first = false;
                }
                else
                {
                    constructBuilder.Append(", ");
                }

                constructBuilder.Append($"_{target.Name}");
            }
            /*
            else if (field.Type.IsEnumerable(out ITypeSymbol elementType))
            {
                builder.AppendLine($"{indent}count = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[read..]));");
                builder.AppendLine($"{indent}read += 4;");
                builder.AppendLine($"{indent}for (Int32 index = 0; index < count; index++)");
                builder.AppendLine($"{indent}{{");

                if (strategies.TryGetValue(key: elementType,
                                           value: out strategyType))
                {
                    builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize<{field.Type.ToTypename()}, {strategyType.ToDisplayString()}>(buffer[read..], out {field.Type.ToTypename()} _{target.Name});");
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        constructBuilder.Append(", ");
                    }

                    constructBuilder.Append($"_{target.Name}");
                }
                else if (Array.IndexOf(array: IntrinsicTypes.SerializedTypes,
                                       value: field.Type.ToTypename()) > -1)
                {
                    DeserializationHelper.WriteKnownTypeDeserialization(field: field,
                                                                        target: target,
                                                                        builder: builder,
                                                                        indent: indent);
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        constructBuilder.Append(", ");
                    }

                    constructBuilder.Append($"_{target.Name}");
                }
                else if (field.Type.IsUnmanagedType)
                {
                    builder.AppendLine($"{indent}{field.Type.ToTypename()} _{target.Name} = Unsafe.As<Byte, {field.Type.ToTypename()}>(ref MemoryMarshal.GetReference(buffer[read..]));");
                    if (semanticModel.Compilation.Options is CSharpCompilationOptions compilationOptions &&
                        compilationOptions.AllowUnsafe)
                    {
                        builder.AppendLine($"{indent}read += sizeof({field.Type.ToTypename()});");
                    }
                    else
                    {
                        builder.AppendLine($"{indent}read += {field.Type.UnmanagedSize()};");
                    }

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        constructBuilder.Append(", ");
                    }

                    constructBuilder.Append($"_{target.Name}");
                }
                else if (field.Type.IsSerializable())
                {
                    builder.AppendLine($"{indent}read += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize<{field.Type.ToTypename()}>(buffer[read..], out {field.Type.ToTypename()} _{target.Name});");

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        constructBuilder.Append(", ");
                    }

                    constructBuilder.Append($"_{target.Name}");
                }
                else
                {
                    throw new Exception();
                }

                builder.AppendLine($"{indent}}}");
            }
                */
            else
            {
                throw new Exception();
            }
        }

        constructBuilder.AppendLine(");");
        builder.Append(constructBuilder.ToString());
    }
}