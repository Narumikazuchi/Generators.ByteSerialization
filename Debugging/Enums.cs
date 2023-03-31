using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Enums(AttributeTargets AttributeTargets,
                                         ConsoleColor ConsoleColor);