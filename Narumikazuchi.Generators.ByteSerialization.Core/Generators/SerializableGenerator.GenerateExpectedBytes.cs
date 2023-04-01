namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private void GenerateGetExpectedByteSizeMethod(INamedTypeSymbol symbol,
                                                          ImmutableArray<IFieldSymbol> fields,
                                                          Dictionary<ITypeSymbol, ITypeSymbol> strategies,
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
                          builder: builder,
                          indent: indent);

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static private void GenerateArraySize(ImmutableArray<IFieldSymbol> fields,
                                          Dictionary<ITypeSymbol, ITypeSymbol> strategies,
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

            SizeHelper.WriteTypeSize(type: field.Type,
                                     strategies: strategies,
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
}