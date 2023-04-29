using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class DeserializeCodeWriter
{
    static public void WriteMethod(ITypeSymbol type,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out {type.ToFrameworkString()} result)");
        builder.AppendLine("        {");
        builder.AppendLine("            var pointer = sizeof(Int32);");

        Int32 varCounter = -1;
        WriteForType(type: type,
                     builder: builder,
                     indent: "            ",
                     target: "result",
                     varCounter: ref varCounter);

        builder.AppendLine("            return (UInt32)pointer;");
        builder.AppendLine("        }");
    }

    static private void WriteForType(ITypeSymbol type,
                                     StringBuilder builder,
                                     String indent,
                                     String target,
                                     ref Int32 varCounter)
    {
        if (type is IArrayTypeSymbol array)
        {
            WriteForArrayType(array: array,
                              builder: builder,
                              indent: indent,
                              target: target,
                              varCounter: ref varCounter);
        }
        else if (type.IsValueType)
        {
            WriteForValueType(type: (INamedTypeSymbol)type,
                              builder: builder,
                              indent: indent,
                              target: target,
                              varCounter: ref varCounter);
        }
        else if (type.IsSealed)
        {
            WriteForSealedType(type: (INamedTypeSymbol)type,
                               builder: builder,
                               indent: indent,
                               target: target,
                               varCounter: ref varCounter);
        }
        else if (type.IsAbstract)
        {
            WriteForAbstractType(type: (INamedTypeSymbol)type,
                                 builder: builder,
                                 indent: indent,
                                 target: target,
                                 varCounter: ref varCounter);
        }
        else
        {
            WriteForPolymorphicType(type: (INamedTypeSymbol)type,
                                    builder: builder,
                                    indent: indent,
                                    target: target,
                                    varCounter: ref varCounter);
        }
    }

    static private void WriteForArrayType(IArrayTypeSymbol array,
                                          StringBuilder builder,
                                          String indent,
                                          String target,
                                          ref Int32 varCounter)
    {
        builder.AppendLine($"{indent}{AssignTo(target)} = default({array.ToFrameworkString()});");
        if (array.Rank is 1 &&
            array.ElementType.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            builder.AppendLine($"{indent}{{");
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            // TODO: Type doesn't match
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<Int32>();");
            builder.AppendLine($"{indent}    {target} = MemoryMarshal.Cast<Byte, {array.ElementType.ToFrameworkString()}>(buffer[pointer..(pointer + _var{varCounter})]).ToArray();");
            builder.AppendLine($"{indent}    pointer += _var{varCounter};");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            builder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            builder.AppendLine($"{indent}{{");
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            // TODO: Type doesn't match
            String[] arraySizes = new String[array.Rank];
            Int32 arrayCounter = ++varCounter;
            varCounter += 2 * array.Rank;
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}    var _var{arrayCounter + index} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
                builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
                arraySizes[index] = $"_var{arrayCounter + index}";
            }

            builder.AppendLine($"{indent}    {target} = {array.CreateArray(arraySizes)};");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}    for (var _var{arrayCounter + array.Rank + index} = 0; _var{arrayCounter + array.Rank + index} < _var{arrayCounter + index}; _var{arrayCounter + array.Rank + index}++)");
                builder.AppendLine($"{indent}    {{");
                indent += "    ";
            }

            WriteForType(type: array.ElementType,
                         builder: builder,
                         indent: indent + "    ",
                         target: $"_var{arrayCounter + 2 * array.Rank}",
                         varCounter: ref varCounter);

            builder.Append($"{indent}    {target}[");
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }

                builder.Append($"_var{arrayCounter + array.Rank + index}");
            }

            builder.AppendLine($"] = _var{arrayCounter + 2 * array.Rank};");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}}}");
                indent = indent.Substring(4);
            }

            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
        }
    }

    static private void WriteForValueType(INamedTypeSymbol type,
                                          StringBuilder builder,
                                          String indent,
                                          String target,
                                          ref Int32 varCounter)
    {
        if (type.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}{AssignTo(target)} = Unsafe.As<Byte, {type.ToFrameworkString()}>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}pointer += Unsafe.SizeOf<{type.ToFrameworkString()}>();");
        }
        else if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
        {
            IPropertySymbol property = type.GetMembers("Key")
                                           .OfType<IPropertySymbol>()
                                           .First();
            varCounter++;
            String key = $"_var{varCounter}";
            WriteForType(type: property.Type,
                         builder: builder,
                         indent: indent,
                         target: key,
                         varCounter: ref varCounter);

            property = type.GetMembers("Value")
                           .OfType<IPropertySymbol>()
                           .First();
            varCounter++;
            String value = $"_var{varCounter}";
            WriteForType(type: property.Type,
                         builder: builder,
                         indent: indent,
                         target: value,
                         varCounter: ref varCounter);
            builder.AppendLine($"{indent}{AssignTo(target)} = new {type.ToFrameworkString()}({key}, {value});");
        }
        else
        {
            builder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
            varCounter++;
            builder.AppendLine($"{indent}var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            // TODO: Type doesn't match

            CreateObject(type: type,
                         builder: builder,
                         indent: indent,
                         target: target,
                         varCounter: ref varCounter);
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            WriteForCollection(type: type,
                               elementType: elementType,
                               builder: builder,
                               indent: indent,
                               target: target,
                               varCounter: ref varCounter);
        }
    }

    static private void WriteForSealedType(INamedTypeSymbol type,
                                           StringBuilder builder,
                                           String indent,
                                           String target,
                                           ref Int32 varCounter)
    {
        builder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
        if (type.SpecialType is SpecialType.System_String)
        {
            builder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            builder.AppendLine($"{indent}{{");
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            // TODO: Type doesn't match
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<Int32>();");
            builder.AppendLine($"{indent}    {target} = new String(MemoryMarshal.Cast<Byte, Char>(buffer[pointer..(pointer + _var{varCounter})]));");
            builder.AppendLine($"{indent}    pointer += _var{varCounter};");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
            return;
        }
        else
        {
            builder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            builder.AppendLine($"{indent}{{");
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            // TODO: Type doesn't match

            String furtherIndent = indent + "    ";
            CreateObject(type: type,
                         builder: builder,
                         indent: furtherIndent,
                         target: target,
                         varCounter: ref varCounter);

            if (type.IsCollection(out ITypeSymbol elementType))
            {
                WriteForCollection(type: type,
                                   elementType: elementType,
                                   builder: builder,
                                   indent: furtherIndent,
                                   varCounter: ref varCounter,
                                   target: target);
            }

            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
        }
    }

    static private void WriteForAbstractType(INamedTypeSymbol type,
                                             StringBuilder builder,
                                             String indent,
                                             String target,
                                             ref Int32 varCounter)
    {
        builder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
        builder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
        builder.AppendLine($"{indent}{{");
        varCounter++;
        builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
        builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        Boolean first = true;
        String typeIdentifier = $"_var{varCounter}";
        foreach (INamedTypeSymbol derivedType in derivedTypes)
        {
            if (first)
            {
                builder.AppendLine($"{indent}    if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
                first = false;
            }
            else
            {
                builder.AppendLine($"{indent}    else if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
            }

            builder.AppendLine($"{indent}    {{");

            String furtherIndent = indent + "        ";
            CreateObject(type: derivedType,
                         builder: builder,
                         indent: furtherIndent,
                         target: target,
                         varCounter: ref varCounter);

            if (type.IsCollection(out ITypeSymbol elementType))
            {
                WriteForCollection(type: type,
                                   elementType: elementType,
                                   builder: builder,
                                   indent: furtherIndent,
                                   varCounter: ref varCounter,
                                   target: target);
            }

            builder.AppendLine($"{indent}    }}");
        }

        builder.AppendLine($"{indent}}}");
        builder.AppendLine($"{indent}else");
        builder.AppendLine($"{indent}{{");
        builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
        builder.AppendLine($"{indent}}}");
    }

    static private void WriteForPolymorphicType(INamedTypeSymbol type,
                                                StringBuilder builder,
                                                String indent,
                                                String target,
                                                ref Int32 varCounter)
    {
        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        String furtherIndent = indent + "        ";

        if (derivedTypes.Length is 0)
        {
            WriteForSealedType(type: type,
                               builder: builder,
                               indent: indent,
                               varCounter: ref varCounter,
                               target: target);
        }
        else
        {
            builder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
            builder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            builder.AppendLine($"{indent}{{");
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            String typeIdentifier = $"_var{varCounter}";
            Boolean first = true;
            foreach (INamedTypeSymbol derivedType in derivedTypes)
            {
                if (first)
                {
                    builder.AppendLine($"{indent}    if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
                    first = false;
                }
                else
                {
                    builder.AppendLine($"{indent}    else if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
                }

                builder.AppendLine($"{indent}    {{");

                CreateObject(type: derivedType,
                             builder: builder,
                             indent: furtherIndent,
                             target: target,
                             varCounter: ref varCounter);

                if (type.IsCollection(out ITypeSymbol derivedElementType))
                {
                    WriteForCollection(type: derivedType,
                                       elementType: derivedElementType,
                                       builder: builder,
                                       indent: furtherIndent,
                                       varCounter: ref varCounter,
                                       target: target);
                }

                builder.AppendLine($"{indent}    }}");
            }

            builder.AppendLine($"{indent}    else if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()})))");
            builder.AppendLine($"{indent}    {{");

            CreateObject(type: type,
                         builder: builder,
                         indent: furtherIndent,
                         target: target,
                         varCounter: ref varCounter);

            if (type.IsCollection(out ITypeSymbol elementType))
            {
                WriteForCollection(type: type,
                                   elementType: elementType,
                                   builder: builder,
                                   indent: furtherIndent,
                                   varCounter: ref varCounter,
                                   target: target);
            }

            builder.AppendLine($"{indent}    }}");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
        }
    }

    static private void WriteForCollection(INamedTypeSymbol type,
                                           ITypeSymbol elementType,
                                           StringBuilder builder,
                                           String indent,
                                           String target,
                                           ref Int32 varCounter)
    {
        varCounter++;
        builder.AppendLine($"{indent}var _var{varCounter} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
        builder.AppendLine($"{indent}pointer += sizeof(Int32);");

        varCounter++;
        builder.AppendLine($"{indent}for (var _var{varCounter} = 0; _var{varCounter} < _var{varCounter - 1}; _var{varCounter}++)");
        builder.AppendLine($"{indent}{{");

        varCounter++;
        String element = $"_var{varCounter}";
        WriteForType(type: elementType,
                     builder: builder,
                     indent: indent + "    ",
                     varCounter: ref varCounter,
                     target: element);

        if (type.GetMembers("Add")
                .OfType<IMethodSymbol>()
                .Any(method => method.DeclaredAccessibility is Accessibility.Public &&
                               method.Parameters.Length is 1 &&
                               SymbolEqualityComparer.Default.Equals(elementType, method.Parameters[0].Type)))
        {
            builder.AppendLine($"{indent}    {target}.Add({element});");
        }
        else
        {
            builder.AppendLine($"{indent}    ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Add({element});");
        }

        builder.AppendLine($"{indent}}}");
    }

    private static void CreateObject(INamedTypeSymbol type,
                                     StringBuilder builder,
                                     String indent,
                                     String target,
                                     ref Int32 varCounter)
    {
        ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
        Dictionary<String, String> propertyMap = new();
        foreach (ISymbol member in members)
        {
            varCounter++;
            propertyMap.Add(key: member.Name,
                            value: $"_var{varCounter}");
            if (member is IFieldSymbol field)
            {
                WriteForType(type: field.Type,
                             builder: builder,
                             indent: indent,
                             varCounter: ref varCounter,
                             target: $"_var{varCounter}");
            }
            else if (member is IPropertySymbol property)
            {
                WriteForType(type: property.Type,
                             builder: builder,
                             indent: indent,
                             varCounter: ref varCounter,
                             target: $"_var{varCounter}");
            }
        }

        if (type.HasDefaultConstructor())
        {
            builder.Append($"{indent}{target} = new {type.ToFrameworkString()}()");
            if (propertyMap.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine($"{indent}{{");
                foreach (KeyValuePair<String, String> property in propertyMap)
                {
                    builder.AppendLine($"{indent}    {property.Key} = {property.Value},");
                }

                builder.AppendLine($"{indent}}};");
            }
            else
            {
                builder.AppendLine(";");
            }
        }
        else if (type.IsRecord)
        {
            List<String> parameters = new();
            StringBuilder initializer = new();
            IMethodSymbol constructor = type.InstanceConstructors.First();
            foreach (KeyValuePair<String, String> property in propertyMap)
            {
                if (constructor.Parameters.Any(parameter => parameter.Name == property.Key))
                {
                    parameters.Add($"{property.Key}: {property.Value}");
                }
                else
                {
                    initializer.AppendLine($"{indent}   {property.Key} = {property.Value},");
                }
            }

            builder.Append($"{indent}{target} = new {type.ToFrameworkString()}({String.Join(", ", parameters)})");
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
            String parameters = String.Join(", ", members.Select(member => $"{propertyMap[member.Name]}"));
            builder.AppendLine($"{indent}{target} = s_Constructor.Value.Invoke({parameters});");
        }
    }

    private static String AssignTo(String target)
    {
        if (target is "result")
        {
            return target;
        }
        else
        {
            return $"var {target}";
        }
    }
}