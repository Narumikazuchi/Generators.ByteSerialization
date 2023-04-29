using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization.Generators;

static public class ConstructorCodeWriter
{
    static public void WriteConstructor(ITypeSymbol type,
                                        StringBuilder builder)
    {
        if (type is INamedTypeSymbol named &&
            !named.HasDefaultConstructor() &&
            !named.IsRecord)
        {
            builder.AppendLine();
            builder.AppendLine();

            ImmutableArray<ISymbol> members = named.GetMembersToSerialize();
            WriteMethod(type: named,
                        members: members,
                        builder: builder);

            builder.AppendLine();
            builder.AppendLine($"        static private readonly Lazy<Constructor> s_Constructor = new Lazy<Constructor>(GenerateConstructor, LazyThreadSafetyMode.ExecutionAndPublication);");

            String parameters = String.Join(", ", members.Select(MemberToParameter));

            builder.AppendLine();
            builder.AppendLine($"        private delegate {named.ToFrameworkString()} Constructor({parameters});");
        }
    }

    static private String MemberToParameter(ISymbol member)
    {
        if (member is IFieldSymbol field)
        {
            return $"{field.Type.ToFrameworkString()} {field.Name}";
        }
        else if (member is IPropertySymbol property)
        {
            return $"{property.Type.ToFrameworkString()} {property.Name}";
        }
        else
        {
            return String.Empty;
        }
    }

    static private void WriteMethod(INamedTypeSymbol type,
                                    ImmutableArray<ISymbol> members,
                                    StringBuilder builder)
    {
        builder.AppendLine("        static private Constructor GenerateConstructor()");
        builder.AppendLine("        {");
        builder.Append("            Type[] parameters = new Type[] { ");
        Boolean first = true;
        foreach (ISymbol member in members)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                builder.Append(", ");
            }

            if (member is IFieldSymbol field)
            {
                builder.Append($"typeof({field.Type.ToFrameworkString()})");
            }
            else if (member is IPropertySymbol property)
            {
                builder.Append($"typeof({property.Type.ToFrameworkString()})");
            }
        }

        builder.AppendLine(" };");

        builder.AppendLine($"            DynamicMethod method = new DynamicMethod(\"<Generated>_Constructor\", typeof({type.ToFrameworkString()}), parameters, typeof({type.ToFrameworkString()}));");
        builder.AppendLine("            ILGenerator generator = method.GetILGenerator();");
        builder.AppendLine($"            generator.DeclareLocal(typeof({type.ToFrameworkString()}));");
        if (type.IsValueType)
        {
            builder.AppendLine("            generator.Emit(OpCodes.Ldloc_0);");
            builder.AppendLine($"            generator.Emit(OpCodes.Initobj, typeof({type.ToFrameworkString()}));");
        }
        else
        {
            builder.AppendLine($"            generator.Emit(OpCodes.Ldtoken, typeof({type.ToFrameworkString()}));");
            builder.AppendLine("            generator.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle))!);");
            builder.AppendLine("            generator.Emit(OpCodes.Call, typeof(RuntimeHelpers).GetMethod(nameof(RuntimeHelpers.GetUninitializedObject))!);");
            builder.AppendLine($"            generator.Emit(OpCodes.Castclass, typeof({type.ToFrameworkString()}));");
            builder.AppendLine("            generator.Emit(OpCodes.Stloc_0);");
        }

        Int32 argumentIndex = 0;
        foreach (ISymbol member in members)
        {
            if (member is IFieldSymbol field)
            {
                builder.AppendLine("            generator.Emit(OpCodes.Ldloc_0);");
                builder.AppendLine($"            generator.Emit(OpCodes.Ldarg, {argumentIndex++});");
                builder.AppendLine($"            generator.Emit(OpCodes.Stfld, typeof({field.ContainingType.ToFrameworkString()}).GetField(\"{field.Name}\", BindingFlags.Public | BindingFlags.Instance)!);");
            }
            else if (member is IPropertySymbol property)
            {
                builder.AppendLine("            generator.Emit(OpCodes.Ldloc_0);");
                builder.AppendLine($"            generator.Emit(OpCodes.Ldarg, {argumentIndex++});");
                if (type.GetMembers()
                        .OfType<IFieldSymbol>()
                        .Any(field => field.Name == $"<{property.Name}>k__BackingField"))
                {
                    builder.AppendLine($"            generator.Emit(OpCodes.Stfld, typeof({property.ContainingType.ToFrameworkString()}).GetField(\"<{property.Name}>k__BackingField\", BindingFlags.NonPublic | BindingFlags.Instance)!);");
                }
                else
                {
                    builder.AppendLine($"            generator.Emit(OpCodes.Call, typeof({property.ContainingType.ToFrameworkString()}).GetProperty(\"{property.Name}\", BindingFlags.Public | BindingFlags.Instance)!.SetMethod!);");
                }
            }
        }

        builder.AppendLine("            generator.Emit(OpCodes.Ldloc_0);");
        builder.AppendLine("            generator.Emit(OpCodes.Ret);");
        builder.AppendLine("            return method.CreateDelegate<Constructor>();");
        builder.AppendLine("        }");
    }
}