using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;

namespace Tests.Functionality.ManagedWithDefaultConstructor;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class TwoDimensionalArray
{
    [TestMethod]
    public void ByteArray()
    {
        Person data1 = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person data2 = new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new(1992, 6, 9),
            CustomerId = Guid.NewGuid()
        };
        Person[,] data = new Person[2, 1] { { data1 }, { data2 } };
        ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(data);
        _ = ByteSerializer.Deserialize(buffer, out Person[,]? deserialized);

        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public void ByteSpan()
    {
        Person data1 = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person data2 = new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new(1992, 6, 9),
            CustomerId = Guid.NewGuid()
        };
        Person[,] data = new Person[2, 1] { { data1 }, { data2 } };
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
        UInt32 written = ByteSerializer.Serialize(buffer, data);
        UInt32 read = ByteSerializer.Deserialize(buffer, out Person[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public unsafe void BytePointer()
    {
        Person data1 = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person data2 = new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new(1992, 6, 9),
            CustomerId = Guid.NewGuid()
        };
        Person[,] data = new Person[2, 1] { { data1 }, { data2 } };
        Person[,]? deserialized = null;
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
        Person data1 = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person data2 = new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new(1992, 6, 9),
            CustomerId = Guid.NewGuid()
        };
        Person[,] data = new Person[2, 1] { { data1 }, { data2 } };
        using MemoryStream stream = new();
        UInt32 written = ByteSerializer.Serialize(stream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(stream, out Person[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task IOStreamAsynchronous()
    {
        Person data1 = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person data2 = new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new(1992, 6, 9),
            CustomerId = Guid.NewGuid()
        };
        Person[,] data = new Person[2, 1] { { data1 }, { data2 } };
        using MemoryStream stream = new();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Person[,]?> result = await ByteSerializer.DeserializeAsynchronously<Person[,]>(stream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    [TestMethod]
    public void InterfaceStream()
    {
        Person data1 = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person data2 = new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new(1992, 6, 9),
            CustomerId = Guid.NewGuid()
        };
        Person[,] data = new Person[2, 1] { { data1 }, { data2 } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = ByteSerializer.Serialize(writeStream, data);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(readStream, out Person[,]? deserialized);

        Assert.AreEqual(written, read);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(SequenceEqual(data, deserialized));
    }

    [TestMethod]
    public async Task InterfaceStreamAsynchronous()
    {
        Person data1 = new()
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new(1990, 4, 20),
            CustomerId = Guid.NewGuid()
        };
        Person data2 = new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new(1992, 6, 9),
            CustomerId = Guid.NewGuid()
        };
        Person[,] data = new Person[2, 1] { { data1 }, { data2 } };
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, data);
        stream.Position = 0;
        AsynchronousDeserializationResult<Person[,]?> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, Person[,]>(readStream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.IsNotNull(result.Result);
        Assert.IsTrue(SequenceEqual(data, result.Result));
    }

    static private Boolean SequenceEqual(Person[,] left, Person[,] right)
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
