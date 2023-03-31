using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

public readonly struct Vector2D
{
    public Double X { get; init; }
    public Double Y { get; init; }
}

[ByteSerializable]
public sealed unsafe partial record class Unmanaged(Half Half,
                                                    Guid Id,
                                                    Vector2D Vector2D);