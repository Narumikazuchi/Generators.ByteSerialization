namespace Tests;

static public partial class DatabaseModels
{
    public const String STORE = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Store(Guid Id,
                                         String Name,
                                         String? PhoneNumber,
                                         String? Email,
                                         String Street,
                                         String City,
                                         String State
                                         UInt16 ZipCode)
    : Entity(Id);";
}