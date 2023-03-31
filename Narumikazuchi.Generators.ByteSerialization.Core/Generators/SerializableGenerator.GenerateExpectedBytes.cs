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
                                       value: out ITypeSymbol strategyType))
            {
                AttributeData attribute = strategyType.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass is not null &&
                                                                                                   attribute.AttributeClass.ToDisplayString() is FIXEDSERIALIZATIONSIZE_ATTRIBUTE);
                if (attribute is null)
                {
                    sizeBuilder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<{field.Type.Name}, {strategyType.ToDisplayString()}>(value.{target.Name});");
                }
                else
                {
                    expectedSize += (Int32)attribute.ConstructorArguments[0].Value;
                }
            }
            else if (Array.IndexOf(array: IntrinsicTypes.SerializedTypes,
                                   value: field.Type.ToTypename()) > -1)
            {
                SizeHelper.WriteKnownTypeSize(target: target,
                                              builder: sizeBuilder,
                                              typename: field.Type.ToTypename(),
                                              indent: indent,
                                              expectedSize: ref expectedSize);
            }
            else if (field.Type.IsUnmanagedType &&
                     field.Type.TypeKind is not TypeKind.Pointer &&
                     field.Type.Name is not "IntPtr"
                                     and not "UIntPtr")
            {
                if ((semanticModel.Compilation.Options is CSharpCompilationOptions compilationOptions &&
                    compilationOptions.AllowUnsafe) ||
                    field.Type.TypeKind is TypeKind.Enum)
                {
                    sizeBuilder.AppendLine($"{indent}expectedSize += sizeof({field.Type.ToTypename()});");
                }
                else
                {
                    sizeBuilder.AppendLine($"{indent}expectedSize += Marshal.SizeOf<{field.Type.ToTypename()}>();");
                }
            }
            else if (field.Type.IsSerializable())
            {
                sizeBuilder.AppendLine($"{indent}expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize(value.{target.Name});");
            }
            /*
            else if (field.Type.IsEnumerable(out ITypeSymbol elementType))
            {
                if (strategies.TryGetValue(key: elementType,
                                           value: out strategyType))
                {
                    AttributeData attribute = strategyType.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass is not null &&
                                                                                                       attribute.AttributeClass.ToDisplayString() is FIXEDSERIALIZATIONSIZE_ATTRIBUTE);
                    if (attribute is null)
                    {
                        sizeBuilder.AppendLine($"{indent}expectedSize += 4;");
                        sizeBuilder.AppendLine($"{indent}foreach ({elementType.ToTypename()} item in value.{target.Name})");
                        sizeBuilder.AppendLine($"{indent}{{");
                        sizeBuilder.AppendLine($"{indent}    expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<{elementType.ToTypename()}, {strategyType.ToDisplayString()}>(item);");
                        sizeBuilder.AppendLine($"{indent}}}");
                    }
                    else
                    {
                        sizeBuilder.AppendLine($"{indent}expectedSize += 4 + value.{target.Name}{field.Type.EnumerableCount()} * {(Int32)attribute.ConstructorArguments[0].Value};");
                    }
                }
                else if (Array.IndexOf(array: IntrinsicTypes.SerializedTypes,
                                       value: elementType.ToTypename()) > -1)
                {
                    SizeHelper.WriteKnownTypeSizeEnumerable(target: target,
                                                            type: elementType,
                                                            builder: sizeBuilder,
                                                            indent: indent,
                                                            expectedSize: ref expectedSize);
                }
                else if (elementType.IsUnmanagedType)
                {
                    if (semanticModel.Compilation.Options is CSharpCompilationOptions compilationOptions &&
                        compilationOptions.AllowUnsafe)
                    {
                        sizeBuilder.AppendLine($"{indent}expectedSize += 4 + value.{target.Name}{field.Type.EnumerableCount()} * sizeof({elementType.ToTypename()});");
                    }
                    else
                    {
                        sizeBuilder.AppendLine($"{indent}expectedSize += 4 + value.{target.Name}{field.Type.EnumerableCount()} * {elementType.UnmanagedSize()};");
                    }
                }
                else if (elementType.IsSerializable())
                {
                    sizeBuilder.AppendLine($"{indent}expectedSize += 4;");
                    sizeBuilder.AppendLine($"{indent}foreach ({elementType.ToTypename()} item in value.{target.Name})");
                    sizeBuilder.AppendLine($"{indent}{{");
                    sizeBuilder.AppendLine($"{indent}    expectedSize += Narumikazuchi.Generators.ByteSerialization.ByteSerializer.GetExpectedSerializedSize<{field.Type.ToTypename()}>(item);");
                    sizeBuilder.AppendLine($"{indent}}}");
                }
                else if (elementType.IsValueType &&
                         elementType.TypeKind is TypeKind.Enum)
                {
                    EnumDeclarationSyntax syntax = (EnumDeclarationSyntax)field.Type.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntax.BaseList is null)
                    {
                        sizeBuilder.AppendLine($"{indent}expectedSize += 4 + value.{target.Name}{field.Type.EnumerableCount()} * 4;");
                    }
                    else
                    {
                        TypeSyntax baseType = syntax.BaseList.Types[0].Type;
                        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(baseType);
                        ISymbol symbol = symbolInfo.Symbol;
                        if (symbol is ITypeSymbol typeSymbol)
                        {
                            SizeHelper.WriteKnownTypeSizeEnumerable(target: target,
                                                                    builder: sizeBuilder,
                                                                    type: typeSymbol,
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
            */
            else
            {
                throw new Exception();
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