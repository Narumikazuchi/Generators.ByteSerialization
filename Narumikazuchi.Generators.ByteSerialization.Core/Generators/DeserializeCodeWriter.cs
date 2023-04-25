using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class DeserializeCodeWriter
{
    static public void WriteMethod(IArrayTypeSymbol array,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine($"        static public Int32 Deserialize(Byte* buffer, out {array.ToFrameworkString()} result)");
        builder.AppendLine("        {");

        WriteMethodBody(array: array,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("        }");
    }
    static public void WriteMethod(INamedTypeSymbol type,
                                   ImmutableArray<ISymbol> members,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine($"        static public Int32 Deserialize(Byte* buffer, out {type.ToFrameworkString()} result)");
        builder.AppendLine("        {");
        builder.AppendLine("            var pointer = buffer;");

        WriteMethodBody(type: type,
                        members: members,
                        builder: builder,
                        indent: "            ");

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
            builder.AppendLine($"{indent}var size = *(Int32*)buffer;");
            builder.AppendLine($"{indent}var bytes = new Span<Byte>(buffer + sizeof(Int32), size);");
            builder.AppendLine($"{indent}result = MemoryMarshal.Cast<Byte, {array.ElementType.ToFrameworkString()}>(bytes).ToArray();");
            builder.AppendLine($"{indent}return result.Length + sizeof(Int32);");
        }
        else
        {
            builder.AppendLine("            var pointer = buffer;");

            String[] arraySizes = new String[array.Rank];
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}var size_{index} = *(Int32*)pointer;");
                builder.AppendLine($"{indent}pointer += sizeof(Int32);");
                arraySizes[index] = $"size_{index}";
            }

            builder.AppendLine($"{indent}result = {array.CreateArray(arraySizes)};");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}for (var index_{index} = 0; index_{index} < size_{index}; index_{index}++)");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
            }

            if (array.ElementType.SpecialType is SpecialType.System_String)
            {
                builder.AppendLine($"{indent}pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<String>(pointer, out var element);");
            }
            else if (array.ElementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}var element = *({array.ElementType.ToFrameworkString()}*)pointer;");
                builder.AppendLine($"{indent}pointer += sizeof({array.ElementType.ToFrameworkString()});");
            }
            else
            {
                builder.AppendLine($"{indent}pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<{array.ElementType.ToFrameworkString()}>(pointer, out var element);");
            }

            builder.Append($"{indent}result[");
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append($"index_{index}");
            }

            builder.AppendLine($"] = element;");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                indent = indent.Substring(4);
                builder.AppendLine($"{indent}}}");
            }

            builder.AppendLine($"{indent}return (Int32)(pointer - buffer);");
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
            builder.AppendLine($"{indent}result = new {type.ToFrameworkString()}(_Key, _Value);");
            return;
        }

        if (type.HasDefaultConstructor())
        {
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

            builder.AppendLine($"{indent}result = new {type.ToFrameworkString()}()");
            builder.AppendLine($"{indent}{{");
            foreach (ISymbol member in members)
            {
                builder.AppendLine($"{indent}   {member.Name} = _{member.Name},");
            }

            builder.AppendLine($"{indent}}};");
        }
        else if (type.IsRecord)
        {
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

            List<String> parameters = new();
            StringBuilder initializer = new();
            IMethodSymbol constructor = type.InstanceConstructors.First();
            foreach (ISymbol member in members)
            {
                if (constructor.Parameters.Any(parameter => parameter.Name == member.Name))
                {
                    parameters.Add($"{member.Name}: _{member.Name}");
                }
                else
                {
                    initializer.AppendLine($"{indent}   {member.Name} = _{member.Name},");
                }
            }

            builder.Append($"{indent}result = new {type.ToFrameworkString()}({String.Join(", ", parameters)})");
            if (initializer.Length > 0)
            {
                builder.AppendLine();
                builder.AppendLine($"{indent}{{");
                builder.Append(initializer.ToString());
                builder.AppendLine($"{indent}}};");
            }
            else
            {
                builder.AppendLine(";");
            }
        }
        else
        {
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

            String parameters = String.Join(", ", members.Select(member => $"_{member.Name}"));
            builder.AppendLine($"{indent}result = s_Constructor.Value.Invoke({parameters});");
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            builder.AppendLine($"{indent}Int32 count = *(Int32*)pointer;");
            builder.AppendLine($"{indent}pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}for (var counter = 0; counter < count; counter++)");
            builder.AppendLine($"{indent}{{");
            if (type.GetMembers("Add")
                    .OfType<IMethodSymbol>()
                    .Any(method => method.Parameters.Length is 1 &&
                                   method.DeclaredAccessibility is Accessibility.Public &&
                                   SymbolEqualityComparer.Default.Equals(elementType, method.Parameters[0].Type)))
            {
                if (elementType.IsUnmanagedSerializable())
                {
                    builder.AppendLine($"{indent}    result.Add(*({elementType.ToFrameworkString()}*)pointer);");
                    builder.AppendLine($"{indent}    pointer += sizeof({elementType.ToFrameworkString()});");
                }
                else
                {
                    builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<{elementType.ToFrameworkString()}>(pointer, out var element);");
                    builder.AppendLine($"{indent}    result.Add(element);");
                }
            }
            else
            {
                if (elementType.IsUnmanagedSerializable())
                {
                    builder.AppendLine($"{indent}    ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>)result).Add(*({elementType.ToFrameworkString()}*)pointer);");
                    builder.AppendLine($"{indent}    pointer += sizeof({elementType.ToFrameworkString()});");
                }
                else
                {
                    builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<{elementType.ToFrameworkString()}>(pointer, out var element);");
                    builder.AppendLine($"{indent}    ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>)result).Add(element);");
                }
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
            builder.AppendLine($"{indent}var _{member.Name} = *({memberType.ToFrameworkString()}*)pointer;");
            builder.AppendLine($"{indent}pointer += sizeof({memberType.ToFrameworkString()});");
        }
        else
        {
            if (memberType.IsValueType ||
                memberType.IsSealed)
            {
                builder.AppendLine($"{indent}pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<{memberType.ToFrameworkString()}>(pointer, out var _{member.Name});");
            }
            else
            {
                ImmutableArray<INamedTypeSymbol> derivedTypes = ((INamedTypeSymbol)memberType).GetDerivedTypes();
                if (derivedTypes.Length is 0)
                {
                    builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<{memberType.ToFrameworkString()}>(pointer, out var _{member.Name});");
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
                        builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<({derivedType.ToFrameworkString()}>(pointer, out var _{member.Name});");
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
                        builder.AppendLine($"{indent}    pointer += {GlobalNames.BYTESERIALIZER}.Deserialize<{memberType.ToFrameworkString()}>(pointer, out var _{member.Name});");
                        builder.AppendLine($"{indent}}}");
                    }
                }
            }
        }
    }
}