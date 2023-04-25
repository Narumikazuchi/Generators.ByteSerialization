using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SizeCodeWriter
{
    static public void WriteMethod(IArrayTypeSymbol array,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine($"        static public Int32 GetExpectedArraySize({array.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            var size = 0;");

        WriteMethodBody(array: array,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("            return size;");
        builder.AppendLine("        }");
    }
    static public void WriteMethod(INamedTypeSymbol type,
                                   ImmutableArray<ISymbol> members,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine($"        static public Int32 GetExpectedArraySize({type.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            var size = 0;");

        WriteMethodBody(type: type,
                        members: members,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("            return size;");
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
            builder.AppendLine($"{indent}    size += sizeof(Int32) + value.Length * sizeof({array.ElementType.ToFrameworkString()});");
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            builder.AppendLine($"{indent}size += {array.Rank * sizeof(Int32)};");

            builder.AppendLine($"{indent}if (value is not null)");
            builder.AppendLine($"{indent}{{");

            if (array.ElementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}    size += value.Length * sizeof({array.ElementType.ToFrameworkString()});");
            }
            else
            {
                builder.AppendLine($"{indent}    foreach (var element in value)");
                builder.AppendLine($"{indent}    {{");
                builder.AppendLine($"{indent}        size += {GlobalNames.BYTESERIALIZER}.GetExpectedSerializedSize(element);");
                builder.AppendLine($"{indent}    }}");
            }

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

        if (!type.IsValueType)
        {
            builder.AppendLine($"{indent}if (value is not null)");
            builder.AppendLine($"{indent}{{");
            indent += "    ";
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
            if (elementType.IsUnmanagedSerializable())
            {
                if (type.GetMembers("Count")
                        .OfType<IPropertySymbol>()
                        .Any(property => property.DeclaredAccessibility is Accessibility.Public))
                {
                    builder.AppendLine($"{indent}size += sizeof(Int32) + value.Count * sizeof({elementType.ToFrameworkString()});");
                }
                else
                {
                    builder.AppendLine($"{indent}size += sizeof(Int32) + ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>)value).Count * sizeof({elementType.ToFrameworkString()});");
                }
            }
            else
            {
                builder.AppendLine($"{indent}size += sizeof(Int32);");
                builder.AppendLine($"{indent}foreach (var element in value)");
                builder.AppendLine($"{indent}{{");
                builder.AppendLine($"{indent}   size += {GlobalNames.BYTESERIALIZER}.GetExpectedSerializedSize(element);");
                builder.AppendLine($"{indent}}}");
            }
        }

        if (!type.IsValueType)
        {
            indent = indent.Substring(4);
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
            builder.AppendLine($"{indent}size += sizeof({memberType.ToFrameworkString()});");
        }
        else
        {
            if (memberType.IsValueType ||
                memberType.IsSealed)
            {
                builder.AppendLine($"{indent}size += {GlobalNames.BYTESERIALIZER}.GetExpectedSerializedSize(value.{member.Name});");
            }
            else
            {
                ImmutableArray<INamedTypeSymbol> derivedTypes = ((INamedTypeSymbol)memberType).GetDerivedTypes();
                if (derivedTypes.Length is 0)
                {
                    builder.AppendLine($"{indent}pointer += {GlobalNames.BYTESERIALIZER}.GetExpectedSerializedSize(value.{member.Name});");
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
                        builder.AppendLine($"{indent}    size += {GlobalNames.BYTESERIALIZER}.GetExpectedSerializedSize(({derivedType.ToFrameworkString()})value.{member.Name});");
                        builder.AppendLine($"{indent}}}");
                    }

                    if (memberType.IsAbstract)
                    {
                        builder.AppendLine($"{indent}else");
                        builder.AppendLine($"{indent}{{");
                        builder.AppendLine($"{indent}    throw new {GlobalNames.BYTESERIALIZER}.TypeNotSerializable(typeof({memberType.ToFrameworkString()}));");
                        builder.AppendLine($"{indent}}}");
                    }
                    else
                    {
                        builder.AppendLine($"{indent}else");
                        builder.AppendLine($"{indent}{{");
                        builder.AppendLine($"{indent}    size += {GlobalNames.BYTESERIALIZER}.GetExpectedSerializedSize(value.{member.Name});");
                        builder.AppendLine($"{indent}}}");
                    }
                }
            }
        }
    }
}