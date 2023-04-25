using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SerializeCodeWriter
{
    static public void WriteMethod(IArrayTypeSymbol array,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine($"        static public Int32 Serialize(Byte* buffer, {array.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            var pointer = buffer;");

        WriteMethodBody(array: array,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("            return (Int32)(pointer - buffer);");
        builder.AppendLine("        }");
    }
    static public void WriteMethod(INamedTypeSymbol type,
                                   ImmutableArray<ISymbol> members,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine($"        static public Int32 Serialize(Byte* buffer, {type.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            var pointer = buffer;");

        if (!type.IsValueType)
        {
            builder.AppendLine("            if (value is null)");
            builder.AppendLine("            {");
            builder.AppendLine("                return 0;");
            builder.AppendLine("            }");
            builder.AppendLine("            else");
            builder.AppendLine("            {");
            WriteMethodBody(type: type,
                            members: members,
                            builder: builder,
                            indent: "                ");
            builder.AppendLine("            }");
        }
        else
        {
            WriteMethodBody(type: type,
                            members: members,
                            builder: builder,
                            indent: "            ");
        }

        builder.AppendLine("            return (Int32)(pointer - buffer);");
        builder.AppendLine("        }");
    }

    static private void WriteMethodBody(IArrayTypeSymbol array,
                                        StringBuilder builder,
                                        String indent)
    {
        if (array.Rank is 1 &&
            array.ElementType.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}if (value is not null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    var memory = MemoryMarshal.AsBytes<{array.ElementType.ToFrameworkString()}>(value);");
            builder.AppendLine($"{indent}    *(Int32*)pointer = memory.Length;");
            builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}    var destination = new Span<Byte>(pointer, memory.Length);");
            builder.AppendLine($"{indent}    memory.CopyTo(destination);");
            builder.AppendLine($"{indent}    pointer += memory.Length;");
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            builder.AppendLine($"{indent}if (value is null)");
            builder.AppendLine($"{indent}{{");
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}    *(Int32*)pointer = 0;");
                builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            }

            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}    *(Int32*)pointer = value.GetLength({index});");
                builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            }

            builder.AppendLine($"{indent}    foreach (var element in value)");
            builder.AppendLine($"{indent}    {{");
            if (array.ElementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}        *({array.ElementType.ToFrameworkString()}*)pointer = element;");
                builder.AppendLine($"{indent}        pointer += sizeof({array.ElementType.ToFrameworkString()});");
            }
            else
            {
                builder.AppendLine($"{indent}        pointer += {GlobalNames.BYTESERIALIZER}.Serialize(pointer, element);");
            }

            builder.AppendLine($"{indent}    }}");
            builder.AppendLine($"{indent}}}");
        }
    }
    static private void WriteMethodBody(INamedTypeSymbol type,
                                        ImmutableArray<ISymbol> members,
                                        StringBuilder builder,
                                        String indent)
    {
        if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<") &&
            !type.IsUnmanagedSerializable())
        {
            IPropertySymbol property = type.GetMembers("Key")
                                           .OfType<IPropertySymbol>()
                                           .First();
            WriteForMember(member: property,
                           memberType: property.Type,
                           builder: builder,
                           indent: indent);
            property = type.GetMembers("Value")
                           .OfType<IPropertySymbol>()
                           .First();
            WriteForMember(member: property,
                           memberType: property.Type,
                           builder: builder,
                           indent: indent);
            return;
        }

        foreach (ISymbol member in members)
        {
            if (member is IFieldSymbol field)
            {
                WriteForMember(member: field,
                               memberType: field.Type,
                               builder: builder,
                               indent: indent);
            }
            else if (member is IPropertySymbol property)
            {
                WriteForMember(member: property,
                               memberType: property.Type,
                               builder: builder,
                               indent: indent);
            }
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            if (type.GetMembers("Count")
                    .OfType<IPropertySymbol>()
                    .Any(property => property.DeclaredAccessibility is Accessibility.Public))
            {
                builder.AppendLine($"{indent}*(Int32*)pointer = value.Count;");
                builder.AppendLine($"{indent}pointer += sizeof(Int32);");
            }
            else
            {
                builder.AppendLine($"{indent}*(Int32*)pointer = ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>)value).Count;");
                builder.AppendLine($"{indent}pointer += sizeof(Int32);");
            }

            builder.AppendLine($"{indent}foreach (var element in value)");
            builder.AppendLine($"{indent}{{");
            if (elementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}    *({elementType.ToFrameworkString()}*)pointer = element;");
                builder.AppendLine($"{indent}    pointer += sizeof({elementType.ToFrameworkString()});");
            }
            else
            {
                builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Serialize(pointer, element);");
            }

            builder.AppendLine($"{indent}}}");
        }
    }

    static private void WriteForMember(ISymbol member,
                                       ITypeSymbol memberType,
                                       StringBuilder builder,
                                       String indent)
    {
        if (memberType.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}*({memberType.ToFrameworkString()}*)pointer = value.{member.Name};");
            builder.AppendLine($"{indent}pointer += sizeof({memberType.ToFrameworkString()});");
        }
        else
        {
            if (memberType.IsValueType ||
                memberType.IsSealed)
            {
                builder.AppendLine($"{indent}pointer += {GlobalNames.BYTESERIALIZER}.Serialize(pointer, value.{member.Name});");
            }
            else
            {
                ImmutableArray<INamedTypeSymbol> derivedTypes = ((INamedTypeSymbol)memberType).GetDerivedTypes();
                if (derivedTypes.Length is 0)
                {
                    builder.AppendLine($"{indent}pointer += {GlobalNames.BYTESERIALIZER}.Serialize(pointer, value.{member.Name});");
                }
                else
                {
                    Boolean first = true;
                    foreach (ITypeSymbol derivedType in derivedTypes)
                    {
                        if (first)
                        {
                            first = false;
                            builder.AppendLine($"{indent}if (value.{member.Name} is {derivedType.ToFrameworkString()})");
                        }
                        else
                        {
                            builder.AppendLine($"{indent}else if (value.{member.Name} is {derivedType.ToFrameworkString()})");
                        }

                        builder.AppendLine($"{indent}{{");
                        builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Serialize(pointer, ({derivedType.ToFrameworkString()})value.{member.Name});");
                        builder.AppendLine($"{indent}}}");
                    }

                    if (memberType.IsAbstract)
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
                        builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Serialize(pointer, value.{member.Name});");
                        builder.AppendLine($"{indent}}}");
                    }
                }
            }
        }
    }
}