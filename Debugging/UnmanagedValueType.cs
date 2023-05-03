using System;

namespace Debugging;

public record struct UnmanagedValueType(Guid Id)
{
    public DateTimeOffset ModifiedAt { get; set; }
}