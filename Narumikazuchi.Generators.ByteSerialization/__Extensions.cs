namespace Narumikazuchi.Generators.ByteSerialization;

static internal class __Extensions
{
    static internal Boolean IsUnmanagedStruct(this Type type)
    {
        if (s_UnmanagedTypeCache.TryGetValue(key: type,
                                             value: out Boolean result))
        {
            return result;
        }

        if (!type.IsValueType)
        {
            s_UnmanagedTypeCache.Add(key: type,
                                     value: false);
            return false;
        }

        if (type.IsPrimitive)
        {
            s_UnmanagedTypeCache.Add(key: type,
                                     value: true);
            return true;
        }

        result = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                     .All(field => !field.FieldType.IsPointer &&
                                   field.FieldType != typeof(IntPtr) &&
                                   field.FieldType != typeof(UIntPtr) &&
                                   (field.FieldType.IsPrimitive ||
                                   field.FieldType.IsEnum ||
                                   field.FieldType.IsUnmanagedStruct()));

        s_UnmanagedTypeCache.Add(key: type,
                                 value: result);
        return result;
    }

    static internal Boolean IsRecord(this Type type)
    {
        if (s_IsRecordeCache.TryGetValue(key: type,
                                         value: out Boolean result))
        {
            return result;
        }
        else
        {
            MethodInfo? equals = type.GetMethod(name: "Equals",
                                                bindingAttr: BindingFlags.Instance | BindingFlags.Public,
                                                types: new Type[] { type });
            if (equals is null)
            {
                return false;
            }

            MethodInfo? deconstruct = type.GetMethod(name: "Deconstruct",
                                                     bindingAttr: BindingFlags.Instance | BindingFlags.Public);
            if (deconstruct is null)
            {
                return false;
            }

            MethodInfo? op_Equality = type.GetMethod(name: "op_Equality",
                                                      bindingAttr: BindingFlags.Static | BindingFlags.Public);
            if (op_Equality is null)
            {
                return false;
            }

            MethodInfo? op_Inequality = type.GetMethod(name: "op_Inequality",
                                                      bindingAttr: BindingFlags.Static | BindingFlags.Public);
            if (op_Inequality is null)
            {
                return false;
            }

            MethodInfo? printMembers = type.GetMethod(name: "PrintMembers",
                                                      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);
            if (printMembers is null)
            {
                return false;
            }

            result = new MethodInfo[]
            {
                equals,
                deconstruct,
                op_Equality,
                op_Inequality,
                printMembers
            }.All(method => method.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(CompilerGeneratedAttribute)));
            s_IsRecordeCache.Add(key: type,
                                 value: result);
            return result;
        }
    }

    static private readonly Dictionary<Type, Boolean> s_UnmanagedTypeCache = new();
    static private readonly Dictionary<Type, Boolean> s_IsRecordeCache = new();
}