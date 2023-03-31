namespace Tests;

static public partial class DatabaseModels
{
    public const String STOCK = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Stock(Guid Id,
                                         Store Store,
                                         Product Product,
                                         UInt16 Quantity)
    : Entity(Id);";
}