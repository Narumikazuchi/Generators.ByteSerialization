namespace Tests;

static public partial class DatabaseModels
{
    public const String ORDER = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

public enum OrderStatus
{
    Open,
    InProgress,
    Closed
}

[ByteSerializable]
public sealed partial record class Order(Guid Id,
                                         Customer Customer,
                                         OrderStatus Status,
                                         DateTimeOffset OrderDate,
                                         DateTimeOffset RequiredDate,
                                         DateTimeOffset ShippedDate,
                                         Store Store,
                                         Staff Staff)
    : Entity(Id);";
}