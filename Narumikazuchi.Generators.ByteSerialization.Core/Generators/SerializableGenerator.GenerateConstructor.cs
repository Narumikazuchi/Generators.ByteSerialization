﻿namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    static private void GenerateConstructorMediator(INamedTypeSymbol symbol,
                                                    ImmutableArray<IFieldSymbol> fields,
                                                    SourceProductionContext context)
    {
        StringBuilder builder = new();
        builder.Append($"public delegate {symbol.ToDisplayString()} ConstructorFor_{symbol.ToDisplayString().Replace(".", "")}(");

        GenerateConstructorMediatorParameters(fields: fields,
                                              builder: builder);

        builder.Append(");");

        String meta = $@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#pragma warning disable
#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Narumikazuchi.Generated;

[EditorBrowsable(EditorBrowsableState.Never)]
[CompilerGenerated]
";
        String source = meta + builder.ToString();

        SourceText text = SourceText.From(text: source,
                                          encoding: Encoding.UTF8);
        context.AddSource(hintName: $"{symbol.Name}.IByteSerializable.Constructor.g.cs",
                          sourceText: text);
    }

    static private void GenerateConstructorMediatorParameters(ImmutableArray<IFieldSymbol> fields,
                                                              StringBuilder builder)
    {
        Boolean first = true;
        foreach (IFieldSymbol field in fields)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                builder.Append(", ");
            }

            ISymbol target = field;
            if (field.AssociatedSymbol is not null)
            {
                target = field.AssociatedSymbol;
            }

            builder.Append($"{field.Type.ToTypename()} {target.Name}");
        }
    }

    static private void GenerateSuitableConstructor(INamedTypeSymbol symbol,
                                                    ImmutableArray<IFieldSymbol> fields,
                                                    SourceProductionContext context)
    {
        StringBuilder builder = new();
        builder.AppendLine("static public partial class ConstructorGenerator");
        builder.AppendLine("{");
        String indent = "    ";
        builder.AppendLine($"{indent}[EditorBrowsable(EditorBrowsableState.Never)]");
        builder.AppendLine($"{indent}[CompilerGenerated]");
        builder.AppendLine($"{indent}static public ConstructorFor_{symbol.ToDisplayString().Replace(".", "")} ConstructorFor_{symbol.ToDisplayString().Replace(".", "")}");
        builder.AppendLine($"{indent}{{");
        indent += "    ";
        builder.AppendLine($"{indent}get");
        builder.AppendLine($"{indent}{{");
        indent += "    ";
        builder.AppendLine($"{indent}return s_ConstructorFor_{symbol.ToDisplayString().Replace(".", "")}.Value;");

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
        builder.AppendLine();

        builder.AppendLine($"{indent}[EditorBrowsable(EditorBrowsableState.Never)]");
        builder.AppendLine($"{indent}[CompilerGenerated]");
        builder.AppendLine($"{indent}static private ConstructorFor_{symbol.ToDisplayString().Replace(".", "")} GenerateConstructorFor_{symbol.ToDisplayString().Replace(".", "")}()");
        builder.AppendLine($"{indent}{{");
        indent += "    ";

        GenerateConstructorBody(symbol: symbol,
                                fields: fields,
                                builder: builder,
                                indent: indent);

        indent = indent.Substring(4);
        builder.AppendLine($"{indent}}}");
        builder.AppendLine();

        builder.AppendLine();
        builder.AppendLine($"{indent}static private Lazy<ConstructorFor_{symbol.ToDisplayString().Replace(".", "")}> s_ConstructorFor_{symbol.ToDisplayString().Replace(".", "")} = new Lazy<ConstructorFor_{symbol.ToDisplayString().Replace(".", "")}>(GenerateConstructorFor_{symbol.ToDisplayString().Replace(".", "")}, LazyThreadSafetyMode.ExecutionAndPublication);");
        builder.Append('}');

        String meta = $@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#pragma warning disable
#nullable enable

using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Narumikazuchi.Generated;

";
        String source = meta + builder.ToString();

        SourceText text = SourceText.From(text: source,
                                          encoding: Encoding.UTF8);
        context.AddSource(hintName: $"{symbol.Name}.IByteSerializable.ConstructorGenerator.g.cs",
                          sourceText: text);
    }

    static private void GenerateConstructorBody(INamedTypeSymbol symbol,
                                                ImmutableArray<IFieldSymbol> fields,
                                                StringBuilder builder,
                                                String indent)
    {
        builder.Append($"{indent}Type[] parameters = new Type[] {{ ");
        Boolean first = true;
        foreach (IFieldSymbol field in fields)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                builder.Append(", ");
            }

            builder.Append($"typeof({field.Type.ToTypename()})");
        }
        builder.AppendLine(" };");

        builder.AppendLine($"{indent}DynamicMethod method = new(\"<Generated>_Constructor\", typeof({symbol.ToDisplayString()}), parameters, typeof({symbol.ToDisplayString()}));");
        builder.AppendLine($"{indent}ILGenerator generator = method.GetILGenerator();");
        builder.AppendLine($"{indent}generator.DeclareLocal(typeof({symbol.ToDisplayString()}));");
        if (symbol.IsValueType)
        {
            builder.AppendLine($"{indent}generator.Emit(OpCodes.Ldloca_S, 0);");
            builder.AppendLine($"{indent}generator.Emit(OpCodes.Initobj, typeof({symbol.ToDisplayString()}));");
            Int32 argumentIndex = 0;
            foreach (IFieldSymbol field in fields)
            {
                builder.AppendLine($"{indent}generator.Emit(OpCodes.Ldloca_S, 0);");
                builder.AppendLine($"{indent}generator.Emit(OpCodes.Ldarg, {argumentIndex++});");
                builder.AppendLine($"{indent}generator.Emit(OpCodes.Stfld, typeof({field.ContainingType.ToDisplayString()}).GetField(\"{field.Name}\", BindingFlags.NonPublic | BindingFlags.Instance)!);");
            }
        }
        else
        {
            builder.AppendLine($"{indent}generator.Emit(OpCodes.Ldtoken, typeof({symbol.ToDisplayString()}));");
            builder.AppendLine($"{indent}generator.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))!);");
            builder.AppendLine($"{indent}generator.Emit(OpCodes.Call, typeof(RuntimeHelpers).GetMethod(nameof(RuntimeHelpers.GetUninitializedObject))!);");
            builder.AppendLine($"{indent}generator.Emit(OpCodes.Castclass, typeof({symbol.ToDisplayString()}));");
            builder.AppendLine($"{indent}generator.Emit(OpCodes.Stloc_0);");
            Int32 argumentIndex = 0;
            foreach (IFieldSymbol field in fields)
            {
                builder.AppendLine($"{indent}generator.Emit(OpCodes.Ldloc_0);");
                builder.AppendLine($"{indent}generator.Emit(OpCodes.Ldarg, {argumentIndex++});");
                builder.AppendLine($"{indent}generator.Emit(OpCodes.Stfld, typeof({field.ContainingType.ToDisplayString()}).GetField(\"{field.Name}\", BindingFlags.NonPublic | BindingFlags.Instance)!);");
            }
        }

        builder.AppendLine($"{indent}generator.Emit(OpCodes.Ldloc_0);");
        builder.AppendLine($"{indent}generator.Emit(OpCodes.Ret);");
        builder.AppendLine($"{indent}return method.CreateDelegate<ConstructorFor_{symbol.ToDisplayString().Replace(".", "")}>();");
    }
}