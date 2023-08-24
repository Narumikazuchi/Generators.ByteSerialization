using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;

namespace Tests.Functionality.Unmanaged;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class TwoDimensionalArray
{
    [TestMethod]
    public void ByteArray()
    {
        Guid[,] data = new Guid[2, 1] { { Guid.NewGuid() }, { Guid.NewGuid() } };
        ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(data);
        _ = ByteSerializer.Deserialize(buffer, out Guid[,]? deserialized);

        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public void ByteSpan()
    {
        Guid[,] data = new Guid[2, 1] { { Guid.NewGuid() }, { Guid.NewGuid() } };
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = ByteSerializer.Serialize(buffer, data);
        UInt32 read = ByteSerializer.Deserialize(buffer, out Guid[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public unsafe void BytePointer()
    {
        Guid[,] data = new Guid[2, 1] { { Guid.NewGuid() }, { Guid.NewGuid() } };
        Guid[,]? deserialized = null;
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
        Guid[,] data = new Guid[2, 1] { { Guid.NewGuid() }, { Guid.NewGuid() } };
        using MemoryStream stream = new();
        UInt32 written = ByteSerializer.Serialize(stream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(stream, out Guid[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task IOStreamAsynchronous()
    {
        Guid[,] data = new Guid[2, 1] { { Guid.NewGuid() }, { Guid.NewGuid() } };
        using MemoryStream stream = new();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Guid[,]?> result = await ByteSerializer.DeserializeAsynchronously<Guid[,]>(stream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    [TestMethod]
    public void InterfaceStream()
    {
        Guid[,] data = new Guid[2, 1] { { Guid.NewGuid() }, { Guid.NewGuid() } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = ByteSerializer.Serialize(writeStream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(readStream, out Guid[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task InterfaceStreamAsynchronous()
    {
        Guid[,] data = new Guid[2, 1] { { Guid.NewGuid() }, { Guid.NewGuid() } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Guid[,]?> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, Guid[,]>(readStream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    static private Boolean SequenceEqual(Guid[,] left, Guid[,] right)
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
