namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private void GenerateGetExpectedByteSizeMethod(INamedTypeSymbol symbol,
                                                          ImmutableArray<IFieldSymbol> fields,
                                                          Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                                          SemanticModel semanticModel,
                                                          StringBuilder builder,
                                                          String indent)
    {
        builder.AppendLine($"{indent}[CompilerGenerated]");
        builder.AppendLine($"{indent}[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"{indent}static Int32 Narumikazuchi.Generators.ByteSerialization.IByteSerializable<{symbol.Name}>.GetExpectedByteSize({symbol.Name} value)");
        builder.AppendLine($"{indent}{{");
        indent += "    ";

        GenerateArraySize(fields: fields,
                          strategies: strategies,
                          semanticModel: semanticModel,
                          builder: builder,
                          indent: indent);

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static private void GenerateArraySize(ImmutableArray<IFieldSymbol> fields,
                                          Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                          SemanticModel semanticModel,
                                          StringBuilder builder,
                                          String indent)
    {
        Int32 expectedSize = 4;
        StringBuilder sizeBuilder = new();
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
                AttributeData? attribute = strategyType.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass is not null &&
                                                                                                    attribute.AttributeClass.ToDisplayString() is "Narumikazuchi.Generators.ByteSerialization.FixedSerializationSizeAttribute");
                if (attribute is null)
                {
                    sizeBuilder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<{field.Type.Name}, {strategyType.ToDisplayString()}>(value.{target.Name});");
                }
                else
                {
                    expectedSize += (Int32)attribute.ConstructorArguments[0].Value!;
                }
            }
            else if (Array.IndexOf(array: s_KnownTypes,
                                   value: field.Type.ToTypename()) > -1)
            {
                SizeHelper.WriteKnownTypeSize(field: field,
                                              target: target,
                                              builder: sizeBuilder,
                                              indent: indent,
                                              expectedSize: ref expectedSize);
            }
            else
            {
                Boolean isSerializable = field.Type.GetAttributes().Any(attribute => attribute.AttributeClass is not null &&
                                                                                     attribute.AttributeClass.ToDisplayString() is BYTESERIALIZABLE_ATTRIBUTE);
                if (isSerializable)
                {
                    sizeBuilder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize(value.{target.Name});");
                }
                else if (field.Type.IsValueType &&
                         field.Type.TypeKind is TypeKind.Enum)
                {
                    EnumDeclarationSyntax syntax = (EnumDeclarationSyntax)field.Type.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntax.BaseList is null)
                    {
                        expectedSize += sizeof(Int32);
                    }
                    else
                    {
                        TypeSyntax baseType = syntax.BaseList.Types[0].Type;
                        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(baseType);
                        ISymbol? symbol = symbolInfo.Symbol;
                        if (symbol is ITypeSymbol typeSymbol)
                        {
                            SizeHelper.WriteKnownTypeSize(target: target,
                                                          builder: sizeBuilder,
                                                          typename: typeSymbol.ToTypename(),
                                                          indent: indent,
                                                          expectedSize: ref expectedSize);
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
}