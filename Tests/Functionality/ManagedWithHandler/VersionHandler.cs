using Narumikazuchi.Generators.ByteSerialization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tests.Functionality.ManagedWithHandler;

public readonly struct VersionHandler : ISerializationHandler<Version>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Version? result)
    {
        Int32 pointer = sizeof(Int32);
        TypeLayout typeIdentifier = Unsafe.As<Byte, TypeLayout>(ref MemoryMarshal.GetReference(buffer[pointer..]));
        pointer += Unsafe.SizeOf<TypeLayout>();
        if (typeIdentifier != TypeLayout.CreateFrom(typeof(Version)))
        {
            throw new Exception();
        }
        result = default(Version);
        if (buffer[pointer++] == 0x1)
        {
            Int32 major = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += sizeof(Int32);
            Int32 minor = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += sizeof(Int32);
            Int32 build = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += sizeof(Int32);
            Int32 revision = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += sizeof(Int32);
            if (revision > -1)
            {
                result = new(major, minor, build, revision);
            }
            else if (build > -1)
            {
                result = new(major, minor, build);
            }
            else if (minor > -1)
            {
                result = new(major, minor);
            }
        }
        return (UInt32)pointer;
    }

    public Int32 GetExpectedArraySize(Version? graph)
    {
        return 6 * sizeof(Int32) + Unsafe.SizeOf<TypeLayout>();
    }

    public UInt32 Serialize(Span<Byte> buffer, Version? graph)
    {
        Int32 pointer = sizeof(Int32);
        Unsafe.As<Byte, TypeLayout>(ref buffer[pointer]) = TypeLayout.CreateFrom(typeof(Version));
        pointer += Unsafe.SizeOf<TypeLayout>();
        if (graph is null)
        {
            buffer[pointer++] = 0x0;
        }
        else
        {
            buffer[pointer++] = 0x1;
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = graph.Major;
            pointer += sizeof(Int32);
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = graph.Minor;
            pointer += sizeof(Int32);
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = graph.Build;
            pointer += sizeof(Int32);
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = graph.Revision;
            pointer += sizeof(Int32);
        }
        Unsafe.As<Byte, UInt32>(ref buffer[0]) = (UInt32)pointer;
        return (UInt32)pointer;
    }
}