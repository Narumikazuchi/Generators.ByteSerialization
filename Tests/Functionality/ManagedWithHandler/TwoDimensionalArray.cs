using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;

namespace Tests.Functionality.ManagedWithHandler;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class TwoDimensionalArray
{
    [TestMethod]
    public void ByteArray()
    {
        Version[,] data = new Version[3, 1] { { new(2, 5, 0) }, { new(3, 0) }, { new(1, 1, 6, 125) } };
        ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(data);
        _ = ByteSerializer.Deserialize(buffer, out Version[,]? deserialized);

        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public void ByteSpan()
    {
        Version[,] data = new Version[3, 1] { { new(2, 5, 0) }, { new(3, 0) }, { new(1, 1, 6, 125) } };
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = ByteSerializer.Serialize(buffer, data);
        UInt32 read = ByteSerializer.Deserialize(buffer, out Version[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public unsafe void BytePointer()
    {
        Version[,] data = new Version[3, 1] { { new(2, 5, 0) }, { new(3, 0) }, { new(1, 1, 6, 125) } };
        Version[,]? deserialized = null;
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
        Version[,] data = new Version[3, 1] { { new(2, 5, 0) }, { new(3, 0) }, { new(1, 1, 6, 125) } };
        using MemoryStream stream = new();
        UInt32 written = ByteSerializer.Serialize(stream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(stream, out Version[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task IOStreamAsynchronous()
    {
        Version[,] data = new Version[3, 1] { { new(2, 5, 0) }, { new(3, 0) }, { new(1, 1, 6, 125) } };
        using MemoryStream stream = new();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Version[,]?> result = await ByteSerializer.DeserializeAsynchronously<Version[,]>(stream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    [TestMethod]
    public void InterfaceStream()
    {
        Version[,] data = new Version[3, 1] { { new(2, 5, 0) }, { new(3, 0) }, { new(1, 1, 6, 125) } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = ByteSerializer.Serialize(writeStream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(readStream, out Version[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task InterfaceStreamAsynchronous()
    {
        Version[,] data = new Version[3, 1] { { new(2, 5, 0) }, { new(3, 0) }, { new(1, 1, 6, 125) } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Version[,]?> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, Version[,]>(readStream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    static private Boolean SequenceEqual(Version[,] left, Version[,] right)
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
