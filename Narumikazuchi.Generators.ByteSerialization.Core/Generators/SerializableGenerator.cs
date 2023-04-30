﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

[Generator(LanguageNames.CSharp)]
public sealed partial class SerializableGenerator
{
    static private Boolean IsEligableTypeSyntax(SyntaxNode syntaxNode,
                                                CancellationToken cancellationToken = default)
    {
        return syntaxNode is InvocationExpressionSyntax;
    }

    static private ImmutableArray<ITypeSymbol> TransformToType(GeneratorSyntaxContext context,
                                                               CancellationToken cancellationToken)
    {
        return MethodToTypeReferenceFinder.FindTypes(compilation: context.SemanticModel.Compilation,
                                                     invocation: (InvocationExpressionSyntax)context.Node);
    }

    static private void GenerateSerializationCode(SourceProductionContext context,
                                                  (Compilation, ImmutableArray<ITypeSymbol>) compilationAndTypes)
    {
        (Compilation compilation, ImmutableArray<ITypeSymbol> types) = compilationAndTypes;
        if (types.IsDefaultOrEmpty)
        {
            return;
        }

        types = types.Where(ShouldGenerateCode)
                     .Distinct((IEqualityComparer<ITypeSymbol>)SymbolEqualityComparer.Default)
                     .ToImmutableArray();

        CodeAnalysis.Extensions.ClearCaches();
        Extensions.ClearCaches();

        AssemblyHandlerEmitter.Emit(types: types,
                                    context: context,
                                    compilation: compilation);

        Int32 index = 0;
        foreach (ITypeSymbol type in types)
        {
            StringBuilder builder = new();
            builder.AppendLine("//------------------------------------------------------------------------------");
            builder.AppendLine("// <auto-generated>");
            builder.AppendLine("//     This code was generated by a tool.");
            builder.AppendLine("//");
            builder.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            builder.AppendLine("//     the code is regenerated.");
            builder.AppendLine("// </auto-generated>");
            builder.AppendLine("//------------------------------------------------------------------------------");
            builder.AppendLine("#pragma warning disable");
            builder.AppendLine("#nullable disable");
            builder.AppendLine();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Reflection;");
            builder.AppendLine("using System.Reflection.Emit;");
            builder.AppendLine("using System.Runtime.CompilerServices;");
            builder.AppendLine("using System.Runtime.InteropServices;");
            builder.AppendLine("using System.Threading;");
            builder.AppendLine();
            builder.AppendLine("namespace Narumikazuchi.Generated.Internals.ByteSerialization;");
            builder.AppendLine();
            builder.AppendLine($"public unsafe partial interface IAssemblyHandler_{compilation.Assembly.Name.ToValidCSharpTypename()} : {GlobalNames.ISerializationHandler(type)}");
            builder.AppendLine("{");
            builder.AppendLine("    [CompilerGenerated]");
            builder.AppendLine($"    UInt32 {GlobalNames.ISerializationHandler(type)}.Deserialize(ReadOnlySpan<Byte> buffer, out {type.ToFrameworkString()} result)");
            builder.AppendLine("    {");
            builder.AppendLine($"        return __{index}.Deserialize(buffer, out result);");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    [CompilerGenerated]");
            builder.AppendLine($"    Int32 {GlobalNames.ISerializationHandler(type)}.GetExpectedArraySize({type.ToFrameworkString()} value)");
            builder.AppendLine("    {");
            builder.AppendLine($"        return __{index}.GetExpectedArraySize(value);");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    [CompilerGenerated]");
            builder.AppendLine($"    UInt32 {GlobalNames.ISerializationHandler(type)}.Serialize(Span<Byte> buffer, {type.ToFrameworkString()} value)");
            builder.AppendLine("    {");
            builder.AppendLine($"        return __{index}.Serialize(buffer, value);");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    [CompilerGenerated]");
            builder.AppendLine($"    static private class __{index}");
            builder.AppendLine("    {");

            DeserializeCodeWriter.WriteMethod(type: type,
                                              builder: builder);
            builder.AppendLine();

            SizeCodeWriter.WriteMethod(type: type,
                                       builder: builder);
            builder.AppendLine();

            SerializeCodeWriter.WriteMethod(type: type,
                                            builder: builder);

            ConstructorCodeWriter.WriteConstructor(type: type,
                                                   builder: builder);
            index++;

            builder.AppendLine("    }");

            builder.Append('}');

            String source = builder.ToString();
#if DEBUG && OUTPUT
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"..\Generated"));
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, $@"..\Generated\Narumikazuchi.Generated.Internals.ByteSerialization.Handler.{type.ToFileString()}.g.cs"), source);
#endif
            try
            {
                SourceText text = SourceText.From(text: source,
                                                  encoding: Encoding.UTF8);
                context.AddSource(hintName: $"Narumikazuchi.Generated.Internals.ByteSerialization.Handler.{type.ToFileString()}.g.cs",
                                  sourceText: text);
            }
            catch { }
        }
    }

    static private Boolean ShouldGenerateCode(ITypeSymbol type)
    {
        if (type.IsInterface())
        {
            return false;
        }

        if (type is INamedTypeSymbol named)
        {
            if (named.IsOpenGenericType())
            {
                return false;
            }

            if (named.IsAbstract &&
                named.GetDerivedTypes().Length is 0)
            {
                return false;
            }
        }

        if (type.SpecialType is SpecialType.System_IntPtr
                             or SpecialType.System_UIntPtr
                             or SpecialType.System_Delegate
                             or SpecialType.System_MulticastDelegate)
        {
            return false;
        }

        return true;
    }
}