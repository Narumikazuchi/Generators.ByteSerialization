using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System.Linq;

namespace Tests.Functionality.SpecialType_String;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class OneDimensionalArray
{
    [TestMethod]
    public void ByteArray()
    {
        String[] data = new String[] { "Foo", "Bar" };
        ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(data);
        _ = ByteSerializer.Deserialize(buffer, out String[]? deserialized);

        Assert.IsNotNull(deserialized);
        Assert.IsTrue(data.SequenceEqual(deserialized));
    }

    [TestMethod]
    public void ByteSpan()
    {
        String[] data = new String[] { "Foo", "Bar" };
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = ByteSerializer.Serialize(buffer, data);
        UInt32 read = ByteSerializer.Deserialize(buffer, out String[]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(data.SequenceEqual(deserialized));
    }

    [TestMethod]
    public unsafe void BytePointer()
    {
        String[] data = new String[] { "Foo", "Bar" };
        String[]? deserialized = null;
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = 0;
        UInt32 read = 0;
        fixed (Byte* pointer = buffer)
        {
            written = ByteSerializer.Serialize(pointer, data);
            read = ByteSerializer.Deserialize(pointer, out deserialized);
        }

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(data.SequenceEqual(deserialized));
    }

    [TestMethod]
    public void IOStream()
    {
        String[] data = new String[] { "Foo", "Bar" };
        using MemoryStream stream = new();
        UInt32 written = ByteSerializer.Serialize(stream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(stream, out String[]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(data.SequenceEqual(deserialized));
    }

    [TestMethod]
    public async Task IOStreamAsynchronous()
    {
        String[] data = new String[] { "Foo", "Bar" };
        using MemoryStream stream = new();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<String[]?> result = await ByteSerializer.DeserializeAsynchronously<String[]>(stream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(data.SequenceEqual(result.Result));
    }

    [TestMethod]
    public void InterfaceStream()
    {
        String[] data = new String[] { "Foo", "Bar" };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = ByteSerializer.Serialize(writeStream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(readStream, out String[]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(data.SequenceEqual(deserialized));
    }

    [TestMethod]
    public async Task InterfaceStreamAsynchronous()
    {
        String[] data = new String[] { "Foo", "Bar" };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<String[]?> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, String[]>(readStream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(data.SequenceEqual(result.Result));
    }
}
