using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SizeCodeWriter
{
    static public void WriteMethod(IArrayTypeSymbol array,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public Int32 GetExpectedArraySize({array.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            var size = 37;");

        WriteMethodBody(array: array,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("            return size + 128;");
        builder.AppendLine("        }");
    }
    static public void WriteMethod(INamedTypeSymbol type,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public Int32 GetExpectedArraySize({type.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            var size = 37;");

        WriteMethodBody(type: type,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("            return size + 128;");
        builder.AppendLine("        }");
    }

    static private void WriteMethodBody(IArrayTypeSymbol array,
                                        StringBuilder builder,
                                        String indent,
                                        String target = "value")
    {
        if (array.Rank is 1 &&
            array.ElementType.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    size += sizeof(Int32) + {target}.Length * sizeof({array.ElementType.ToFrameworkString()});");
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            builder.AppendLine($"{indent}size += {array.Rank * sizeof(Int32)};");

            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");

            if (array.ElementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}    size += {target}.Length * sizeof({array.ElementType.ToFrameworkString()});");
            }
            else if (array.ElementType is IArrayTypeSymbol elementArray)
            {
                String targetNormalized = target.Replace('.', '_');
                builder.AppendLine($"{indent}    foreach (var {targetNormalized}_element in {target})");
                builder.AppendLine($"{indent}    {{");

                WriteMethodBody(array: elementArray,
                                builder: builder,
                                indent: indent + "        ",
                                target: $"{targetNormalized}_element");

                builder.AppendLine($"{indent}    }}");
            }
            else if (array.ElementType is INamedTypeSymbol elementType)
            {
                String targetNormalized = target.Replace('.', '_');
                builder.AppendLine($"{indent}    foreach (var {targetNormalized}_element in {target})");
                builder.AppendLine($"{indent}    {{");

                WriteMethodBody(type: elementType,
                                builder: builder,
                                indent: indent + "        ",
                                target: $"{targetNormalized}_element");

                builder.AppendLine($"{indent}    }}");
            }

            builder.AppendLine($"{indent}}}");
        }
    }
    static private void WriteMethodBody(INamedTypeSymbol type,
                                        StringBuilder builder,
                                        String indent,
                                        String target = "value")
    {
        if (type.SpecialType is SpecialType.System_String)
        {
            builder.AppendLine($"{indent}size += sizeof(Int32);");
            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}   size += 4 * {target}.Length;");
            builder.AppendLine($"{indent}}}");
            return;
        }
        else if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<") &&
                 !type.IsUnmanagedSerializable())
        {
            IPropertySymbol property = type.GetMembers("Key")
                                           .OfType<IPropertySymbol>()
                                           .First();
            WriteForMember(member: property,
                           memberType: property.Type,
                           builder: builder,
                           indent: indent,
                           target: target);
            property = type.GetMembers("Value")
                           .OfType<IPropertySymbol>()
                           .First();
            WriteForMember(member: property,
                           memberType: property.Type,
                           builder: builder,
                           indent: indent,
                           target: target);
            return;
        }

        if (!type.IsValueType)
        {
            builder.AppendLine($"{indent}size++;");
            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");
            indent += "    ";
        }

        ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
        foreach (ISymbol member in members)
        {
            if (member is IFieldSymbol field)
            {
                WriteForMember(member: field,
                               memberType: field.Type,
                               builder: builder,
                               indent: indent,
                               target: target);
            }
            else if (member is IPropertySymbol property)
            {
                WriteForMember(member: property,
                               memberType: property.Type,
                               builder: builder,
                               indent: indent,
                               target: target);
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
                    builder.AppendLine($"{indent}size += sizeof(Int32) + {target}.Count * sizeof({elementType.ToFrameworkString()});");
                }
                else
                {
                    builder.AppendLine($"{indent}size += sizeof(Int32) + ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Count * sizeof({elementType.ToFrameworkString()});");
                }
            }
            else
            {
                String targetNormalized = target.Replace('.', '_');
                builder.AppendLine($"{indent}size += sizeof(Int32);");
                builder.AppendLine($"{indent}foreach (var {targetNormalized}_element in {target})");
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
                                       String indent,
                                       String target = "value")
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
                if (memberType is IArrayTypeSymbol elementArray)
                {
                    WriteMethodBody(array: elementArray,
                                    builder: builder,
                                    indent: indent,
                                    target: $"{target}.{member.Name}");
                }
                else if (memberType is INamedTypeSymbol elementType)
                {
                    WriteMethodBody(type: elementType,
                                    builder: builder,
                                    indent: indent,
                                    target: $"{target}.{member.Name}");
                }
            }
            else
            {
                ImmutableArray<INamedTypeSymbol> derivedTypes = ((INamedTypeSymbol)memberType).GetDerivedTypes();
                if (derivedTypes.Length is 0)
                {
                    WriteMethodBody(type: (INamedTypeSymbol)memberType,
                                    builder: builder,
                                    indent: indent,
                                    target: $"{target}.{member.Name}");
                }
                else
                {
                    builder.AppendLine($"{indent}size += 32;");
                    String targetNormalized = target.Replace('.', '_');
                    Boolean first = true;
                    foreach (INamedTypeSymbol derivedType in derivedTypes)
                    {
                        if (first)
                        {
                            first = false;
                            builder.AppendLine($"{indent}if ({target}.{member.Name} is {derivedType.ToFrameworkString()} {targetNormalized}_{member.Name}_{derivedType.Name})");
                        }
                        else
                        {
                            builder.AppendLine($"{indent}else if ({target}.{member.Name} is {derivedType.ToFrameworkString()} {targetNormalized}_{member.Name}_{derivedType.Name})");
                        }

                        builder.AppendLine($"{indent}{{");

                        WriteMethodBody(type: derivedType,
                                        builder: builder,
                                        indent: indent + "    ",
                                        target: $"{targetNormalized}_{member.Name}_{derivedType.Name}");

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

                        WriteMethodBody(type: (INamedTypeSymbol)memberType,
                                        builder: builder,
                                        indent: indent + "    ",
                                        target: $"{target}.{member.Name}");

                        builder.AppendLine($"{indent}}}");
                    }
                }
            }
        }
    }
}