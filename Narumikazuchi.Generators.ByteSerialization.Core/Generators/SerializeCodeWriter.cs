using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SerializeCodeWriter
{
    static public void WriteMethod(ITypeSymbol type,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public UInt32 Serialize(Span<Byte> buffer, {type.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine("            var pointer = sizeof(Int32);");

        Int32 varCounter = -1;
        WriteForType(type: type,
                     builder: builder,
                     indent: "            ",
                     target: "value",
                     varCounter: ref varCounter);

        builder.AppendLine("            Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer;");
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
                              varCounter: ref varCounter,
                              target: target);
        }
        else if (type.IsValueType)
        {
            WriteForValueType(type: (INamedTypeSymbol)type,
                              builder: builder,
                              indent: indent,
                              varCounter: ref varCounter,
                              target: target);
        }
        else if (type.IsSealed)
        {
            WriteForSealedType(type: (INamedTypeSymbol)type,
                               builder: builder,
                               indent: indent,
                               varCounter: ref varCounter,
                               target: target);
        }
        else if (type.IsAbstract)
        {
            WriteForAbstractType(type: (INamedTypeSymbol)type,
                                 builder: builder,
                                 indent: indent,
                                 varCounter: ref varCounter,
                                 target: target);
        }
        else
        {
            WriteForPolymorphicType(type: (INamedTypeSymbol)type,
                                    builder: builder,
                                    indent: indent,
                                    varCounter: ref varCounter,
                                    target: target);
        }
    }

    static private void WriteForArrayType(IArrayTypeSymbol array,
                                          StringBuilder builder,
                                          String indent,
                                          String target,
                                          ref Int32 varCounter)
    {
        if (array.Rank is 1 &&
            array.ElementType.IsUnmanagedSerializable())
        {
            builder.AppendLine($"{indent}if ({target} is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = MemoryMarshal.AsBytes<{array.ElementType.ToFrameworkString()}>({target});");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var{varCounter}.Length;");
            builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}    _var{varCounter}.CopyTo(buffer[pointer..]);");
            builder.AppendLine($"{indent}    pointer += _var{varCounter}.Length;");
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            builder.AppendLine($"{indent}if ({target} is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                builder.AppendLine($"{indent}    Unsafe.As<Byte, Int32>(ref buffer[pointer]) = {target}.GetLength({index});");
                builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            }

            varCounter++;
            builder.AppendLine($"{indent}    foreach (var _var{varCounter} in {target})");
            builder.AppendLine($"{indent}    {{");

            WriteForType(type: array.ElementType,
                         builder: builder,
                         indent: indent + "        ",
                         varCounter: ref varCounter,
                         target: $"_var{varCounter}");

            builder.AppendLine($"{indent}    }}");
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
            builder.AppendLine($"{indent}Unsafe.As<Byte, {type.ToFrameworkString()}>(ref buffer[pointer]) = {target};");
            builder.AppendLine($"{indent}pointer += Unsafe.SizeOf<{type.ToFrameworkString()}>();");
        }
        else if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
        {
            IPropertySymbol property = type.GetMembers("Key")
                                           .OfType<IPropertySymbol>()
                                           .First();
            WriteForType(type: property.Type,
                         builder: builder,
                         indent: indent,
                         varCounter: ref varCounter,
                         target: $"{target}.Key");

            property = type.GetMembers("Value")
                           .OfType<IPropertySymbol>()
                           .First();
            WriteForType(type: property.Type,
                         builder: builder,
                         indent: indent,
                         varCounter: ref varCounter,
                         target: $"{target}.Value");
        }
        else
        {
            builder.AppendLine($"{indent}Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine($"{indent}pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    WriteForType(type: field.Type,
                                 builder: builder,
                                 indent: indent,
                                 varCounter: ref varCounter,
                                 target: $"{target}.{field.Name}");
                }
                else if (member is IPropertySymbol property)
                {
                    WriteForType(type: property.Type,
                                 builder: builder,
                                 indent: indent,
                                 varCounter: ref varCounter,
                                 target: $"{target}.{property.Name}");
                }
            }
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            WriteForCollection(type: type,
                               elementType: elementType,
                               builder: builder,
                               indent: indent,
                               varCounter: ref varCounter,
                               target: target);
        }
    }

    static private void WriteForSealedType(INamedTypeSymbol type,
                                           StringBuilder builder,
                                           String indent,
                                           String target,
                                           ref Int32 varCounter)
    {
        if (type.SpecialType is SpecialType.System_String)
        {
            builder.AppendLine($"{indent}if ({target} is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            varCounter++;
            builder.AppendLine($"{indent}    var _var{varCounter} = MemoryMarshal.AsBytes({target}.AsSpan());");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var{varCounter}.Length;");
            builder.AppendLine($"{indent}    pointer += sizeof(Int32);");
            builder.AppendLine($"{indent}    _var{varCounter}.CopyTo(buffer[pointer..]);");
            builder.AppendLine($"{indent}    pointer += _var{varCounter}.Length;");
            builder.AppendLine($"{indent}}}");
            return;
        }
        else
        {
            builder.AppendLine($"{indent}if ({target} is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
            String furtherIndent = indent + "    ";
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    WriteForType(type: field.Type,
                                 builder: builder,
                                 indent: furtherIndent,
                                 varCounter: ref varCounter,
                                 target: $"{target}.{field.Name}");
                }
                else if (member is IPropertySymbol property)
                {
                    WriteForType(type: property.Type,
                                 builder: builder,
                                 indent: furtherIndent,
                                 varCounter: ref varCounter,
                                 target: $"{target}.{property.Name}");
                }
            }

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
        }
    }

    static private void WriteForAbstractType(INamedTypeSymbol type,
                                             StringBuilder builder,
                                             String indent,
                                             String target,
                                             ref Int32 varCounter)
    {
        builder.AppendLine($"{indent}if ({target} is null)");
        builder.AppendLine($"{indent}{{");
        builder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
        builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
        builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
        builder.AppendLine($"{indent}}}");
        builder.AppendLine($"{indent}else");
        builder.AppendLine($"{indent}{{");

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        Boolean first = true;
        foreach (INamedTypeSymbol derivedType in derivedTypes)
        {
            varCounter++;
            if (first)
            {
                builder.AppendLine($"{indent}    if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                first = false;
            }
            else
            {
                builder.AppendLine($"{indent}    else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
            }

            builder.AppendLine($"{indent}    {{");
            builder.AppendLine($"{indent}        buffer[pointer++] = 0x1;");
            builder.AppendLine($"{indent}        Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()}));");
            builder.AppendLine($"{indent}        pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            ImmutableArray<ISymbol> members = derivedType.GetMembersToSerialize();
            String furtherIndent = indent + "        ";
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    WriteForType(type: field.Type,
                                 builder: builder,
                                 indent: furtherIndent,
                                 varCounter: ref varCounter,
                                 target: $"_var{varCounter}.{field.Name}");
                }
                else if (member is IPropertySymbol property)
                {
                    WriteForType(type: property.Type,
                                 builder: builder,
                                 indent: furtherIndent,
                                 varCounter: ref varCounter,
                                 target: $"_var{varCounter}.{property.Name}");
                }
            }

            if (derivedType.IsCollection(out ITypeSymbol elementType))
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
    }

    static private void WriteForPolymorphicType(INamedTypeSymbol type,
                                                StringBuilder builder,
                                                String indent,
                                                String target,
                                                ref Int32 varCounter)
    {
        ImmutableArray<ISymbol> members;
        String furtherIndent;

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
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
            builder.AppendLine($"{indent}if ({target} is null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            builder.AppendLine($"{indent}    Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            builder.AppendLine($"{indent}}}");
            builder.AppendLine($"{indent}else");
            builder.AppendLine($"{indent}{{");

            Boolean first = true;
            foreach (INamedTypeSymbol derivedType in derivedTypes)
            {
                varCounter++;
                if (first)
                {
                    builder.AppendLine($"{indent}    if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                    first = false;
                }
                else
                {
                    builder.AppendLine($"{indent}    else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                }

                builder.AppendLine($"{indent}    {{");
                builder.AppendLine($"{indent}        buffer[pointer++] = 0x1;");
                builder.AppendLine($"{indent}        Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()}));");
                builder.AppendLine($"{indent}        pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

                members = derivedType.GetMembersToSerialize();
                furtherIndent = indent + "        ";
                foreach (ISymbol member in members)
                {
                    if (member is IFieldSymbol field)
                    {
                        WriteForType(type: field.Type,
                                     builder: builder,
                                     indent: furtherIndent,
                                     varCounter: ref varCounter,
                                     target: $"_var{varCounter}.{field.Name}");
                    }
                    else if (member is IPropertySymbol property)
                    {
                        WriteForType(type: property.Type,
                                     builder: builder,
                                     indent: furtherIndent,
                                     varCounter: ref varCounter,
                                     target: $"_var{varCounter}.{property.Name}");
                    }
                }

                if (derivedType.IsCollection(out ITypeSymbol derivedElementType))
                {
                    WriteForCollection(type: type,
                                       elementType: derivedElementType,
                                       builder: builder,
                                       indent: furtherIndent,
                                       varCounter: ref varCounter,
                                       target: target);
                }

                builder.AppendLine($"{indent}    }}");
            }

            varCounter++;
            builder.AppendLine($"{indent}    else if ({target} is {type.ToFrameworkString()} _var{varCounter})");
            builder.AppendLine($"{indent}    {{");
            builder.AppendLine($"{indent}        buffer[pointer++] = 0x0;");
            builder.AppendLine($"{indent}        Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref buffer[pointer]) = {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()}));");
            builder.AppendLine($"{indent}        pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            members = type.GetMembersToSerialize();
            furtherIndent = indent + "        ";
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    WriteForType(type: field.Type,
                                 builder: builder,
                                 indent: furtherIndent,
                                 varCounter: ref varCounter,
                                 target: $"_var{varCounter}.{field.Name}");
                }
                else if (member is IPropertySymbol property)
                {
                    WriteForType(type: property.Type,
                                 builder: builder,
                                 indent: furtherIndent,
                                 varCounter: ref varCounter,
                                 target: $"_var{varCounter}.{property.Name}");
                }
            }

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
        }
    }

    static private void WriteForCollection(INamedTypeSymbol type,
                                           ITypeSymbol elementType,
                                           StringBuilder builder,
                                           String indent,
                                           String target,
                                           ref Int32 varCounter)
    {
        if (type.GetMembers("Count")
                .OfType<IPropertySymbol>()
                .Any(property => property.DeclaredAccessibility is Accessibility.Public))
        {
            builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = {target}.Count;");
            builder.AppendLine($"{indent}pointer += sizeof(Int32);");
        }
        else
        {
            builder.AppendLine($"{indent}Unsafe.As<Byte, Int32>(ref buffer[pointer]) = ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Count;");
            builder.AppendLine($"{indent}pointer += sizeof(Int32);");
        }

        varCounter++;
        builder.AppendLine($"{indent}foreach (var _var{varCounter} in {target})");
        builder.AppendLine($"{indent}{{");

        WriteForType(type: elementType,
                     builder: builder,
                     indent: indent + "    ",
                     varCounter: ref varCounter,
                     target: $"_var{varCounter}");

        builder.AppendLine($"{indent}}}");
    }
}