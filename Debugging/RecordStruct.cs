using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

public readonly partial record struct RecordStruct(Vector2D Vector2D,
                                                   UInt64 Distance);