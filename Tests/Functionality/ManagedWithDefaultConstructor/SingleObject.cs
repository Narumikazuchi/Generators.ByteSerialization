using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;

namespace Tests.Functionality.ManagedWithDefaultConstructor;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class SingleObject
{
    [TestMethod]
    public void ByteArray()
    {
        Person data = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(data);
        _ = ByteSerializer.Deserialize(buffer, out Person? deserialized);

        Assert.AreEqual(data, deserialized);
    }

    [TestMethod]
    public void ByteSpan()
    {
        Person data = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = ByteSerializer.Serialize(buffer, data);
        UInt32 read = ByteSerializer.Deserialize(buffer, out Person? deserialized);

        Assert.AreEqual(written, read);
        Assert.AreEqual(data, deserialized);
    }

    [TestMethod]
    public unsafe void BytePointer()
    {
        Person data = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person? deserialized = null;
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = 0;
        UInt32 read = 0;
        fixed (Byte* pointer = buffer)
        {
            written = ByteSerializer.Serialize(pointer, data);
            read = ByteSerializer.Deserialize(pointer, out deserialized);
        }

        Assert.AreEqual(written, read);
        Assert.AreEqual(data, deserialized);
    }

    [TestMethod]
    public void IOStream()
    {
        Person data = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        using MemoryStream stream = new();
        UInt32 written = ByteSerializer.Serialize(stream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(stream, out Person? deserialized);

        Assert.AreEqual(written, read);
        Assert.AreEqual(data, deserialized);
    }

    [TestMethod]
    public async Task IOStreamAsynchronous()
    {
        Person data = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        using MemoryStream stream = new();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Person?> result = await ByteSerializer.DeserializeAsynchronously<Person>(stream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.AreEqual(data, result.Result);
    }

    [TestMethod]
    public void InterfaceStream()
    {
        Person data = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = ByteSerializer.Serialize(writeStream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(readStream, out Person? deserialized);

        Assert.AreEqual(written, read);
        Assert.AreEqual(data, deserialized);
    }

    [TestMethod]
    public async Task InterfaceStreamAsynchronous()
    {
        Person data = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Person?> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, Person>(readStream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.AreEqual(data, result.Result);
    }
}