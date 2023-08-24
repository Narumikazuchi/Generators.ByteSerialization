namespace Tests.CodeGenerator.ManagedWithHandler;

static public class HandlerSource
{
    public const String SOURCE = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public readonly struct VersionHandler : ISerializationHandler<Version>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Version? result)
    {
        Int32 pointer = sizeof(Int32);
        TypeIdentifier typeIdentifier = Unsafe.As<Byte, TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
        pointer += Unsafe.SizeOf<TypeIdentifier>();
        if (typeIdentifier != TypeIdentifier.CreateFrom(typeof(Version)))
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
            result = new(major, minor, build, revision);
        }
        return (UInt32)pointer;
    }

    public Int32 GetExpectedArraySize(Version? graph)
    {
        return 6 * sizeof(Int32) + Unsafe.SizeOf<TypeIdentifier>();
    }

    public UInt32 Serialize(Span<Byte> buffer, Version? graph)
    {
        Int32 pointer = sizeof(Int32);
        Unsafe.As<Byte, TypeIdentifier>(ref buffer[pointer]) = TypeIdentifier.CreateFrom(typeof(Version));
        pointer += Unsafe.SizeOf<TypeIdentifier>();
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
        return (UInt32)pointer;
    }
}";
}