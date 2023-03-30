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
                                       value: out ITypeSymbol? strategyType))
            {
                builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize<{field.Type.Name}, {strategyType.ToDisplayString()}>(buffer[pointer..], value.{target.Name});");
            }
            else if (Array.IndexOf(array: s_KnownTypes,
                                   value: field.Type.ToTypename()) > -1)
            {
                SerializationHelper.WriteKnownTypeSerialization(field: field,
                                                                target: target,
                                                                builder: builder,
                                                                indent: indent);
            }
            else
            {
                Boolean isSerializable = field.Type.GetAttributes().Any(attribute => attribute.AttributeClass is not null &&
                                                                                     attribute.AttributeClass.ToDisplayString() is BYTESERIALIZABLE_ATTRIBUTE);
                if (isSerializable)
                {
                    builder.AppendLine($"{indent}pointer += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.Serialize(buffer[pointer..], value.{target.Name});");
                }
                else if (field.Type.IsValueType &&
                         field.Type.TypeKind is TypeKind.Enum)
                {
                    EnumDeclarationSyntax syntax = (EnumDeclarationSyntax)field.Type.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntax.BaseList is null)
                    {
                        builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..])) = (Int32)value.{target.Name};");
                        builder.AppendLine($"{indent}pointer += 4;");
                    }
                    else
                    {
                        TypeSyntax baseType = syntax.BaseList.Types[0].Type;
                        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(baseType);
                        ISymbol? symbol = symbolInfo.Symbol;
                        if (symbol is ITypeSymbol typeSymbol)
                        {
                            SerializationHelper.WriteEnumTypeSerialization(field: field,
                                                                           target: target,
                                                                           baseType: typeSymbol,
                                                                           builder: builder,
                                                                           indent: indent);
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
    }
}