using System;

namespace Debugging;

public record struct ManagedValueType(String Name,
                                      Int32 Value)
{
    public Guid Id { get; init; }

    public Decimal Price { get; set; }

    public Boolean ReadOnly { get; }
}