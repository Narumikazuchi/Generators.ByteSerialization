using Narumikazuchi.Generators.ByteSerialization.Strategies;
using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

public record class Customer(Guid Id,
                             String FirstName,
                             String LastName)
    : Entity(Id);

public partial class CustomerStrategy : IByteSerializationStrategy<Customer>
{
    public static Customer Deserialize(ReadOnlySpan<Byte> buffer, out Int32 read)
    {
        Guid id = new(buffer[..16]);
        read = 16;
        String? firstName = StringStrategy.Deserialize(buffer[16..], out Int32 bytesRead);
        read += bytesRead;
        String? lastName = StringStrategy.Deserialize(buffer[read..], out bytesRead);
        read += bytesRead;
        return new Customer(id, firstName!, lastName!);
    }

    public static Int32 GetExpectedByteSize(Customer value)
    {
        Int32 expectedSize = 16;
        expectedSize += StringStrategy.GetExpectedByteSize(value.FirstName);
        expectedSize += StringStrategy.GetExpectedByteSize(value.LastName);
        return expectedSize;
    }

    public static Int32 Serialize(Span<Byte> buffer, Customer value)
    {
        Int32 pointer = 0;
        _ = value.Id.TryWriteBytes(buffer[pointer..(pointer + 16)]);
        pointer += 16;
        pointer += StringStrategy.Serialize(buffer[pointer..], value.FirstName);
        pointer += StringStrategy.Serialize(buffer[pointer..], value.LastName);
        return pointer;
    }
}