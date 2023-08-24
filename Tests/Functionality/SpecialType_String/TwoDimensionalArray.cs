using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;

namespace Tests.Functionality.SpecialType_String;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class TwoDimensionalArray
{
    [TestMethod]
    public void ByteArray()
    {
        String[,] data = new String[2, 1] { { "Foo" }, { "Bar" } };
        ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(data);
        _ = ByteSerializer.Deserialize(buffer, out String[,]? deserialized);

        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public void ByteSpan()
    {
        String[,] data = new String[2, 1] { { "Foo" }, { "Bar" } };
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = ByteSerializer.Serialize(buffer, data);
        UInt32 read = ByteSerializer.Deserialize(buffer, out String[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public unsafe void BytePointer()
    {
        String[,] data = new String[2, 1] { { "Foo" }, { "Bar" } };
        String[,]? deserialized = null;
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
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public void IOStream()
    {
        String[,] data = new String[2, 1] { { "Foo" }, { "Bar" } };
        using MemoryStream stream = new();
        UInt32 written = ByteSerializer.Serialize(stream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(stream, out String[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task IOStreamAsynchronous()
    {
        String[,] data = new String[2, 1] { { "Foo" }, { "Bar" } };
        using MemoryStream stream = new();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<String[,]?> result = await ByteSerializer.DeserializeAsynchronously<String[,]>(stream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    [TestMethod]
    public void InterfaceStream()
    {
        String[,] data = new String[2, 1] { { "Foo" }, { "Bar" } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = ByteSerializer.Serialize(writeStream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(readStream, out String[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task InterfaceStreamAsynchronous()
    {
        String[,] data = new String[2, 1] { { "Foo" }, { "Bar" } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<String[,]?> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, String[,]>(readStream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    static private Boolean SequenceEqual(String[,] left, String[,] right)
    {
        if (left.GetLength(0) != right.GetLength(0) ||
            left.GetLength(1) != right.GetLength(1))
        {
            return false;
        }

        for (Int32 first = 0; first < left.GetLength(0); first++)
        {
            for (Int32 second = 0; second < left.GetLength(1); second++)
            {
                if (left[first, second] != right[first, second])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
