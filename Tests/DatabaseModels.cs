namespace Tests;

static public partial class DatabaseModels
{
    public const String ENTITY = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public abstract partial record class Entity(Guid Id);";
}