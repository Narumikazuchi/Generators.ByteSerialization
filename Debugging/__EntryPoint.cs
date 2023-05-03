using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Net;

namespace Debugging;

internal class __EntryPoint
{
    static internal void Main(String[] _)
    {
        Uri? uri = new("http://localhost");
        Byte[] buffer = ByteSerializer.Serialize(uri);
        ByteSerializer.Deserialize(buffer, out uri);

        Test(null, null, null, null, null, null, null);

        Console.WriteLine(uri);
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadLine();
    }

    static private void Test(ManagedReferenceType? managed, Cookie[]? cookies_1, String[]? values_1, Cookie[,]? cookies_2, String[,]? values_2, Cookie[][]? cookies_3, String[][]? values_3)
    {
        ByteSerializer.Serialize(managed);
        ByteSerializer.Serialize(cookies_1);
        ByteSerializer.Serialize(cookies_2);
        ByteSerializer.Serialize(cookies_3);
        ByteSerializer.Serialize(values_1);
        ByteSerializer.Serialize(values_2);
        ByteSerializer.Serialize(values_3);
    }
}

public sealed class UriHandler : ISerializationHandler<Uri>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Uri? result)
    {
        UInt32 read = ByteSerializer.Deserialize(buffer, out String? uri);
        result = uri is null ? null : new(uri);
        return read;
    }

    public Int32 GetExpectedArraySize(Uri? graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph?.AbsoluteUri);
    }

    public UInt32 Serialize(Span<Byte> buffer, Uri? graph)
    {
        return ByteSerializer.Serialize(buffer, graph?.AbsoluteUri);
    }
}

public sealed class Test2Handler : ISerializationHandler<Version>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Version? result)
    {
        UInt32 read = ByteSerializer.Deserialize(buffer, out String? version);
        result = version is null ? null : Version.Parse(version);
        return read;
    }

    public Int32 GetExpectedArraySize(Version? graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph?.ToString());
    }

    public UInt32 Serialize(Span<Byte> buffer, Version? graph)
    {
        return ByteSerializer.Serialize(buffer, graph?.ToString());
    }
}