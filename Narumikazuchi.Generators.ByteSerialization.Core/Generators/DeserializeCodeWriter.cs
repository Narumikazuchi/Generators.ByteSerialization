using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public sealed class DeserializeCodeWriter
{
    public DeserializeCodeWriter(ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> customSerializers)
    {
        m_CustomSerializers = customSerializers;
        m_CustomSerializerVars = new(SymbolEqualityComparer.Default);
        m_SerializerBuilder = new();
        m_CodeBuilder = new();
    }

    public String WriteMethod(ITypeSymbol type)
    {
        StringBuilder builder = new();
        builder.AppendLine("    [CompilerGenerated]");
        builder.AppendLine($"    UInt32 {GlobalNames.ISerializationHandler(type)}.Deserialize(ReadOnlySpan<Byte> buffer, out {type.ToFrameworkString()} result)");
        builder.AppendLine("    {");
        builder.AppendLine("        var pointer = sizeof(Int32);");

        Int32 varCounter = -1;
        this.WriteForType(type: type,
                          indent: "        ",
                          target: "result",
                          varCounter: ref varCounter);

        builder.Append(m_SerializerBuilder.ToString());
        builder.Append(m_CodeBuilder.ToString());

        builder.AppendLine("        return (UInt32)pointer;");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private void WriteForType(ITypeSymbol type,
                              String indent,
                              String target,
                              ref Int32 varCounter)
    {
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

            m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
            m_CodeBuilder.AppendLine($"{indent}pointer += (Int32){serializer}.Deserialize(buffer[pointer..], out {target});");
        }
        else if (type is IArrayTypeSymbol array)
        {
            this.WriteForArrayType(array: array,
                                   indent: indent,
                                   target: target,
                                   varCounter: ref varCounter);
        }
        else if (type.IsValueType)
        {
            this.WriteForValueType(type: (INamedTypeSymbol)type,
                                   indent: indent,
                                   target: target,
                                   varCounter: ref varCounter);
        }
        else if (type.IsSealed)
        {
            this.WriteForSealedType(type: (INamedTypeSymbol)type,
                                    indent: indent,
                                    target: target,
                                    varCounter: ref varCounter);
        }
        else if (type.IsAbstract)
        {
            this.WriteForAbstractType(type: (INamedTypeSymbol)type,
                                      indent: indent,
                                      target: target,
                                      varCounter: ref varCounter);
        }
        else
        {
            this.WriteForPolymorphicType(type: (INamedTypeSymbol)type,
                                         indent: indent,
                                         target: target,
                                         varCounter: ref varCounter);
        }
    }

    private void WriteForArrayType(IArrayTypeSymbol array,
                                   String indent,
                                   String target,
                                   ref Int32 varCounter)
    {
        m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = default({array.ToFrameworkString()});");
        if (array.Rank is 1 &&
            array.ElementType.IsUnmanagedSerializable())
        {
            m_CodeBuilder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}    if (_var{varCounter} != {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()})))");
            m_CodeBuilder.AppendLine($"{indent}    {{");
            m_CodeBuilder.AppendLine($"{indent}        throw new {GlobalNames.NAMESPACE}.WrongTypeDeserialization(typeof({array.ToFrameworkString()}));");
            m_CodeBuilder.AppendLine($"{indent}    }}");

            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<Int32>();");
            m_CodeBuilder.AppendLine($"{indent}    {target} = MemoryMarshal.Cast<Byte, {array.ElementType.ToFrameworkString()}>(buffer[pointer..(pointer + _var{varCounter})]).ToArray();");
            m_CodeBuilder.AppendLine($"{indent}    pointer += _var{varCounter};");
            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}    if (_var{varCounter} != {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({array.ToFrameworkString()})))");
            m_CodeBuilder.AppendLine($"{indent}    {{");
            m_CodeBuilder.AppendLine($"{indent}        throw new {GlobalNames.NAMESPACE}.WrongTypeDeserialization(typeof({array.ToFrameworkString()}));");
            m_CodeBuilder.AppendLine($"{indent}    }}");

            String[] arraySizes = new String[array.Rank];
            Int32 arrayCounter = ++varCounter;
            varCounter += 2 * array.Rank;
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                m_CodeBuilder.AppendLine($"{indent}    var _var{arrayCounter + index} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
                m_CodeBuilder.AppendLine($"{indent}    pointer += sizeof(Int32);");
                arraySizes[index] = $"_var{arrayCounter + index}";
            }

            m_CodeBuilder.AppendLine($"{indent}    {target} = {array.CreateArray(arraySizes)};");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                m_CodeBuilder.AppendLine($"{indent}    for (var _var{arrayCounter + array.Rank + index} = 0; _var{arrayCounter + array.Rank + index} < _var{arrayCounter + index}; _var{arrayCounter + array.Rank + index}++)");
                m_CodeBuilder.AppendLine($"{indent}    {{");
                indent += "    ";
            }

            this.WriteForType(type: array.ElementType,
                              indent: indent + "    ",
                              target: $"_var{arrayCounter + 2 * array.Rank}",
                              varCounter: ref varCounter);

            m_CodeBuilder.Append($"{indent}    {target}[");
            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                if (index > 0)
                {
                    m_CodeBuilder.Append(", ");
                }

                m_CodeBuilder.Append($"_var{arrayCounter + array.Rank + index}");
            }

            m_CodeBuilder.AppendLine($"] = _var{arrayCounter + 2 * array.Rank};");

            for (Int32 index = 0;
                 index < array.Rank;
                 index++)
            {
                m_CodeBuilder.AppendLine($"{indent}}}");
                indent = indent.Substring(4);
            }

            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
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
            m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = Unsafe.As<Byte, {type.ToFrameworkString()}>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}pointer += Unsafe.SizeOf<{type.ToFrameworkString()}>();");
        }
        else if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
        {
            IPropertySymbol property = type.GetMembers("Key")
                                           .OfType<IPropertySymbol>()
                                           .First();
            varCounter++;
            String key = $"_var{varCounter}";
            this.WriteForType(type: property.Type,
                              indent: indent,
                              target: key,
                              varCounter: ref varCounter);

            property = type.GetMembers("Value")
                           .OfType<IPropertySymbol>()
                           .First();
            varCounter++;
            String value = $"_var{varCounter}";
            this.WriteForType(type: property.Type,
                              indent: indent,
                              target: value,
                              varCounter: ref varCounter);
            m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = new {type.ToFrameworkString()}({key}, {value});");
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}    if (_var{varCounter} != {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()})))");
            m_CodeBuilder.AppendLine($"{indent}    {{");
            m_CodeBuilder.AppendLine($"{indent}        throw new {GlobalNames.NAMESPACE}.WrongTypeDeserialization(typeof({type.ToFrameworkString()}));");
            m_CodeBuilder.AppendLine($"{indent}    }}");

            this.CreateObject(type: type,
                              indent: indent,
                              target: target,
                              varCounter: ref varCounter);
        }

        if (type.IsCollection(out ITypeSymbol elementType))
        {
            this.WriteForCollection(type: type,
                                    elementType: elementType,
                                    indent: indent,
                                    target: target,
                                    varCounter: ref varCounter);
        }
    }

    private void WriteForSealedType(INamedTypeSymbol type,
                                    String indent,
                                    String target,
                                    ref Int32 varCounter)
    {
        m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
        if (type.SpecialType is SpecialType.System_String)
        {
            m_CodeBuilder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}    if (_var{varCounter} != {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()})))");
            m_CodeBuilder.AppendLine($"{indent}    {{");
            m_CodeBuilder.AppendLine($"{indent}        throw new {GlobalNames.NAMESPACE}.WrongTypeDeserialization(typeof({type.ToFrameworkString()}));");
            m_CodeBuilder.AppendLine($"{indent}    }}");

            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<Int32>();");
            m_CodeBuilder.AppendLine($"{indent}    {target} = new String(MemoryMarshal.Cast<Byte, Char>(buffer[pointer..(pointer + _var{varCounter})]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += _var{varCounter};");
            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}}}");
            return;
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}    if (_var{varCounter} != {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()})))");
            m_CodeBuilder.AppendLine($"{indent}    {{");
            m_CodeBuilder.AppendLine($"{indent}        throw new {GlobalNames.NAMESPACE}.WrongTypeDeserialization(typeof({type.ToFrameworkString()}));");
            m_CodeBuilder.AppendLine($"{indent}    }}");

            String furtherIndent = indent + "    ";
            this.CreateObject(type: type,
                              indent: furtherIndent,
                              target: target,
                              varCounter: ref varCounter);

            if (type.IsCollection(out ITypeSymbol elementType))
            {
                this.WriteForCollection(type: type,
                                        elementType: elementType,
                                        indent: furtherIndent,
                                        varCounter: ref varCounter,
                                        target: target);
            }

            m_CodeBuilder.AppendLine($"{indent}}}");
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
    }

    private void WriteForAbstractType(INamedTypeSymbol type,
                                      String indent,
                                      String target,
                                      ref Int32 varCounter)
    {
        m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
        m_CodeBuilder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
        m_CodeBuilder.AppendLine($"{indent}{{");
        varCounter++;
        m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
        m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        Boolean first = true;
        String typeIdentifier = $"_var{varCounter}";
        foreach (INamedTypeSymbol derivedType in derivedTypes)
        {
            if (first)
            {
                m_CodeBuilder.AppendLine($"{indent}    if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
                first = false;
            }
            else
            {
                m_CodeBuilder.AppendLine($"{indent}    else if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
            }

            m_CodeBuilder.AppendLine($"{indent}    {{");

            String furtherIndent = indent + "        ";
            this.CreateObject(type: derivedType,
                              indent: furtherIndent,
                              target: target,
                              varCounter: ref varCounter);

            if (type.IsCollection(out ITypeSymbol elementType))
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
        m_CodeBuilder.AppendLine($"{indent}else");
        m_CodeBuilder.AppendLine($"{indent}{{");
        m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
        m_CodeBuilder.AppendLine($"{indent}}}");
    }

    private void WriteForPolymorphicType(INamedTypeSymbol type,
                                         String indent,
                                         String target,
                                         ref Int32 varCounter)
    {
        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        String furtherIndent = indent + "        ";

        if (derivedTypes.Length is 0)
        {
            this.WriteForSealedType(type: type,
                                    indent: indent,
                                    varCounter: ref varCounter,
                                    target: target);
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}{AssignTo(target)} = default({type.ToFrameworkString()});");
            m_CodeBuilder.AppendLine($"{indent}if (buffer[pointer++] == 0x1)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    var _var{varCounter} = Unsafe.As<Byte, {GlobalNames.NAMESPACE}.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            String typeIdentifier = $"_var{varCounter}";
            Boolean first = true;
            foreach (INamedTypeSymbol derivedType in derivedTypes)
            {
                if (first)
                {
                    m_CodeBuilder.AppendLine($"{indent}    if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
                    first = false;
                }
                else
                {
                    m_CodeBuilder.AppendLine($"{indent}    else if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({derivedType.ToFrameworkString()})))");
                }

                m_CodeBuilder.AppendLine($"{indent}    {{");

                this.CreateObject(type: derivedType,
                                  indent: furtherIndent,
                                  target: target,
                                  varCounter: ref varCounter);

                if (type.IsCollection(out ITypeSymbol derivedElementType))
                {
                    this.WriteForCollection(type: derivedType,
                                            elementType: derivedElementType,
                                            indent: furtherIndent,
                                            varCounter: ref varCounter,
                                            target: target);
                }

                m_CodeBuilder.AppendLine($"{indent}    }}");
            }

            m_CodeBuilder.AppendLine($"{indent}    else if ({typeIdentifier} == {GlobalNames.NAMESPACE}.TypeIdentifier.CreateFrom(typeof({type.ToFrameworkString()})))");
            m_CodeBuilder.AppendLine($"{indent}    {{");

            this.CreateObject(type: type,
                              indent: furtherIndent,
                              target: target,
                              varCounter: ref varCounter);

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
            m_CodeBuilder.AppendLine($"{indent}else");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    pointer += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
    }

    private void WriteForCollection(INamedTypeSymbol type,
                                    ITypeSymbol elementType,
                                    String indent,
                                    String target,
                                    ref Int32 varCounter)
    {
        varCounter++;
        m_CodeBuilder.AppendLine($"{indent}var _var{varCounter} = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));");
        m_CodeBuilder.AppendLine($"{indent}pointer += sizeof(Int32);");

        varCounter++;
        m_CodeBuilder.AppendLine($"{indent}for (var _var{varCounter} = 0; _var{varCounter} < _var{varCounter - 1}; _var{varCounter}++)");
        m_CodeBuilder.AppendLine($"{indent}{{");

        varCounter++;
        String element = $"_var{varCounter}";
        this.WriteForType(type: elementType,
                          indent: indent + "    ",
                          varCounter: ref varCounter,
                          target: element);

        if (type.GetMembers("Add")
                .OfType<IMethodSymbol>()
                .Any(method => method.DeclaredAccessibility is Accessibility.Public &&
                               method.Parameters.Length is 1 &&
                               SymbolEqualityComparer.Default.Equals(elementType, method.Parameters[0].Type)))
        {
            m_CodeBuilder.AppendLine($"{indent}    {target}.Add({element});");
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}    ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Add({element});");
        }

        m_CodeBuilder.AppendLine($"{indent}}}");
    }

    private void CreateObject(INamedTypeSymbol type,
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
                this.WriteForType(type: field.Type,
                                  indent: indent,
                                  varCounter: ref varCounter,
                                  target: $"_var{varCounter}");
            }
            else if (member is IPropertySymbol property)
            {
                this.WriteForType(type: property.Type,
                                  indent: indent,
                                  varCounter: ref varCounter,
                                  target: $"_var{varCounter}");
            }
        }

        if (type.HasDefaultConstructor())
        {
            m_CodeBuilder.Append($"{indent}{target} = new {type.ToFrameworkString()}()");
            if (propertyMap.Count > 0)
            {
                m_CodeBuilder.AppendLine();
                m_CodeBuilder.AppendLine($"{indent}{{");
                foreach (KeyValuePair<String, String> property in propertyMap)
                {
                    m_CodeBuilder.AppendLine($"{indent}    {property.Key} = {property.Value},");
                }

                m_CodeBuilder.AppendLine($"{indent}}};");
            }
            else
            {
                m_CodeBuilder.AppendLine(";");
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

            m_CodeBuilder.Append($"{indent}{target} = new {type.ToFrameworkString()}({String.Join(", ", parameters)})");
            if (initializer.Length > 0)
            {
                m_CodeBuilder.AppendLine();
                m_CodeBuilder.AppendLine($"{indent}{{");
                m_CodeBuilder.Append(initializer.ToString());
                m_CodeBuilder.AppendLine($"{indent}}};");
            }
            else
            {
                m_CodeBuilder.AppendLine(";");
            }
        }
        else
        {
            List<String> usedMembers = new();
            IMethodSymbol constructor = type.ParameterizedConstructor();
            StringBuilder parameters = new();
            Boolean first = true;
            foreach (IParameterSymbol parameter in constructor.Parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    parameters.Append(", ");
                }

                AttributeData attribute = parameter.GetAttributes()
                                                   .SingleOrDefault(data => data.AttributeClass is not null &&
                                                                            data.AttributeClass.ToFrameworkString().StartsWith(GlobalNames.SERIALIZEDEFAULTATTRIBUTE));
                if (attribute is not null)
                {
                    if (attribute.ConstructorArguments[0].IsNull)
                    {
                        parameters.Append("null");
                    }
                    else
                    {
                        parameters.Append(attribute.ConstructorArguments[0].Value.ToString());
                    }
                }
                else
                {
                    attribute = parameter.GetAttributes()
                                         .Single(data => data.AttributeClass is not null &&
                                                         data.AttributeClass.ToFrameworkString().StartsWith(GlobalNames.SERIALIZEFROMMEMBERATTRIBUTE));
                    String memberName = attribute.ConstructorArguments[0].Value.ToString();
                    usedMembers.Add(memberName);
                    parameters.Append(propertyMap[memberName]);
                }
            }

            StringBuilder initializer = new();
            foreach (KeyValuePair<String, String> property in propertyMap)
            {
                if (!usedMembers.Contains(property.Key))
                {
                    initializer.AppendLine($"{indent}   {property.Key} = {property.Value},");
                }
            }

            m_CodeBuilder.Append($"{indent}{target} = new {type.ToFrameworkString()}({parameters})");
            if (initializer.Length > 0)
            {
                m_CodeBuilder.AppendLine();
                m_CodeBuilder.AppendLine($"{indent}{{");
                m_CodeBuilder.Append(initializer.ToString());
                m_CodeBuilder.AppendLine($"{indent}}};");
            }
            else
            {
                m_CodeBuilder.AppendLine(";");
            }
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

    private readonly Dictionary<ITypeSymbol, String> m_CustomSerializerVars;
    private readonly ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> m_CustomSerializers;
    private readonly StringBuilder m_SerializerBuilder;
    private readonly StringBuilder m_CodeBuilder;
}