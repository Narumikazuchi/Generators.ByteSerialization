namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Represents a map, where you can access both sides of the map with the other object as key.
/// </summary>
public sealed class MirroredMap<TLeft, TRight>
    where TLeft : notnull
    where TRight : notnull
{
    /// <summary>
    /// Adds a mapping pair to the map.
    /// </summary>
    /// <param name="left">The left partner of the mapping.</param>
    /// <param name="right">The right partner of the mapping.</param>
    public void Add(TLeft left,
                    TRight right)
    {
        m_LeftCache.TryAdd(key: left,
                           value: right);
        m_RightCache.TryAdd(key: right,
                            value: left);
    }

    /// <summary>
    /// Gets the left partner for the supplied right key.
    /// </summary>
    /// <param name="key">The key to find the left partner.</param>
    /// <returns>The mapped partner for the key.</returns>
    public TLeft GetLeftPartner(TRight key)
    {
        return m_RightCache[key];
    }

    /// <summary>
    /// Gets the right partner for the supplied left key.
    /// </summary>
    /// <param name="key">The key to find the right partner.</param>
    /// <returns>The mapped partner for the key.</returns>
    public TRight GetRightPartner(TLeft key)
    {
        return m_LeftCache[key];
    }

    private readonly Dictionary<TLeft, TRight> m_LeftCache = new();
    private readonly Dictionary<TRight, TLeft> m_RightCache = new();
}