using System;

namespace Debugging;

#pragma warning disable CS0659
public sealed partial class NonRecord
{
    public override Boolean Equals(Object? obj)
    {
        return obj is NonRecord other &&
               this.Id.Equals(other.Id) &&
               this.Value.Equals(other.Value);
    }

    public Guid Id { get; init; }

    public Int32 Value { get; set; }

    static public Double Dont { get; set; }

    public Int64 m_Flags = 64L;
}