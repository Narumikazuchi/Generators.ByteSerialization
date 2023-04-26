using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;
using System.IO;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SerializeCodeWriter
{
    static public void WriteMethod(IArrayTypeSymbol array,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public UInt32 Serialize(Byte* buffer, {array.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            if (value is null)");
        builder.AppendLine("            {");
        builder.AppendLine("                *(UInt32*)buffer = 0;");
        builder.AppendLine($"                *(TypeIdentifier*)(buffer + 4) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()}));");
        builder.AppendLine("                *(buffer + 36) = 0x0;");
        builder.AppendLine("                return 37;");
        builder.AppendLine("            }");
        builder.AppendLine("            else");
        builder.AppendLine("            {");
        builder.AppendLine($"                *(TypeIdentifier*)(buffer + 4) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()}));");
        builder.AppendLine("                *(buffer + 36) = 0x1;");
        builder.AppendLine("                var pointer = buffer + 37;");

        WriteMethodBody(array: array,
                        builder: builder,
                        indent: "                ");

        builder.AppendLine("                var totalSerialized = (UInt32)(pointer - buffer);");
        builder.AppendLine("                *(UInt32*)buffer = totalSerialized - 37U;");
        builder.AppendLine("                return totalSerialized;");
        builder.AppendLine("            }");
        builder.AppendLine("        }");
    }
    static public void WriteMethod(INamedTypeSymbol type,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public UInt32 Serialize(Byte* buffer, {type.ToFrameworkString()} value)");
        builder.AppendLine("        {");

        if (!type.IsValueType)
        {
            builder.AppendLine("            if (value is null)");
            builder.AppendLine("            {");
            builder.AppendLine("                *(UInt32*)buffer = 0;");
            builder.AppendLine($"                *({GlobalNames.NAMESPACE}.TypeIdentifier*)(buffer + 4) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine("                *(buffer + 36) = 0x0;");
            builder.AppendLine("                return 37;");
            builder.AppendLine("            }");
            builder.AppendLine("            else");
            builder.AppendLine("            {");
            builder.AppendLine($"                *({GlobalNames.NAMESPACE}.TypeIdentifier*)(buffer + 4) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine("                *(buffer + 36) = 0x1;");
            builder.AppendLine("                var pointer = buffer + 37;");

            WriteMethodBody(type: type,
                            builder: builder,
                            indent: "                ");

            builder.AppendLine("                var totalSerialized = (UInt32)(pointer - buffer);");
            builder.AppendLine("                *(UInt32*)buffer = totalSerialized - 37U;");
            builder.AppendLine("                return totalSerialized;");
            builder.AppendLine("            }");
        }
        else
        {
            builder.AppendLine($"            *({GlobalNames.NAMESPACE}.TypeIdentifier*)(buffer + 4) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine("            *(buffer + 36) = 0x1;");
            builder.AppendLine("            var pointer = buffer + 37;");

            WriteMethodBody(type: type,
                            builder: builder,
                            indent: "            ");

            builder.AppendLine("            var totalSerialized = (UInt32)(pointer - buffer);");
            builder.AppendLine("            *(UInt32*)buffer = totalSerialized - 37U;");
            builder.AppendLine("            return totalSerialized;");
        }

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
            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    var {targetNormalized}_memory = MemoryMarshal.AsBytes<{array.ElementType.ToFrameworkString()}>({target});");
            builder.AppendLine($"{indent}    *(Int32*)pointer = {targetNormalized}_memory.Length;");
            builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}    var {targetNormalized}_destination = new Span<Byte>(pointer, {targetNormalized}_memory.Length);");
            builder.AppendLine($"{indent}    {targetNormalized}_memory.CopyTo({targetNormalized}_destination);");
            builder.AppendLine($"{indent}    pointer += {targetNormalized}_memory.Length;");
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            builder.AppendLine($"{indent}if ({target} is null)");
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
                builder.AppendLine($"{indent}    *(Int32*)pointer = {target}.GetLength({index});");
                builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            }

            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}    foreach (var {targetNormalized}_element in {target})");
            builder.AppendLine($"{indent}    {{");
            if (array.ElementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}        *({array.ElementType.ToFrameworkString()}*)pointer = {targetNormalized}_element;");
                builder.AppendLine($"{indent}        pointer += sizeof({array.ElementType.ToFrameworkString()});");
            }
            else if (array.ElementType is IArrayTypeSymbol elementArray)
            {
                WriteMethodBody(array: elementArray,
                                builder: builder,
                                indent: indent + "        ",
                                target: $"{targetNormalized}_element");
            }
            else if (array.ElementType is INamedTypeSymbol elementType)
            {
                WriteMethodBody(type: elementType,
                                builder: builder,
                                indent: indent + "        ",
                                target: $"{targetNormalized}_element");
            }

            builder.AppendLine($"{indent}    }}");
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
            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}if ({target} is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    *(Int32*)pointer = 0;");
            builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}   var {targetNormalized}_bytes = MemoryMarshal.AsBytes({target}.AsSpan());");
            builder.AppendLine($"{indent}   var {targetNormalized}_destination = new Span<Byte>(pointer + sizeof(Int32), {targetNormalized}_bytes.Length);");
            builder.AppendLine($"{indent}   {targetNormalized}_bytes.CopyTo({targetNormalized}_destination);");
            builder.AppendLine($"{indent}   *(UInt32*)pointer = (UInt32){targetNormalized}_bytes.Length | 0x80000000;");
            builder.AppendLine($"{indent}   pointer += {targetNormalized}_destination.Length + sizeof(Int32);");
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

        if (type.IsValueType)
        {
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
        }
        else
        {
            builder.AppendLine($"{indent}if ({target} is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    *pointer = 0x0;");
            builder.AppendLine($"{indent}    pointer++;");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    *pointer = 0x1;");
            builder.AppendLine($"{indent}    pointer++;");

            ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    WriteForMember(member: field,
                                   memberType: field.Type,
                                   builder: builder,
                                   indent: indent + "    ",
                                   target: target);
                }
                else if (member is IPropertySymbol property)
                {
                    WriteForMember(member: property,
                                   memberType: property.Type,
                                   builder: builder,
                                   indent: indent + "    ",
                                   target: target);
                }
            }

            builder.AppendLine($"{indent}}}");
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            if (type.GetMembers("Count")
                    .OfType<IPropertySymbol>()
                    .Any(property => property.DeclaredAccessibility is Accessibility.Public))
            {
                builder.AppendLine($"{indent}*(Int32*)pointer = {target}.Count;");
                builder.AppendLine($"{indent}pointer += sizeof(Int32);");
            }
            else
            {
                builder.AppendLine($"{indent}*(Int32*)pointer = ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Count;");
                builder.AppendLine($"{indent}pointer += sizeof(Int32);");
            }

            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}foreach (var {targetNormalized}_element in {target})");
            builder.AppendLine($"{indent}{{");
            if (elementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}    *({elementType.ToFrameworkString()}*)pointer = {targetNormalized}_element;");
                builder.AppendLine($"{indent}    pointer += sizeof({elementType.ToFrameworkString()});");
            }
            else if (elementType is IArrayTypeSymbol elementArray)
            {
                WriteMethodBody(array: elementArray,
                                builder: builder,
                                indent: indent + "    ",
                                target: $"{targetNormalized}_element");
            }
            else if (elementType is INamedTypeSymbol elementNamedType)
            {
                WriteMethodBody(type: elementNamedType,
                                builder: builder,
                                indent: indent + "    ",
                                target: $"{targetNormalized}_element");
            }

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
            builder.AppendLine($"{indent}*({memberType.ToFrameworkString()}*)pointer = {target}.{member.Name};");
            builder.AppendLine($"{indent}pointer += sizeof({memberType.ToFrameworkString()});");
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
                        builder.AppendLine($"{indent}    *({GlobalNames.NAMESPACE}.TypeIdentifier*)pointer = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()}));");
                        builder.AppendLine($"{indent}    pointer += sizeof({GlobalNames.NAMESPACE}.TypeIdentifier);");

                        WriteMethodBody(type: derivedType,
                                        builder: builder,
                                        indent: indent + "    ",
                                        target: $"{targetNormalized}_{member.Name}_{derivedType.Name}");

                        builder.AppendLine($"{indent}}}");
                    }

                    if (!memberType.IsAbstract)
                    {
                        builder.AppendLine($"{indent}else");
                        builder.AppendLine($"{indent}{{");
                        builder.AppendLine($"{indent}    *({GlobalNames.NAMESPACE}.TypeIdentifier*)pointer = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({memberType.ToFrameworkString()}));");
                        builder.AppendLine($"{indent}    pointer += sizeof({GlobalNames.NAMESPACE}.TypeIdentifier);");

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