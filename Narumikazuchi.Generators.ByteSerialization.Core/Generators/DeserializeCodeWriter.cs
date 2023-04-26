using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;
using System.IO;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class DeserializeCodeWriter
{
    static public void WriteMethod(IArrayTypeSymbol array,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public UInt32 Deserialize(Byte* buffer, out {array.ToFrameworkString()} result)");
        builder.AppendLine("        {");
        builder.AppendLine("            var objectSize = *(Int32*)buffer;");
        builder.AppendLine("            if (*(buffer + 36) == 0x0)");
        builder.AppendLine("            {");
        builder.AppendLine("                result = default;");
        builder.AppendLine("                return 37;");
        builder.AppendLine("            }");
        builder.AppendLine($"            var typeIdentifier = *({GlobalNames.NAMESPACE}.TypeIdentifier*)(buffer + 4);");
        builder.AppendLine($"            if (typeIdentifier != {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()})))");
        builder.AppendLine("            {");
        builder.AppendLine("                throw new Exception();");
        builder.AppendLine("            }");
        builder.AppendLine("            var pointer = buffer + 37;");

        WriteMethodBody(array: array,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("            return (UInt32)(pointer - buffer);");
        builder.AppendLine("        }");
    }
    static public void WriteMethod(INamedTypeSymbol type,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public UInt32 Deserialize(Byte* buffer, out {type.ToFrameworkString()} result)");
        builder.AppendLine("        {");
        builder.AppendLine("            var objectSize = *(Int32*)buffer;");
        builder.AppendLine("            if (*(buffer + 36) == 0x0)");
        builder.AppendLine("            {");
        builder.AppendLine("                result = default;");
        builder.AppendLine("                return 37;");
        builder.AppendLine("            }");
        builder.AppendLine($"            var typeIdentifier = *({GlobalNames.NAMESPACE}.TypeIdentifier*)(buffer + 4);");
        builder.AppendLine($"            if (typeIdentifier != {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()})))");
        builder.AppendLine("            {");
        builder.AppendLine("                throw new Exception();");
        builder.AppendLine("            }");
        builder.AppendLine("            var pointer = buffer + 37;");

        WriteMethodBody(type: type,
                        builder: builder,
                        indent: "            ");

        builder.AppendLine("            return (UInt32)(pointer - buffer);");
        builder.AppendLine("        }");
    }

    static private void WriteMethodBody(IArrayTypeSymbol array,
                                        StringBuilder builder,
                                        String indent,
                                        String target = "result")
    {
        if (array.Rank is 1 &&
            array.ElementType.IsUnmanagedSerializable())
        {
            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}var {targetNormalized}_size = *(Int32*)pointer;");
            builder.AppendLine($"{indent}pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}var {targetNormalized}_bytes = new Span<Byte>(pointer, {targetNormalized}_size);");
            if (target is "result")
            {
                builder.AppendLine($"{indent}result = MemoryMarshal.Cast<Byte, {array.ElementType.ToFrameworkString()}>({targetNormalized}_bytes).ToArray();");
            }
            else
            {
                builder.AppendLine($"{indent}var {target} = MemoryMarshal.Cast<Byte, {array.ElementType.ToFrameworkString()}>({targetNormalized}_bytes).ToArray();");
            }

            builder.AppendLine($"{indent}pointer += {targetNormalized}_bytes.Length;");
        }
        else
        {
            String targetNormalized = target.Replace('.', '_');
            String[] arraySizes = new String[array.Rank];
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}var {targetNormalized}_size_{index} = *(Int32*)pointer;");
                builder.AppendLine($"{indent}pointer += sizeof(Int32);");
                arraySizes[index] = $"{targetNormalized}_size_{index}";
            }

            if (target is "result")
            {
                builder.AppendLine($"{indent}result = {array.CreateArray(arraySizes)};");
            }
            else
            {
                builder.AppendLine($"{indent}var {target} = {array.CreateArray(arraySizes)};");
            }

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}for (var {targetNormalized}_index_{index} = 0; {targetNormalized}_index_{index} < {targetNormalized}_size_{index}; {targetNormalized}_index_{index}++)");
                builder.AppendLine($"{indent}{{");
                indent += "    ";
            }

            if (array.ElementType.IsUnmanagedSerializable())
            {
                builder.AppendLine($"{indent}var {targetNormalized}_element = *({array.ElementType.ToFrameworkString()}*)pointer;");
                builder.AppendLine($"{indent}pointer += sizeof({array.ElementType.ToFrameworkString()});");
            }
            else if (array.ElementType is IArrayTypeSymbol elementArray)
            {
                WriteMethodBody(array: elementArray,
                                builder: builder,
                                indent: indent,
                                target: $"{targetNormalized}_element");
            }
            else if (array.ElementType is INamedTypeSymbol elementType)
            {
                WriteMethodBody(type: elementType,
                                builder: builder,
                                indent: indent,
                                target: $"{targetNormalized}_element");
            }

            builder.Append($"{indent}{targetNormalized}[");
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append($"{targetNormalized}_index_{index}");
            }

            builder.AppendLine($"] = {targetNormalized}_element;");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                indent = indent.Substring(4);
                builder.AppendLine($"{indent}}}");
            }
        }
    }
    static private void WriteMethodBody(INamedTypeSymbol type,
                                        StringBuilder builder,
                                        String indent,
                                        String target = "result")
    {
        if (type.SpecialType is SpecialType.System_String)
        {
            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}var {targetNormalized}_size = *(UInt32*)pointer;");
            if (target is "result")
            {
                builder.AppendLine($"{indent}result = ({type.ToFrameworkString()})null;");
            }
            else
            {
                builder.AppendLine($"{indent}var {targetNormalized} = ({type.ToFrameworkString()})null;");
            }

            builder.AppendLine($"{indent}if (({targetNormalized}_size & 0x80000000) == 0x80000000)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    var {targetNormalized}_source = new Span<Byte>(pointer + 4, (Int32)({targetNormalized}_size & 0x0FFFFFFF));");
            if (target is "result")
            {
                builder.AppendLine($"{indent}    result = new String(MemoryMarshal.Cast<Byte, Char>({targetNormalized}_source));");
            }
            else
            {
                builder.AppendLine($"{indent}    {targetNormalized} = new String(MemoryMarshal.Cast<Byte, Char>({targetNormalized}_source));");
            }

            builder.AppendLine($"{indent}    pointer += {targetNormalized}_source.Length + sizeof(Int32);");
            builder.AppendLine($"{indent}}}");
            return;
        }
        else if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<") &&
                 !type.IsUnmanagedSerializable())
        {
            String targetNormalized = target.Replace('.', '_');
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
            if (target is "result")
            {
                builder.AppendLine($"{indent}result = new {type.ToFrameworkString()}({targetNormalized}_Key, {targetNormalized}_Value);");
            }
            else
            {
                builder.AppendLine($"{indent}var {targetNormalized} = new {type.ToFrameworkString()}({targetNormalized}_Key, {targetNormalized}_Value);");
            }

            return;
        }

        if (type.IsValueType)
        {
            WriteForType(type: type,
                         builder: builder,
                         indent: indent,
                         target: target,
                         includeVar: true);
        }
        else
        {
            String targetNormalized = target.Replace('.', '_');
            if (target is "result")
            {
                builder.AppendLine($"{indent}result = ({type.ToFrameworkString()})null;");
            }
            else
            {
                builder.AppendLine($"{indent}var {targetNormalized} = ({type.ToFrameworkString()})null;");
            }

            builder.AppendLine($"{indent}if (*pointer == 0x1)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    pointer++;");

            WriteForType(type: type,
                         builder: builder,
                         indent: indent + "    ",
                         target: target);

            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    pointer++;");
            builder.AppendLine($"{indent}}}");
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}var {targetNormalized}_count = *(Int32*)pointer;");
            builder.AppendLine($"{indent}pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}for (var {targetNormalized}_counter = 0; {targetNormalized}_counter < {targetNormalized}_count; {targetNormalized}_counter++)");
            builder.AppendLine($"{indent}{{");
            if (type.GetMembers("Add")
                    .OfType<IMethodSymbol>()
                    .Any(method => method.Parameters.Length is 1 &&
                                   method.DeclaredAccessibility is Accessibility.Public &&
                                   SymbolEqualityComparer.Default.Equals(elementType, method.Parameters[0].Type)))
            {
                if (elementType.IsUnmanagedSerializable())
                {
                    builder.AppendLine($"{indent}    {targetNormalized}.Add(*({elementType.ToFrameworkString()}*)pointer);");
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
                                       String indent,
                                       String target = "result")
    {
        if (memberType.IsUnmanagedSerializable())
        {
            String targetNormalized = target.Replace('.', '_');
            builder.AppendLine($"{indent}var {targetNormalized}_{member.Name} = *({memberType.ToFrameworkString()}*)pointer;");
            builder.AppendLine($"{indent}pointer += sizeof({memberType.ToFrameworkString()});");
        }
        else
        {
            if (memberType.IsValueType ||
                memberType.IsSealed)
            {
                String targetNormalized = target.Replace('.', '_');
                WriteMethodBody(type: (INamedTypeSymbol)memberType,
                                builder: builder,
                                indent: indent,
                                target: $"{targetNormalized}_{member.Name}");
            }
            else
            {
                ImmutableArray<INamedTypeSymbol> derivedTypes = ((INamedTypeSymbol)memberType).GetDerivedTypes();
                if (derivedTypes.Length is 0)
                {
                    String targetNormalized = target.Replace('.', '_');
                    WriteMethodBody(type: (INamedTypeSymbol)memberType,
                                    builder: builder,
                                    indent: indent,
                                    target: $"{targetNormalized}_{member.Name}");
                }
                else
                {
                    String targetNormalized = target.Replace('.', '_');
                    builder.AppendLine($"{indent}var {targetNormalized}_typeIdentifier = *({GlobalNames.NAMESPACE}.TypeIdentifier*)pointer;");
                    builder.AppendLine($"{indent}pointer += sizeof({GlobalNames.NAMESPACE}.TypeIdentifier);");
                    Boolean first = true;
                    foreach (INamedTypeSymbol derivedType in derivedTypes)
                    {
                        if (first)
                        {
                            first = false;
                            builder.AppendLine($"{indent}if ({targetNormalized}_typeIdentifier == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom({derivedType.ToFrameworkString()}))");
                        }
                        else
                        {
                            builder.AppendLine($"{indent}else if ({targetNormalized}_typeIdentifier == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom({derivedType.ToFrameworkString()}))");
                        }

                        builder.AppendLine($"{indent}{{");

                        WriteMethodBody(type: derivedType,
                                        builder: builder,
                                        indent: indent + "    ",
                                        target: $"{targetNormalized}_{member.Name}");

                        builder.AppendLine($"{indent}}}");
                    }

                    if (!memberType.IsAbstract)
                    {
                        builder.AppendLine($"{indent}else if ({targetNormalized}_typeIdentifier == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom({memberType.ToFrameworkString()}))");
                        builder.AppendLine($"{indent}{{");

                        WriteMethodBody(type: (INamedTypeSymbol)memberType,
                                        builder: builder,
                                        indent: indent + "    ",
                                        target: $"{targetNormalized}_{member.Name}");

                        builder.AppendLine($"{indent}}}");
                    }

                    builder.AppendLine($"{indent}else");
                    builder.AppendLine($"{indent}{{");
                    builder.AppendLine($"{indent}    throw new Exception();");
                    builder.AppendLine($"{indent}}}");
                }
            }
        }
    }

    static private void WriteForType(INamedTypeSymbol type,
                                     StringBuilder builder,
                                     String indent,
                                     String target = "result",
                                     Boolean includeVar = default)
    {
        ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
        if (type.HasDefaultConstructor())
        {
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

            String targetNormalized = target.Replace('.', '_');
            if (target is "result")
            {
                builder.AppendLine($"{indent}result = new {type.ToFrameworkString()}()");
            }
            else
            {
                if (includeVar)
                {
                    builder.AppendLine($"{indent}var {targetNormalized} = new {type.ToFrameworkString()}()");
                }
                else
                {
                    builder.AppendLine($"{indent}{targetNormalized} = new {type.ToFrameworkString()}()");
                }
            }

            builder.AppendLine($"{indent}{{");
            foreach (ISymbol member in members)
            {
                builder.AppendLine($"{indent}   {member.Name} = {targetNormalized}_{member.Name},");
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

            List<String> parameters = new();
            StringBuilder initializer = new();
            IMethodSymbol constructor = type.InstanceConstructors.First();
            String targetNormalized = target.Replace('.', '_');
            foreach (ISymbol member in members)
            {
                if (constructor.Parameters.Any(parameter => parameter.Name == member.Name))
                {
                    parameters.Add($"{member.Name}: {targetNormalized}_{member.Name}");
                }
                else
                {
                    initializer.AppendLine($"{indent}   {member.Name} = {targetNormalized}_{member.Name},");
                }
            }

            if (target is "result")
            {
                builder.Append($"{indent}result = new {type.ToFrameworkString()}({String.Join(", ", parameters)})");
            }
            else
            {
                if (includeVar)
                {
                    builder.Append($"{indent}var {targetNormalized} = new {type.ToFrameworkString()}({String.Join(", ", parameters)})");
                }
                else
                {
                    builder.Append($"{indent}{targetNormalized} = new {type.ToFrameworkString()}({String.Join(", ", parameters)})");
                }
            }

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

            String targetNormalized = target.Replace('.', '_');
            String parameters = String.Join(", ", members.Select(member => $"{targetNormalized}_{member.Name}"));
            if (target is "result")
            {
                builder.Append($"{indent}result = s_Constructor.Value.Invoke({parameters});");
            }
            else
            {
                if (includeVar)
                {
                    builder.Append($"{indent}var {targetNormalized} = s_Constructor.Value.Invoke({parameters});");
                }
                else
                {
                    builder.Append($"{indent}{targetNormalized} = s_Constructor.Value.Invoke({parameters});");
                }
            }
        }
    }
}