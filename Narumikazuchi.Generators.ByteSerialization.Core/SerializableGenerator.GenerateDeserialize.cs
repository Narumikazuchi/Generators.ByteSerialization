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
        builder.AppendLine($"{indent}[EditorBrowsable(EditorBrowsableState.Never)]");
        builder.AppendLine($"{indent}[CompilerGenerated]");
        builder.AppendLine($"{indent}[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"{indent}static {symbol.Name} Narumikazuchi.Generators.ByteSerialization.IByteSerializable<{symbol.Name}>.Deserialize(ReadOnlySpan<Byte> buffer, out Int32 read)");
        builder.AppendLine($"{indent}{{");
        indent += "    ";

        builder.AppendLine($"{indent}read = 0;");
        builder.AppendLine($"{indent}Int32 bytesRead;");

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
                                       value: out ITypeSymbol? strategyType))
            {
                if (field.Type.ContainingNamespace.ToDisplayString() is "System")
                {
                    builder.Append($"{indent}{field.Type.Name} ");
                }
                else
                {
                    builder.Append($"{indent}{field.Type.ToDisplayString()} ");
                }

                builder.AppendLine($"_{target.Name} = Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize<{field.Type.Name}, {strategyType.ToDisplayString()}>(buffer[read..], out bytesRead);");
                builder.AppendLine($"{indent}read += bytesRead;");
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
            else if (Array.IndexOf(array: s_KnownTypes,
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
            else
            {
                Boolean isSerializable = field.Type.GetAttributes().Any(attribute => attribute.AttributeClass is not null &&
                                                                                     attribute.AttributeClass.ToDisplayString() is BYTESERIALIZABLE_ATTRIBUTE);
                if (isSerializable)
                {
                    builder.AppendLine($"{indent}{field.Type.ToDisplayString()} _{target.Name} = Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Deserialize<{field.Type.ToDisplayString()}>(buffer[read..], out bytesRead);");
                    builder.AppendLine($"{indent}read += bytesRead;");
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
                else if (field.Type.IsValueType &&
                         field.Type.TypeKind is TypeKind.Enum)
                {
                    EnumDeclarationSyntax syntax = (EnumDeclarationSyntax)field.Type.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntax.BaseList is null)
                    {
                        builder.AppendLine($"{indent}{field.Type.ToDisplayString()} _{target.Name} = ({field.Type.ToDisplayString()})Unsafe.ReadUnaligned<Int32>(ref MemoryMarshal.GetReference(buffer[read..]));");
                        builder.AppendLine($"{indent}read += 4;");
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
                        TypeSyntax baseType = syntax.BaseList.Types[0].Type;
                        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(baseType);
                        ISymbol? baseSymbol = symbolInfo.Symbol;
                        if (baseSymbol is ITypeSymbol typeSymbol)
                        {
                            DeserializationHelper.WriteEnumTypeDeserialization(field: field,
                                                                               target: target,
                                                                               baseType: typeSymbol,
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
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        constructBuilder.AppendLine(");");
        builder.Append(constructBuilder.ToString());
    }
}