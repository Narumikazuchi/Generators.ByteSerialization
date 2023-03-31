namespace Tests;

static public partial class DatabaseModels
{
    public const String BRAND = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Brand(Guid Id,
                                         String Name)
    : Entity(Id);";
}