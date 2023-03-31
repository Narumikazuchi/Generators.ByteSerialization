namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private void GenerateSerializeMethod(INamedTypeSymbol symbol,
                                                ImmutableArray<IFieldSymbol> fields,
                                                Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                                SemanticModel semanticModel,
                                                StringBuilder builder,
                                                String indent)
    {
        builder.AppendLine($"{indent}[CompilerGenerated]");
        builder.AppendLine($"{indent}[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"{indent}static Int32 Narumikazuchi.Generators.ByteSerialization.IByteSerializable<{symbol.Name}>.Serialize(Span<Byte> buffer, {symbol.Name} value)");
        builder.AppendLine($"{indent}{{");
        indent += "    ";

        builder.AppendLine($"{indent}Int32 pointer = 0;");

        GenerateSerializationBody(fields: fields,
                                  strategies: strategies,
                                  semanticModel: semanticModel,
                                  builder: builder,
                                  indent: indent);

        builder.AppendLine($"{indent}return pointer;");

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static private void GenerateSerializationBody(ImmutableArray<IFieldSymbol> fields,
                                                  Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                                  SemanticModel semanticModel,
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

            if (strategies.TryGetValue(key: field.Type,
                                       value: out ITypeSymbol strategyType))
            {
                builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize<{field.Type.Name}, {strategyType.ToDisplayString()}>(buffer[pointer..], value.{target.Name});");
            }
            else if (Array.IndexOf(array: IntrinsicTypes.SerializedTypes,
                                   value: field.Type.ToTypename()) > -1)
            {
                SerializationHelper.WriteKnownTypeSerialization(field: field,
                                                                target: target,
                                                                builder: builder,
                                                                indent: indent);
            }
            else if (field.Type.IsUnmanagedType &&
                     field.Type.TypeKind is not TypeKind.Pointer &&
                     field.Type.Name is not "IntPtr"
                                     and not "UIntPtr")
            {
                builder.AppendLine($"{indent}Unsafe.As<Byte, {field.Type.ToTypename()}>(ref buffer[pointer]) = value.{target.Name};");
                if (semanticModel.Compilation.Options is CSharpCompilationOptions compilationOptions &&
                    compilationOptions.AllowUnsafe)
                {
                    builder.AppendLine($"{indent}pointer += sizeof({field.Type.ToTypename()});");
                }
                else
                {
                    builder.AppendLine($"{indent}pointer += {field.Type.UnmanagedSize()};");
                }
            }
            else if (field.Type.IsSerializable())
            {
                builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], value.{target.Name});");
            }
            /*
            else if (field.Type.IsEnumerable(out ITypeSymbol elementType))
            {
                if (strategies.TryGetValue(key: field.Type,
                                           value: out strategyType))
                {
                    builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{field.Type.EnumerableCount()};");
                    builder.AppendLine($"{indent}pointer += 4;");
                    builder.AppendLine($"{indent}foreach ({elementType.ToTypename()} item in value.{target.Name})");
                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize<{elementType.ToTypename()}, {strategyType.ToDisplayString()}>(buffer[pointer..], item);");
                    builder.AppendLine($"{indent}}}");
                }
                else if (Array.IndexOf(array: IntrinsicTypes.SerializedTypes,
                                       value: elementType.ToTypename()) > -1)
                {
                    SerializationHelper.WriteEnumerableKnownTypeSerialization(enumerableType: field.Type,
                                                                              elementType: elementType,
                                                                              target: target,
                                                                              builder: builder,
                                                                              indent: indent);
                }
                else if (elementType.IsUnmanagedType)
                {
                    builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{field.Type.EnumerableCount()};");
                    builder.AppendLine($"{indent}pointer += 4;");
                    builder.AppendLine($"{indent}foreach ({elementType.ToTypename()} item in value.{target.Name})");
                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    Unsafe.As<Byte, {field.Type.ToTypename()}>(ref buffer[pointer]) = item;");
                    if (semanticModel.Compilation.Options is CSharpCompilationOptions compilationOptions &&
                        compilationOptions.AllowUnsafe)
                    {
                        builder.AppendLine($"{indent}    pointer += sizeof({field.Type.ToTypename()});");
                    }
                    else
                    {
                        builder.AppendLine($"{indent}    pointer += {field.Type.UnmanagedSize()};");
                    }

                    builder.AppendLine($"{indent}}}");
                }
                else if (elementType.IsSerializable())
                {
                    builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.{target.Name}{field.Type.EnumerableCount()};");
                    builder.AppendLine($"{indent}pointer += 4;");
                    builder.AppendLine($"{indent}foreach ({field.Type.ToTypename()} item in value.{target.Name})");
                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], item);");
                    builder.AppendLine($"{indent}}}");
                }
                else
                {
                    throw new Exception();
                }
            }
            */
            else
            {
                throw new Exception();
            }
        }
    }
}