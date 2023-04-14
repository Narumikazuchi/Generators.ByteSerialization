namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Caches unique ids for every serializable type.
/// </summary>
static public class TypeCache
{
    static TypeCache()
    {
        s_TypeIds = new();
        _ = GetIdOfType<Boolean>();
        _ = GetIdOfType<Byte>();
        _ = GetIdOfType<Char>();
        _ = GetIdOfType<DateOnly>();
        _ = GetIdOfType<DateTime>();
        _ = GetIdOfType<DateTimeOffset>();
        _ = GetIdOfType<Decimal>();
        _ = GetIdOfType<Double>();
        _ = GetIdOfType<Int16>();
        _ = GetIdOfType<Int32>();
        _ = GetIdOfType<Int64>();
        _ = GetIdOfType<SByte>();
        _ = GetIdOfType<Single>();
        _ = GetIdOfType<String>();
        _ = GetIdOfType<UInt16>();
        _ = GetIdOfType<UInt32>();
        _ = GetIdOfType<UInt64>();
    }

    /// <summary>
    /// Gets the semi-unique id for the type <typeparamref name="TType"/>.
    /// </summary>
    /// <returns>The semi-unique id for the type <typeparamref name="TType"/>.</returns>
    static public Guid GetIdOfType<TType>()
    {
        return GetIdOfType(typeof(TType));
    }
    /// <summary>
    /// Gets the semi-unique id for <paramref name="type"/>.
    /// </summary>
    /// <returns>The semi-unique id for the passed type.</returns>
    static public Guid GetIdOfType(Type type)
    {
        if (type.AssemblyQualifiedName is null)
        {
            throw new ArgumentException(message: "Generic type parameters are not supported when generating a unique id.",
                                        paramName: nameof(type));
        }

        if (s_TypeIds.TryGetValue(key: type.AssemblyQualifiedName,
                                  value: out Guid result))
        {
            return result;
        }
        else
        {
            ReadOnlySpan<Byte> source = MemoryMarshal.AsBytes(type.AssemblyQualifiedName.AsSpan());
            Span<Byte> buffer = SHA1.HashData(source);
            buffer[6] &= 0x0F;
            buffer[6] |= 0x50;
            buffer[8] &= 0x3F;
            buffer[8] |= 0x80;

            result = Unsafe.ReadUnaligned<Guid>(ref buffer[0]);
            s_TypeIds.Add(key: type.AssemblyQualifiedName,
                          value: result);
            return result;
        }
    }

    /// <summary>
    /// Gets all type candidates for the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The semi-unique id to lookup.</param>
    /// <returns>All type candidates for the specified <paramref name="id"/>.</returns>
    static public ImmutableArray<Type> GetTypeCandidatesForId(Guid id)
    {
        return s_TypeIds.Where(kv => kv.Value == id)
                        .Select(kv => Type.GetType(kv.Key)!)
                        .Where(type => type is not null)
                        .ToImmutableArray();
    }

    static private readonly Dictionary<String, Guid> s_TypeIds;
}