using System;

namespace Debugging;

public record class Derived : IBase
{
    public Guid Id { get; }
}