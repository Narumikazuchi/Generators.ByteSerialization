using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
[UseByteSerializationStrategy<Customer, CustomerStrategy>]
public partial record class Data(String Name,
                                 Decimal Price,
                                 DateTime LastModified,
                                 Contents Contents,
                                 Customer Customer) :
    Entity(Guid.NewGuid());