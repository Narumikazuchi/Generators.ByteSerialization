namespace Tests;

static public partial class DatabaseModels
{
    public const String ORDER_ITEM = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class OrderItem(Guid Id,
                                             Order Order,
                                             Product Product,
                                             UInt16 Quantity,
                                             Decimal ListPrice,
                                             Decimal Discount)
    : Entity(Id);";
}