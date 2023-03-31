namespace Tests;

static public partial class DatabaseModels
{
    public const String PRODUCT = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Product(Guid Id,
                                           String Name,
                                           Brand Brand,
                                           Category Category,
                                           UInt16 ModelYear,
                                           Decimal ListPrice)
    : Entity(Id);";
}