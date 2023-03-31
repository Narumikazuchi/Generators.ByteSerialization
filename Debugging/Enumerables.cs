using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Debugging;
/*
[ByteSerializable]
[UseByteSerializationStrategy<Contract, ContractStrategy>]
[UseByteSerializationStrategy<Version, VersionStrategy>]
public sealed partial record class Enumerables(Int64[] Array,
                                               ImmutableArray<String> ImmutableArray,
                                               List<Vector2D> Collection,
                                               Query<Enums> Simple,
                                               AttributeTargets[] Enums,
                                               ImmutableArray<Contract> Contracts,
                                               List<Version> Versions);

public sealed class Query<T> : IEnumerable<T>
{
    public Query(IEnumerable<T> source)
    {
        m_Values = new(source);
    }

    public List<T>.Enumerator GetEnumerator()
    {
        return m_Values.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private readonly List<T> m_Values;
}
*/