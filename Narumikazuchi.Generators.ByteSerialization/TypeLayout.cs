namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents the layout for a type.
/// </summary>
public readonly partial struct TypeLayout
{
    /// <summary>
    /// Creates a new instance of the <see cref="TypeLayout"/> struct for the specified <typeparamref name="T"/>.
    /// </summary>
    static public TypeLayout CreateFrom<T>()
    {
        return CreateFrom(typeof(T));
    }
    /// <summary>
    /// Creates a new instance of the <see cref="TypeLayout"/> struct for the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to create a layout for.</param>
    /// <exception cref="ArgumentNullException"/>
    static public TypeLayout CreateFrom(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        Monitor.Enter(s_Cached);
        if (s_Cached.TryGetValue(key: type,
                                 value: out TypeLayout result))
        {
            Monitor.Exit(s_Cached);
            return result;
        }
        else
        {
            if (type == typeof(TypeLayout))
            {
                result = new(LayoutMemberType.TypeLayout);
            }
            else if (type == typeof(Boolean))
            {
                result = new(LayoutMemberType.Boolean);
            }
            else if (type == typeof(Byte))
            {
                result = new(LayoutMemberType.UnsignedInteger8Bits);
            }
            else if (type == typeof(SByte))
            {
                result = new(LayoutMemberType.Integer8Bits);
            }
            else if (type == typeof(Char))
            {
                result = new(LayoutMemberType.Char);
            }
            else if (type == typeof(Int16))
            {
                result = new(LayoutMemberType.Integer16Bits);
            }
            else if (type == typeof(UInt16))
            {
                result = new(LayoutMemberType.UnsignedInteger16Bits);
            }
            else if (type == typeof(Single))
            {
                result = new(LayoutMemberType.SinglePrecisionFloat);
            }
            else if (type == typeof(Int32))
            {
                result = new(LayoutMemberType.Integer32Bits);
            }
            else if (type == typeof(UInt32))
            {
                result = new(LayoutMemberType.UnsignedInteger32Bits);
            }
            else if (type == typeof(Double))
            {
                result = new(LayoutMemberType.DoublePrecisionFloat);
            }
            else if (type == typeof(Int64))
            {
                result = new(LayoutMemberType.Integer64Bits);
            }
            else if (type == typeof(UInt64))
            {
                result = new(LayoutMemberType.UnsignedInteger64Bits);
            }
            else if (type == typeof(Decimal))
            {
                result = new(LayoutMemberType.Decimal);
            }
            else if (type == typeof(String))
            {
                result = new(LayoutMemberType.String);
            }
            else
            {
                if (AttributeResolver.HasAttribute<DataLayoutAttribute>(type))
                {
                    DataLayoutAttribute dataLayoutAttribute = AttributeResolver.FetchSingleAttribute<DataLayoutAttribute>(type);
                    switch (dataLayoutAttribute.Layout)
                    {
                        case DataLayout.Sequential:
                            result = CreateWithLayoutSequential(type: type,
                                                                isManaged: !type.IsUnmanagedStruct());
                            break;
                        case DataLayout.Explicit:
                            result = CreateWithLayoutExplicit(type: type,
                                                              isManaged: !type.IsUnmanagedStruct());
                            break;
                    }
                }
                else
                {
                    result = CreateWithLayoutSequential(type: type,
                                                        isManaged: !type.IsUnmanagedStruct());
                }
            }

            s_Cached.Add(key: type,
                         value: result);
            Monitor.Exit(s_Cached);
            return result;
        }
    }

    /// <summary>
    /// Returns the string representation of this object.
    /// </summary>
    /// <returns>The string representation of this object.</returns>
    public override String ToString()
    {
        return m_Type.ToString();
    }

    internal TypeLayout(LayoutMemberType memberType)
        : this(memberType: memberType,
               members: Array.Empty<TypeLayout>())
    { }
    internal TypeLayout(LayoutMemberType memberType,
                        TypeLayout[] members)
    {
        m_Type = memberType;
        m_Members = members;
    }

    internal readonly TypeLayout[] m_Members = Array.Empty<TypeLayout>();
    internal readonly LayoutMemberType m_Type;

    static private TypeLayout CreateWithLayoutExplicit(Type type,
                                                       Boolean isManaged = default)
    {
        MemberInfo[] members;
        if (isManaged)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                 .Where(field => !properties.Any(property => field.Name.StartsWith($"<{property.Name}>")));
            members = properties.Where(property => property.GetMethod is not null &&
                                                   property.SetMethod is not null)
                                .Cast<MemberInfo>()
                                .Concat(fields)
                                .OrderBy(ExplicitSort)
                                .ToArray();
        }
        else
        {
            members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                          .OrderBy(ExplicitSort)
                          .ToArray();
        }

        TypeLayout[] typeMembers = new TypeLayout[members.Length];
        for (Int32 counter = 0;
             counter < members.Length;
             counter++)
        {
            if (members[counter] is FieldInfo field)
            {
                typeMembers[counter] = CreateFrom(field.FieldType);
            }
            else if (members[counter] is PropertyInfo property)
            {
                typeMembers[counter] = CreateFrom(property.PropertyType);
            }
        }

        return new(memberType: LayoutMemberType.Object,
                   members: typeMembers);
    }

    static private TypeLayout CreateWithLayoutSequential(Type type,
                                                         Boolean isManaged = default)
    {
        MemberInfo[] members;
        if (isManaged)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                 .Where(field => !properties.Any(property => field.Name.StartsWith($"<{property.Name}>")));
            members = properties.Where(property => property.GetMethod is not null &&
                                                   property.SetMethod is not null)
                                .Cast<MemberInfo>()
                                .Concat(fields)
                                .ToArray();
        }
        else
        {
            members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        TypeLayout[] typeMembers = new TypeLayout[members.Length];
        for (Int32 counter = 0;
             counter < members.Length;
             counter++)
        {
            if (members[counter] is FieldInfo field)
            {
                typeMembers[counter] = CreateFrom(field.FieldType);
            }
            else if (members[counter] is PropertyInfo property)
            {
                typeMembers[counter] = CreateFrom(property.PropertyType);
            }
        }

        return new(memberType: LayoutMemberType.Object,
                   members: typeMembers);
    }

    static private Int32 ExplicitSort(MemberInfo member)
    {
        if (member is FieldInfo field)
        {

            if (AttributeResolver.HasAttribute<DataLayoutPositionAttribute>(field))
            {
                return AttributeResolver.FetchSingleAttribute<DataLayoutPositionAttribute>(field).Position;
            }
            else
            {
                return Int32.MaxValue;
            }
        }
        else if (member is PropertyInfo property)
        {
            if (AttributeResolver.HasAttribute<DataLayoutPositionAttribute>(property))
            {
                return AttributeResolver.FetchSingleAttribute<DataLayoutPositionAttribute>(property).Position;
            }
            else if (property.DeclaringType is not null &&
                     property.DeclaringType.IsRecord())
            {
                ParameterInfo? parameter = property.DeclaringType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                                                                 .FirstOrDefault()?
                                                                 .GetParameters()
                                                                 .FirstOrDefault(parameter => parameter.Name == property.Name);
                if (parameter is null)
                {
                    return Int32.MaxValue;
                }

                if (AttributeResolver.HasAttribute<DataLayoutPositionAttribute>(parameter))
                {
                    return AttributeResolver.FetchSingleAttribute<DataLayoutPositionAttribute>(parameter).Position;
                }
                else
                {
                    return Int32.MaxValue;
                }
            }
            else
            {
                return Int32.MaxValue;
            }
        }
        else
        {
            return Int32.MaxValue;
        }
    }

    static private readonly Dictionary<Type, TypeLayout> s_Cached = new();
}