using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class SizeCodeWriter
{
    static public void WriteMethod(ITypeSymbol type,
                                   StringBuilder builder)
    {
        builder.AppendLine("        [CompilerGenerated]");
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        static public Int32 GetExpectedArraySize({type.ToFrameworkString()} value)");
        builder.AppendLine("        {");
        builder.AppendLine($"            var size = sizeof(Int64) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

        Int32 varCounter = -1;
        WriteForType(type: type,
                     builder: builder,
                     indent: "            ",
                     target: "value",
                     varCounter: ref varCounter);

        builder.AppendLine("            return size;");
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
            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    size += sizeof(Int32) + {target}.Length * Unsafe.SizeOf<{array.ElementType.ToFrameworkString()}>();");
            builder.AppendLine($"{indent}}}");
        }
        else
        {
            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");
            builder.AppendLine($"{indent}    size += sizeof(Int32) * {array.Rank};");
            varCounter++;
            builder.AppendLine($"{indent}    foreach (var _var{varCounter} in {target})");
            builder.AppendLine($"{indent}    {{");

            WriteForType(type: array.ElementType,
                         builder: builder,
                         indent: indent + "        ",
                         target: $"_var{varCounter}",
                         varCounter: ref varCounter);

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
            builder.AppendLine($"{indent}size += Unsafe.SizeOf<{type.ToFrameworkString()}>();");
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
            builder.AppendLine($"{indent}size += Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

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
        builder.AppendLine($"{indent}size += sizeof(Byte) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");
        if (type.SpecialType is SpecialType.System_String)
        {
            builder.AppendLine($"{indent}size += 4 * {target}.Length;");
        }
        else
        {
            builder.AppendLine($"{indent}if ({target} is not null)");
            builder.AppendLine($"{indent}{{");

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
        builder.AppendLine($"{indent}size += sizeof(Byte) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

        ImmutableArray<INamedTypeSymbol> derivedTypes = type.GetDerivedTypes();
        Boolean first = true;
        foreach (INamedTypeSymbol derivedType in derivedTypes)
        {
            varCounter++;
            if (first)
            {
                builder.AppendLine($"{indent}if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                first = false;
            }
            else
            {
                builder.AppendLine($"{indent}else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
            }

            builder.AppendLine($"{indent}{{");

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

            builder.AppendLine($"{indent}}}");
        }
    }

    static private void WriteForPolymorphicType(INamedTypeSymbol type,
                                                StringBuilder builder,
                                                String indent,
                                                String target,
                                                ref Int32 varCounter)
    {
        ImmutableArray<ISymbol> members;
        String furtherIndent = indent + "    ";

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
            builder.AppendLine($"{indent}size += sizeof(Byte) + Unsafe.SizeOf<{GlobalNames.NAMESPACE}.TypeIdentifier>();");

            Boolean first = true;
            foreach (INamedTypeSymbol derivedType in derivedTypes)
            {
                varCounter++;
                if (first)
                {
                    builder.AppendLine($"{indent}if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                    first = false;
                }
                else
                {
                    builder.AppendLine($"{indent}else if ({target} is {derivedType.ToFrameworkString()} _var{varCounter})");
                }

                builder.AppendLine($"{indent}{{");

                members = derivedType.GetMembersToSerialize();
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

                builder.AppendLine($"{indent}}}");
            }

            varCounter++;
            builder.AppendLine($"{indent}else if ({target} is {type.ToFrameworkString()} _var{varCounter})");
            builder.AppendLine($"{indent}{{");

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

    static private void WriteForCollection(INamedTypeSymbol type,
                                           ITypeSymbol elementType,
                                           StringBuilder builder,
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
                builder.AppendLine($"{indent}size += {target}.Count * Unsafe.SizeOf<{elementType.ToFrameworkString()}>();");
            }
            else
            {
                builder.AppendLine($"{indent}size += ((System.Collections.Generic.ICollection<{elementType.ToFrameworkString()}>){target}).Count * Unsafe.SizeOf<{elementType.ToFrameworkString()}>();");
            }
        }
        else
        {
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
}