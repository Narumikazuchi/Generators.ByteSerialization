namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private void GenerateDeserializeMethod(INamedTypeSymbol symbol,
                                                  ImmutableArray<IFieldSymbol> fields,
                                                  Dictionary<ITypeSymbol, ITypeSymbol> strategies,
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

        GenerateDeserializationBody(symbol: symbol,
                                    fields: fields,
                                    strategies: strategies,
                                    builder: builder,
                                    indent: indent);

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
    }

    static private void GenerateDeserializationBody(INamedTypeSymbol symbol,
                                                    ImmutableArray<IFieldSymbol> fields,
                                                    Dictionary<ITypeSymbol, ITypeSymbol> strategies,
                                                    StringBuilder builder,
                                                    String indent)
    {
        StringBuilder constructBuilder = new();
        constructBuilder.Append($"{indent}return Narumikazuchi.Generated.ConstructorGenerator.ConstructorFor_{symbol.ToFrameworkString().Replace(".", "")}.Invoke(");

        Boolean first = true;
        foreach (IFieldSymbol field in fields)
        {
            ISymbol target = field;
            if (field.AssociatedSymbol is not null)
            {
                target = field.AssociatedSymbol;
            }

            DeserializationHelper.WriteTypeDeserialization(type: field.Type,
                                                           strategies: strategies,
                                                           builder: builder,
                                                           constructBuilder: constructBuilder,
                                                           indent: indent,
                                                           target: target.Name,
                                                           first: ref first);
        }

        constructBuilder.AppendLine(");");
        builder.Append(constructBuilder.ToString());
    }
}