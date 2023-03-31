namespace Tests;

public partial class DatabaseModels
{
    public const String CUSTOMER = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Customer(Guid Id,
                                            String FirstName,
                                            String LastName,
                                            String? PhoneNumber,
                                            String? Email,
                                            String Street,
                                            String City,
                                            String State
                                            UInt16 ZipCode)
    : Entity(Id);";
}