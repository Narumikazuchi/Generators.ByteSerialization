namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private void GenerateSerializeMethod(INamedTypeSymbol symbol,
                                                ImmutableArray<IFieldSymbol> fields,
                                                Dictionary<ITypeSymbol, ITypeSymbol> strategies,
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
                                  builder: builder,
                                  indent: indent);

        builder.AppendLine($"{indent}return pointer;");

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static private void GenerateSerializationBody(ImmutableArray<IFieldSymbol> fields,
                                                  Dictionary<ITypeSymbol, ITypeSymbol> strategies,
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

            SerializationHelper.WriteTypeSerialization(type: field.Type,
                                                       strategies: strategies,
                                                       builder: builder,
                                                       indent: indent,
                                                       target: $"value.{target.Name}");
        }
    }
}