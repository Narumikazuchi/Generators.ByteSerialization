using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public sealed class SerializeCodeWriter
{
    public SerializeCodeWriter(ImmutableArray<IAssemblySymbol> assemblies,
                               ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> customSerializers,
                               Logger logger)
    {
        m_Assemblies = assemblies;
        m_CustomSerializers = customSerializers;
        m_CustomSerializerVars = new(SymbolEqualityComparer.Default);
        m_SerializerBuilder = new();
        m_CodeBuilder = new();
        m_Logger = logger;
    }

    public String WriteMethod(ITypeSymbol type)
    {
        StringBuilder builder = new();
        builder.AppendLine("    [System.Runtime.CompilerServices.CompilerGenerated]");
        builder.AppendLine($"    {GlobalNames.NAMESPACE}.Unsigned31BitInteger {GlobalNames.ISerializationHandler(type)}.Serialize(Span<System.Byte> buffer, {type.ToFrameworkString()} value)");
        builder.AppendLine("    {");
        builder.AppendLine("        var pointer = sizeof(System.Int32);");

        Int32 varCounter = -1;
        this.WriteForType(type: type,
                          indent: "        ",
                          target: "value",
                          varCounter: ref varCounter);

        builder.Append(m_SerializerBuilder.ToString());
        builder.Append(m_CodeBuilder.ToString());

        builder.AppendLine("        System.Runtime.CompilerServices.Unsafe.As<System.Byte, System.Int32>(ref buffer[0]) = pointer;");
        builder.AppendLine("        return pointer;");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private void WriteForType(ITypeSymbol type,
                              String indent,
                              String target,
                              ref Int32 varCounter)
    {
        m_Logger.LogInformation($"Writing serialization code for target '{target}' of type '{type.ToFrameworkString()}'.");
        if (m_CustomSerializers.TryGetValue(key: type,
                                            value: out ImmutableHashSet<INamedTypeSymbol> implementingTypes))
        {
            if (!m_CustomSerializerVars.TryGetValue(key: type,
                                                    value: out String serializer))
            {
                varCounter++;
                serializer = $"_var{varCounter}";
                m_CustomSerializerVars.Add(key: type,
                                           value: serializer);
                m_SerializerBuilder.AppendLine($"        {GlobalNames.ISerializationHandler(type)} {serializer} = new {implementingTypes.First().ToFrameworkString()}();");
            }

            m_Logger.LogInformation($"Using custom serializer for serialization code for target '{target}' of type '{type.ToFrameworkString()}'.");
            m_CodeBuilder.AppendLine($"{indent}pointer += {serializer}.Serialize(buffer[pointer..], {target});");
        }
        else if (type is IArrayTypeSymbol array)
        {
            this.WriteForArrayType(array: array,
                                   indent: indent,
                                   varCounter: ref varCounter,
                                   target: target);
        }
        else if (type.IsValueType)
        {
            this.WriteForValueType(type: (INamedTypeSymbol)type,
                                   indent: indent,
                                   varCounter: ref varCounter,
                                   target: target);
        }
        else if (type.IsSealed)
        {
            this.WriteForSealedType(type: (INamedTypeSymbol)type,
                                    indent: indent,
                                    varCounter: ref varCounter,
                                    target: target);
        }
        else if (type.IsAbstract)
        {
            this.WriteForAbstractType(type: (INamedTypeSymbol)type,
                                      indent: indent,
                                      varCounter: ref varCounter,
                                      target: target);
        }
        else
        {
            this.WriteForPolymorphicType(type: (INamedTypeSymbol)type,
                                         indent: indent,
                                         varCounter: ref varCounter,
                                         target: target);
        }
    }

    private void WriteForArrayType(IArrayTypeSymbol array,
                                   String indent,
                                   String target,
                                   ref Int32 varCounter)
    {
        if (array.Rank is 1 &&
            array.ElementType.IsUnmanagedStruct())
        {
            m_Logger.LogInformation($"Target '{target}' is handled as an array of unmanaged types.");
            m_CodeBuilder.AppendLine($"{indent}if ({target} is null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{array.ToFrameworkString()}>());");
            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{array.ToFrameworkString()}>());");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = System.Runtime.InteropServices.MemoryMarshal.AsBytes<{array.ElementType.ToFrameworkString()}>({target});");
            m_CodeBuilder.AppendLine($"{indent}    System.Runtime.CompilerServices.Unsafe.As<System.Byte, System.Int32>(ref buffer[pointer]) = _var{varCounter}.Length;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += sizeof(System.Int32);");
            m_CodeBuilder.AppendLine($"{indent}    _var{varCounter}.CopyTo(buffer[pointer..]);");
            m_CodeBuilder.AppendLine($"{indent}    pointer += _var{varCounter}.Length;");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
        else
        {
            m_Logger.LogInformation($"Target '{target}' is handled as a regular array.");
            m_CodeBuilder.AppendLine($"{indent}if ({target} is null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{array.ToFrameworkString()}>());");
            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{array.ToFrameworkString()}>());");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                m_CodeBuilder.AppendLine($"{indent}    System.Runtime.CompilerServices.Unsafe.As<System.Byte, System.Int32>(ref buffer[pointer]) = {target}.GetLength({index});");
                m_CodeBuilder.AppendLine($"{indent}    pointer += sizeof(System.Int32);");
            }

            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    foreach (var _var{varCounter} in {target})");
            m_CodeBuilder.AppendLine($"{indent}    {{");

            this.WriteForType(type: array.ElementType,
                              indent: indent + "        ",
                              varCounter: ref varCounter,
                              target: $"_var{varCounter}");

            m_CodeBuilder.AppendLine($"{indent}    }}");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
    }

    private void WriteForValueType(INamedTypeSymbol type,
                                   String indent,
                                   String target,
                                   ref Int32 varCounter)
    {
        if (type.IsUnmanagedSerializable())
        {
            m_Logger.LogInformation($"Target '{target}' is handled as an unmanaged type.");
            m_CodeBuilder.AppendLine($"{indent}System.Runtime.CompilerServices.Unsafe.As<System.Byte, {type.ToFrameworkString()}>(ref buffer[pointer]) = {target};");
            m_CodeBuilder.AppendLine($"{indent}pointer += System.Runtime.CompilerServices.Unsafe.SizeOf<{type.ToFrameworkString()}>();");
        }
        else if (type.IsUnmanagedStruct())
        {
            m_Logger.LogInformation($"Target '{target}' is handled as an unmanaged type.");
            m_CodeBuilder.AppendLine($"{indent}buffer[pointer++] = 0x1;");
            m_CodeBuilder.AppendLine($"{indent}pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");
            m_CodeBuilder.AppendLine($"{indent}System.Runtime.CompilerServices.Unsafe.As<System.Byte, {type.ToFrameworkString()}>(ref buffer[pointer]) = {target};");
            m_CodeBuilder.AppendLine($"{indent}pointer += System.Runtime.CompilerServices.Unsafe.SizeOf<{type.ToFrameworkString()}>();");
        }
        else if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
        {
            m_Logger.LogInformation($"Target '{target}' is handled as special type 'KeyValuePair`2'.");
            IPropertySymbol property = type.GetMembers("Key")
                                           .OfType<IPropertySymbol>()
                                           .First();
            this.WriteForType(type: property.Type,
                              indent: indent,
                              varCounter: ref varCounter,
                              target: $"{target}.Key");

            property = type.GetMembers("Value")
                           .OfType<IPropertySymbol>()
                           .First();
            this.WriteForType(type: property.Type,
                              indent: indent,
                              varCounter: ref varCounter,
                              target: $"{target}.Value");
        }
        else
        {
            m_Logger.LogInformation($"Target '{target}' is handled as managed struct.");
            m_CodeBuilder.AppendLine($"{indent}buffer[pointer++] = 0x1;");
            m_CodeBuilder.AppendLine($"{indent}pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");

            ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    this.WriteForType(type: field.Type,
                                      indent: indent,
                                      varCounter: ref varCounter,
                                      target: $"{target}.{field.Name}");
                }
                else if (member is IPropertySymbol property)
                {
                    this.WriteForType(type: property.Type,
                                      indent: indent,
                                      varCounter: ref varCounter,
                                      target: $"{target}.{property.Name}");
                }
            }
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            this.WriteForCollection(type: type,
                                    elementType: elementType,
                                    indent: indent,
                                    varCounter: ref varCounter,
                                    target: target);
        }
    }

    private void WriteForSealedType(INamedTypeSymbol type,
                                    String indent,
                                    String target,
                                    ref Int32 varCounter)
    {
        if (type.SpecialType is SpecialType.System_String)
        {
            m_Logger.LogInformation($"Target '{target}' is handled as special type 'String'.");
            m_CodeBuilder.AppendLine($"{indent}if ({target} is null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");
            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = System.Runtime.InteropServices.MemoryMarshal.AsBytes({target}.AsSpan());");
            m_CodeBuilder.AppendLine($"{indent}    System.Runtime.CompilerServices.Unsafe.As<System.Byte, System.Int32>(ref buffer[pointer]) = _var{varCounter}.Length;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += sizeof(System.Int32);");
            m_CodeBuilder.AppendLine($"{indent}    _var{varCounter}.CopyTo(buffer[pointer..]);");
            m_CodeBuilder.AppendLine($"{indent}    pointer += _var{varCounter}.Length;");
            m_CodeBuilder.AppendLine($"{indent}}}");
            return;
        }
        else
        {
            m_Logger.LogInformation($"Target '{target}' is handled as sealed class.");
            m_CodeBuilder.AppendLine($"{indent}if ({target} is null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");
            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x1;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");

            ImmutableArray<ISymbol> members = type.GetMembersToSerialize();
            String furtherIndent = indent + "    ";
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    this.WriteForType(type: field.Type,
                                      indent: furtherIndent,
                                      varCounter: ref varCounter,
                                      target: $"{target}.{field.Name}");
                }
                else if (member is IPropertySymbol property)
                {
                    this.WriteForType(type: property.Type,
                                      indent: furtherIndent,
                                      varCounter: ref varCounter,
                                      target: $"{target}.{property.Name}");
                }
            }

            if (type.IsCollection(out ITypeSymbol elementType))
            {
                this.WriteForCollection(type: type,
                                        elementType: elementType,
                                        indent: furtherIndent,
                                        varCounter: ref varCounter,
                                        target: target);
            }

            m_CodeBuilder.AppendLine($"{indent}}}");
        }
    }

    private void WriteForAbstractType(INamedTypeSymbol type,
                                      String indent,
                                      String target,
                                      ref Int32 varCounter)
    {
        m_Logger.LogInformation($"Target '{target}' is handled as abstract class.");
        m_CodeBuilder.AppendLine($"{indent}if ({target} is null)");
        m_CodeBuilder.AppendLine($"{indent}{{");
        m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
        m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");
        m_CodeBuilder.AppendLine($"{indent}}}");
        m_CodeBuilder.AppendLine($"{indent}else");
        m_CodeBuilder.AppendLine($"{indent}{{");

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes(m_Assemblies);
        Boolean first = true;
        foreach (INamedTypeSymbol derivedType in derivedTypes)
        {
            varCounter++;
            if (first)
            {
                m_CodeBuilder.AppendLine($"{indent}    if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                first = false;
            }
            else
            {
                m_CodeBuilder.AppendLine($"{indent}    else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
            }

            m_CodeBuilder.AppendLine($"{indent}    {{");

            String furtherIndent = indent + "        ";
            if (derivedType.IsUnmanagedSerializable())
            {
                m_CodeBuilder.AppendLine($"{indent}        System.Runtime.CompilerServices.Unsafe.As<System.Byte, {derivedType.ToFrameworkString()}>(ref buffer[pointer]);");
                m_CodeBuilder.AppendLine($"{indent}        pointer += System.Runtime.CompilerServices.Unsafe.SizeOf<{derivedType.ToFrameworkString()}>();");
            }
            else if (derivedType.IsUnmanagedStruct())
            {
                m_CodeBuilder.AppendLine($"{indent}        buffer[pointer++] = 0x1;");
                m_CodeBuilder.AppendLine($"{indent}        pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{derivedType.ToFrameworkString()}>());");
                m_CodeBuilder.AppendLine($"{indent}        System.Runtime.CompilerServices.Unsafe.As<System.Byte, {derivedType.ToFrameworkString()}>(ref buffer[pointer]) = _var{varCounter};");
                m_CodeBuilder.AppendLine($"{indent}        pointer += System.Runtime.CompilerServices.Unsafe.SizeOf<{derivedType.ToFrameworkString()}>();");
            }
            else
            {
                m_CodeBuilder.AppendLine($"{indent}        buffer[pointer++] = 0x1;");
                m_CodeBuilder.AppendLine($"{indent}        pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{derivedType.ToFrameworkString()}>());");

                ImmutableArray<ISymbol> members = derivedType.GetMembersToSerialize();
                foreach (ISymbol member in members)
                {
                    if (member is IFieldSymbol field)
                    {
                        this.WriteForType(type: field.Type,
                                          indent: furtherIndent,
                                          varCounter: ref varCounter,
                                          target: $"_var{varCounter}.{field.Name}");
                    }
                    else if (member is IPropertySymbol property)
                    {
                        this.WriteForType(type: property.Type,
                                          indent: furtherIndent,
                                          varCounter: ref varCounter,
                                          target: $"_var{varCounter}.{property.Name}");
                    }
                }
            }

            if (derivedType.IsCollection(out ITypeSymbol elementType))
            {
                this.WriteForCollection(type: type,
                                        elementType: elementType,
                                        indent: furtherIndent,
                                        varCounter: ref varCounter,
                                        target: target);
            }

            m_CodeBuilder.AppendLine($"{indent}    }}");
        }

        m_CodeBuilder.AppendLine($"{indent}}}");
    }

    private void WriteForPolymorphicType(INamedTypeSymbol type,
                                         String indent,
                                         String target,
                                         ref Int32 varCounter)
    {
        ImmutableArray<ISymbol> members;
        String furtherIndent;

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes(m_Assemblies);
        if (derivedTypes.Length is 0)
        {
            this.WriteForSealedType(type: type,
                                    indent: indent,
                                    varCounter: ref varCounter,
                                    target: target);
        }
        else
        {
            m_Logger.LogInformation($"Target '{target}' is handled as polymorphic non-sealed class.");
            m_CodeBuilder.AppendLine($"{indent}if ({target} is null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    buffer[pointer++] = 0x0;");
            m_CodeBuilder.AppendLine($"{indent}    pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");
            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");

            Boolean first = true;
            foreach (INamedTypeSymbol derivedType in derivedTypes)
            {
                varCounter++;
                if (first)
                {
                    m_CodeBuilder.AppendLine($"{indent}    if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                    first = false;
                }
                else
                {
                    m_CodeBuilder.AppendLine($"{indent}    else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                }

                m_CodeBuilder.AppendLine($"{indent}    {{");
                m_CodeBuilder.AppendLine($"{indent}        buffer[pointer++] = 0x1;");
                m_CodeBuilder.AppendLine($"{indent}        pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{derivedType.ToFrameworkString()}>());");

                members = derivedType.GetMembersToSerialize();
                furtherIndent = indent + "        ";
                foreach (ISymbol member in members)
                {
                    if (member is IFieldSymbol field)
                    {
                        this.WriteForType(type: field.Type,
                                          indent: furtherIndent,
                                          varCounter: ref varCounter,
                                          target: $"_var{varCounter}.{field.Name}");
                    }
                    else if (member is IPropertySymbol property)
                    {
                        this.WriteForType(type: property.Type,
                                          indent: furtherIndent,
                                          varCounter: ref varCounter,
                                          target: $"_var{varCounter}.{property.Name}");
                    }
                }

                if (derivedType.IsCollection(out ITypeSymbol derivedElementType))
                {
                    this.WriteForCollection(type: type,
                                            elementType: derivedElementType,
                                            indent: furtherIndent,
                                            varCounter: ref varCounter,
                                            target: target);
                }

                m_CodeBuilder.AppendLine($"{indent}    }}");
            }

            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    else if ({target} is {type.ToFrameworkString()} _var{varCounter})");
            m_CodeBuilder.AppendLine($"{indent}    {{");
            m_CodeBuilder.AppendLine($"{indent}        buffer[pointer++] = 0x0;");
            m_CodeBuilder.AppendLine($"{indent}        pointer += {GlobalNames.NAMESPACE}.TypeLayoutSerializationHandler.Default.Serialize(buffer[pointer..], {GlobalNames.NAMESPACE}.TypeLayout.CreateFrom<{type.ToFrameworkString()}>());");

            members = type.GetMembersToSerialize();
            furtherIndent = indent + "        ";
            foreach (ISymbol member in members)
            {
                if (member is IFieldSymbol field)
                {
                    this.WriteForType(type: field.Type,
                                      indent: furtherIndent,
                                      varCounter: ref varCounter,
                                      target: $"_var{varCounter}.{field.Name}");
                }
                else if (member is IPropertySymbol property)
                {
                    this.WriteForType(type: property.Type,
                                      indent: furtherIndent,
                                      varCounter: ref varCounter,
                                      target: $"_var{varCounter}.{property.Name}");
                }
            }

            if (type.IsCollection(out ITypeSymbol elementType))
            {
                this.WriteForCollection(type: type,
                                        elementType: elementType,
                                        indent: furtherIndent,
                                        varCounter: ref varCounter,
                                        target: target);
            }

            m_CodeBuilder.AppendLine($"{indent}    }}");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
    }

    private void WriteForCollection(INamedTypeSymbol type,
                                    ITypeSymbol elementType,
                                    String indent,
                                    String target,
                                    ref Int32 varCounter)
    {
        m_Logger.LogInformation($"Target '{target}' is handled as collection type.");
        if (type.GetMembers("Count")
                .OfType<IPropertySymbol>()
                .Any(property => property.DeclaredAccessibility is Accessibility.Public))
        {
            m_CodeBuilder.AppendLine($"{indent}System.Runtime.CompilerServices.Unsafe.As<System.Byte, System.Int32>(ref buffer[pointer]) = {target}.Count;");
            m_CodeBuilder.AppendLine($"{indent}pointer += sizeof(System.Int32);");
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}System.Runtime.CompilerServices.Unsafe.As<System.Byte, System.Int32>(ref buffer[pointer]) = ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Count;");
            m_CodeBuilder.AppendLine($"{indent}pointer += sizeof(System.Int32);");
        }

        varCounter++;
        m_CodeBuilder.AppendLine($"{indent}foreach (var _var{varCounter} in {target})");
        m_CodeBuilder.AppendLine($"{indent}{{");

        this.WriteForType(type: elementType,
                          indent: indent + "    ",
                          varCounter: ref varCounter,
                          target: $"_var{varCounter}");

        m_CodeBuilder.AppendLine($"{indent}}}");
    }

    private readonly ImmutableArray<IAssemblySymbol> m_Assemblies;
    private readonly Dictionary<ITypeSymbol, String> m_CustomSerializerVars;
    private readonly ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> m_CustomSerializers;
    private readonly StringBuilder m_SerializerBuilder;
    private readonly StringBuilder m_CodeBuilder;
    private readonly Logger m_Logger;
}