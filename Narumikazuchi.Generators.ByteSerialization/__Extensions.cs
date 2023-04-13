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

    static private readonly Dictionary<Type, Boolean> s_UnmanagedTypeCache = new();
}