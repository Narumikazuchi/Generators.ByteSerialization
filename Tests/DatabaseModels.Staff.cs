namespace Tests;

static public partial class DatabaseModels
{
    public const String STAFF = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Staff(Guid Id,
                                         String FirstName,
                                         String LastName,
                                         String? PhoneNumber,
                                         String? Email,
                                         Boolean Active,
                                         Store Store,
                                         Staff? Manager)
    : Entity(Id);";
}