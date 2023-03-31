namespace Tests;

static public partial class DatabaseModels
{
    public const String CATEGORIES = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Category(Guid Id,
                                            String Name)
    : Entity(Id);";
}