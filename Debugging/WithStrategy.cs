using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.Generators.ByteSerialization.Strategies;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Debugging;

#pragma warning disable CS0659
public sealed class Contract
{
    public override Boolean Equals(Object? obj)
    {
        return obj is Contract other &&
               String.Equals(this.Contents, other.Contents, StringComparison.Ordinal);
    }

    public String? Contents { get; init; }
}

[ByteSerializable]
[UseByteSerializationStrategy<Contract, ContractStrategy>]
[UseByteSerializationStrategy<Version, VersionStrategy>]
public sealed partial record class WithStrategy(Contract Contract,
                                                Version Version);

[FixedSerializationSize(4 * sizeof(Int32))]
public readonly struct VersionStrategy : IByteSerializationStrategy<Version>
{
    static public Version Deserialize(ReadOnlySpan<Byte> buffer,
                                      out Int32 read)
    {
        Int32 major = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer));
        Int32 minor = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[4..]));
        Int32 build = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[8..]));
        Int32 revision = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[12..]));
        read = 4 * sizeof(Int32);
        return new(major, minor, build, revision);
    }

    static public Int32 GetExpectedByteSize(Version value)
    {
        return 4 * sizeof(Int32);
    }

    static public Int32 Serialize(Span<Byte> buffer,
                                  Version value)
    {
        Unsafe.As<Byte, Int32>(ref buffer[0]) = value.Major;
        Unsafe.As<Byte, Int32>(ref buffer[4]) = value.Minor;
        Unsafe.As<Byte, Int32>(ref buffer[8]) = value.Build;
        Unsafe.As<Byte, Int32>(ref buffer[12]) = value.Revision;
        return 4 * sizeof(Int32);
    }
}

public readonly struct ContractStrategy : IByteSerializationStrategy<Contract>
{
    static public Contract Deserialize(ReadOnlySpan<Byte> buffer,
                                       out Int32 read)
    {
        return new()
        {
            Contents = StringStrategy.Deserialize(buffer, out read)
        };
    }

    static public Int32 GetExpectedByteSize(Contract value)
    {
        return StringStrategy.GetExpectedByteSize(value.Contents);
    }

    static public Int32 Serialize(Span<Byte> buffer,
                                  Contract value)
    {
        return StringStrategy.Serialize(buffer, value.Contents);
    }
}