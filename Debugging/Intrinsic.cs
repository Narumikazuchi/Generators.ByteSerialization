using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Intrinsic(DateOnly DateOnly,
                                             DateTime DateTime,
                                             DateTimeOffset DateTimeOffset,
                                             TimeOnly TimeOnly,
                                             TimeSpan TimeSpan,
                                             String String);