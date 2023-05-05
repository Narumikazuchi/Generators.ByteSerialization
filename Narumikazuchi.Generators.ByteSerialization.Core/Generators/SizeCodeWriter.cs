using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public sealed class SizeCodeWriter
{
    public SizeCodeWriter(ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> customSerializers)
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
        builder.AppendLine($"    Int32 {GlobalNames.ISerializationHandler(type)}.GetExpectedArraySize({type.ToFrameworkString()} value)");
        builder.AppendLine("    {");
        builder.AppendLine($"        var size = sizeof(Int64) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

        Int32 varCounter = -1;
        this.WriteForType(type: type,
                          indent: "        ",
                          target: "value",
                          varCounter: ref varCounter);

        builder.Append(m_SerializerBuilder.ToString());
        builder.Append(m_CodeBuilder.ToString());

        builder.AppendLine("        return size;");
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

            m_CodeBuilder.AppendLine($"{indent}size += {serializer}.GetExpectedArraySize({target});");
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
            array.ElementType.IsUnmanagedSerializable())
        {
            m_CodeBuilder.AppendLine($"{indent}if ({target} is not null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    size += sizeof(Int32) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>() + {target}.Length * Unsafe.SizeOf<{array.ElementType.ToFrameworkString()}>();");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}if ({target} is not null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    size += sizeof(Int32) * {array.Rank} + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}    foreach (var _var{varCounter} in {target})");
            m_CodeBuilder.AppendLine($"{indent}    {{");

            this.WriteForType(type: array.ElementType,
                              indent: indent + "        ",
                              target: $"_var{varCounter}",
                              varCounter: ref varCounter);

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
            m_CodeBuilder.AppendLine($"{indent}size += Unsafe.SizeOf<{type.ToFrameworkString()}>();");
        }
        else if (type.ToFrameworkString().StartsWith("System.Collections.Generic.KeyValuePair<"))
        {
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
            m_CodeBuilder.AppendLine($"{indent}size += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

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
        m_CodeBuilder.AppendLine($"{indent}size += sizeof(Byte) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
        if (type.SpecialType is SpecialType.System_String)
        {
            m_CodeBuilder.AppendLine($"{indent}if ({target} is not null)");
            m_CodeBuilder.AppendLine($"{indent}{{");
            m_CodeBuilder.AppendLine($"{indent}    size += 4 * {target}.Length;");
            m_CodeBuilder.AppendLine($"{indent}}}");
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}if ({target} is not null)");
            m_CodeBuilder.AppendLine($"{indent}{{");

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
        m_CodeBuilder.AppendLine($"{indent}size += sizeof(Byte) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        Boolean first = true;
        foreach (INamedTypeSymbol derivedType in derivedTypes)
        {
            varCounter++;
            if (first)
            {
                m_CodeBuilder.AppendLine($"{indent}if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                first = false;
            }
            else
            {
                m_CodeBuilder.AppendLine($"{indent}else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
            }

            m_CodeBuilder.AppendLine($"{indent}{{");

            ImmutableArray<ISymbol> members = derivedType.GetMembersToSerialize();
            String furtherIndent = indent + "        ";
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

            if (derivedType.IsCollection(out ITypeSymbol elementType))
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

    private void WriteForPolymorphicType(INamedTypeSymbol type,
                                         String indent,
                                         String target,
                                         ref Int32 varCounter)
    {
        ImmutableArray<ISymbol> members;
        String furtherIndent = indent + "    ";

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        if (derivedTypes.Length is 0)
        {
            this.WriteForSealedType(type: type,
                                    indent: indent,
                                    varCounter: ref varCounter,
                                    target: target);
        }
        else
        {
            m_CodeBuilder.AppendLine($"{indent}size += sizeof(Byte) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            Boolean first = true;
            foreach (INamedTypeSymbol derivedType in derivedTypes)
            {
                varCounter++;
                if (first)
                {
                    m_CodeBuilder.AppendLine($"{indent}if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                    first = false;
                }
                else
                {
                    m_CodeBuilder.AppendLine($"{indent}else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                }

                m_CodeBuilder.AppendLine($"{indent}{{");

                members = derivedType.GetMembersToSerialize();
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

                m_CodeBuilder.AppendLine($"{indent}}}");
            }

            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}else if ({target} is {type.ToFrameworkString()} _var{varCounter})");
            m_CodeBuilder.AppendLine($"{indent}{{");

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

    private void WriteForCollection(INamedTypeSymbol type,
                                    ITypeSymbol elementType,
                                    String indent,
                                    String target,
                                    ref Int32 varCounter)
    {
        if (elementType.IsUnmanagedSerializable())
        {
            if (type.GetMembers("Count")
                    .OfType<IPropertySymbol>()
                    .Any(property => property.DeclaredAccessibility is Accessibility.Public))
            {
                m_CodeBuilder.AppendLine($"{indent}size += {target}.Count * Unsafe.SizeOf<{elementType.ToFrameworkString()}>();");
            }
            else
            {
                m_CodeBuilder.AppendLine($"{indent}size += ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Count * Unsafe.SizeOf<{elementType.ToFrameworkString()}>();");
            }
        }
        else
        {
            varCounter++;
            m_CodeBuilder.AppendLine($"{indent}foreach (var _var{varCounter} in {target})");
            m_CodeBuilder.AppendLine($"{indent}{{");

            this.WriteForType(type: elementType,
                              indent: indent + "    ",
                              varCounter: ref varCounter,
                              target: $"_var{varCounter}");

            m_CodeBuilder.AppendLine($"{indent}}}");
        }
    }

    private readonly Dictionary<ITypeSymbol, String> m_CustomSerializerVars;
    private readonly ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> m_CustomSerializers;
    private readonly StringBuilder m_SerializerBuilder;
    private readonly StringBuilder m_CodeBuilder;
}