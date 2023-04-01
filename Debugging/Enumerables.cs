using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Debugging;

[ByteSerializable]
[UseByteSerializationStrategy<Contract, ContractStrategy>]
[UseByteSerializationStrategy<Version, VersionStrategy>]
public sealed partial record class Enumerables(Int64[] Array,
                                               ImmutableArray<String> ImmutableArray,
                                               List<Vector2D> Collection,
                                               SortedSet<Enums> Simple,
                                               AttributeTargets[] Enums,
                                               ImmutableArray<Contract> Contracts,
                                               List<Version> Versions,
                                               SortedList<Int32, Double> Dictionary);