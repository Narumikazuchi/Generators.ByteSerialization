using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

public class ManagedReferenceType
{
    public ManagedReferenceType([SerializeDefault<Int32>(69)] Int32 value,
                                [SerializeFromMember(nameof(Name))] String name)
    {
        this.Value = value;
        this.Name = name;
    }

    public Guid Id { get; init; }

    public Decimal Price { get; set; }

    public Int32 Value { get; init; }

    public String Name { get; set; }

    public Version Version { get; } = new Version(1, 0);
}