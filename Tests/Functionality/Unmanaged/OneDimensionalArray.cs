using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System.IO;
using System.Linq;

namespace Tests.Functionality.Unmanaged;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class OneDimensionalArray
{
    [TestMethod]
    [DataRow(typeof(Boolean), DisplayName = "ByteArray (Boolean)")]
    [DataRow(typeof(SByte), DisplayName = "ByteArray (SByte)")]
    [DataRow(typeof(Char), DisplayName = "ByteArray (Char)")]
    [DataRow(typeof(Double), DisplayName = "ByteArray (Double)")]
    [DataRow(typeof(UInt64), DisplayName = "ByteArray (UInt64)")]
    [DataRow(typeof(DateOnly), DisplayName = "ByteArray (DateOnly)")]
    [DataRow(typeof(TimeSpan), DisplayName = "ByteArray (TimeSpan)")]
    [DataRow(typeof(Guid), DisplayName = "ByteArray (Guid)")]
    [DataRow(typeof(ConsoleColor), DisplayName = "ByteArray (ConsoleColor)")]
    public void ByteArray(Type type)
    {
        if (type == typeof(Boolean))
        {
            Boolean[] value = GenerateRandomArray<Boolean>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out Boolean[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(SByte))
        {
            SByte[] value = GenerateRandomArray<SByte>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out SByte[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(Char))
        {
            Char[] value = GenerateRandomArray<Char>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out Char[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(UInt64))
        {
            UInt64[] value = GenerateRandomArray<UInt64>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out UInt64[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(Double))
        {
            Double[] value = GenerateRandomArray<Double>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out Double[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(DateOnly))
        {
            DateOnly[] value = GenerateRandomArray<DateOnly>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out DateOnly[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(TimeSpan))
        {
            TimeSpan[] value = GenerateRandomArray<TimeSpan>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out TimeSpan[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(Guid))
        {
            Guid[] value = GenerateRandomArray<Guid>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out Guid[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
        else if (type == typeof(ConsoleColor))
        {
            ConsoleColor[] value = GenerateRandomArray<ConsoleColor>(16);
            ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
            _ = ByteSerializer.Deserialize(buffer, out ConsoleColor[]? deserialized);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.SequenceEqual(deserialized));
        }
    }

    [TestMethod]
    [DataRow(typeof(Boolean), DisplayName = "ByteArray (Boolean)")]
    [DataRow(typeof(SByte), DisplayName = "ByteArray (SByte)")]
    [DataRow(typeof(Char), DisplayName = "ByteArray (Char)")]
    [DataRow(typeof(Double), DisplayName = "ByteArray (Double)")]
    [DataRow(typeof(UInt64), DisplayName = "ByteArray (UInt64)")]
    [DataRow(typeof(DateOnly), DisplayName = "ByteArray (DateOnly)")]
    [DataRow(typeof(TimeSpan), DisplayName = "ByteArray (TimeSpan)")]
    [DataRow(typeof(Guid), DisplayName = "ByteArray (Guid)")]
    [DataRow(typeof(ConsoleColor), DisplayName = "ByteArray (ConsoleColor)")]
    public void ByteSpan(Type type)
    {
        if (type == typeof(Boolean))
        {
            Boolean[] data = GenerateRandomArray<Boolean>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out Boolean[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(SByte))
        {
            SByte[] data = GenerateRandomArray<SByte>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out SByte[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(Char))
        {
            Char[] data = GenerateRandomArray<Char>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out Char[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(UInt64))
        {
            UInt64[] data = GenerateRandomArray<UInt64>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out UInt64[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(Double))
        {
            Double[] data = GenerateRandomArray<Double>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out Double[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(DateOnly))
        {
            DateOnly[] data = GenerateRandomArray<DateOnly>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out DateOnly[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(TimeSpan))
        {
            TimeSpan[] data = GenerateRandomArray<TimeSpan>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out TimeSpan[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(Guid))
        {
            Guid[] data = GenerateRandomArray<Guid>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out Guid[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(ConsoleColor))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(data)];
            UInt32 written = ByteSerializer.Serialize(buffer, data);
            UInt32 read = ByteSerializer.Deserialize(buffer, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
    }

    [TestMethod]
    [DataRow(typeof(Boolean), DisplayName = "ByteArray (Boolean)")]
    [DataRow(typeof(SByte), DisplayName = "ByteArray (SByte)")]
    [DataRow(typeof(Char), DisplayName = "ByteArray (Char)")]
    [DataRow(typeof(Double), DisplayName = "ByteArray (Double)")]
    [DataRow(typeof(UInt64), DisplayName = "ByteArray (UInt64)")]
    [DataRow(typeof(DateOnly), DisplayName = "ByteArray (DateOnly)")]
    [DataRow(typeof(TimeSpan), DisplayName = "ByteArray (TimeSpan)")]
    [DataRow(typeof(Guid), DisplayName = "ByteArray (Guid)")]
    [DataRow(typeof(ConsoleColor), DisplayName = "ByteArray (ConsoleColor)")]
    public unsafe void BytePointer(Type type)
    {
        if (type == typeof(Boolean))
        {
            Boolean[] data = GenerateRandomArray<Boolean>(16);
            Boolean[]? deserialized = null;
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
        else if (type == typeof(SByte))
        {
            SByte[] data = GenerateRandomArray<SByte>(16);
            SByte[]? deserialized = null;
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
        else if (type == typeof(Char))
        {
            Char[] data = GenerateRandomArray<Char>(16);
            Char[]? deserialized = null;
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
        else if (type == typeof(UInt64))
        {
            UInt64[] data = GenerateRandomArray<UInt64>(16);
            UInt64[]? deserialized = null;
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
        else if (type == typeof(Double))
        {
            Double[] data = GenerateRandomArray<Double>(16);
            Double[]? deserialized = null;
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
        else if (type == typeof(DateOnly))
        {
            DateOnly[] data = GenerateRandomArray<DateOnly>(16);
            DateOnly[]? deserialized = null;
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
        else if (type == typeof(TimeSpan))
        {
            TimeSpan[] data = GenerateRandomArray<TimeSpan>(16);
            TimeSpan[]? deserialized = null;
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
        else if (type == typeof(Guid))
        {
            Guid[] data = GenerateRandomArray<Guid>(16);
            Guid[]? deserialized = null;
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
        else if (type == typeof(ConsoleColor))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            ConsoleColor[]? deserialized = null;
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
    }

    [TestMethod]
    [DataRow(typeof(Boolean), DisplayName = "ByteArray (Boolean)")]
    [DataRow(typeof(SByte), DisplayName = "ByteArray (SByte)")]
    [DataRow(typeof(Char), DisplayName = "ByteArray (Char)")]
    [DataRow(typeof(Double), DisplayName = "ByteArray (Double)")]
    [DataRow(typeof(UInt64), DisplayName = "ByteArray (UInt64)")]
    [DataRow(typeof(DateOnly), DisplayName = "ByteArray (DateOnly)")]
    [DataRow(typeof(TimeSpan), DisplayName = "ByteArray (TimeSpan)")]
    [DataRow(typeof(Guid), DisplayName = "ByteArray (Guid)")]
    [DataRow(typeof(ConsoleColor), DisplayName = "ByteArray (ConsoleColor)")]
    public void IOStream(Type type)
    {
        if (type == typeof(Boolean))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(SByte))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(Char))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(UInt64))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(Double))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(DateOnly))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(TimeSpan))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(Guid))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
        else if (type == typeof(ConsoleColor))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = ByteSerializer.Serialize(stream, data);
            stream.Position = 0;
            UInt32 read = ByteSerializer.Deserialize(stream, out ConsoleColor[]? deserialized);

            Assert.AreEqual(written, read);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(data.SequenceEqual(deserialized));
        }
    }

    [TestMethod]
    [DataRow(typeof(Boolean), DisplayName = "ByteArray (Boolean)")]
    [DataRow(typeof(SByte), DisplayName = "ByteArray (SByte)")]
    [DataRow(typeof(Char), DisplayName = "ByteArray (Char)")]
    [DataRow(typeof(Double), DisplayName = "ByteArray (Double)")]
    [DataRow(typeof(UInt64), DisplayName = "ByteArray (UInt64)")]
    [DataRow(typeof(DateOnly), DisplayName = "ByteArray (DateOnly)")]
    [DataRow(typeof(TimeSpan), DisplayName = "ByteArray (TimeSpan)")]
    [DataRow(typeof(Guid), DisplayName = "ByteArray (Guid)")]
    [DataRow(typeof(ConsoleColor), DisplayName = "ByteArray (ConsoleColor)")]
    public async Task IOStreamAsynchronous(Type type)
    {
        if (type == typeof(Boolean))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(SByte))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(Char))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(UInt64))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(Double))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(DateOnly))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(TimeSpan))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(Guid))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
        else if (type == typeof(ConsoleColor))
        {
            ConsoleColor[] data = GenerateRandomArray<ConsoleColor>(16);
            using MemoryStream stream = new();
            UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, data);
            stream.Position = 0;
            AsynchronousDeserializationResult<ConsoleColor[]?> result = await ByteSerializer.DeserializeAsynchronously<ConsoleColor[]>(stream);

            Assert.AreEqual(written, result.BytesRead);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(data.SequenceEqual(result.Result));
        }
    }

    //[TestMethod]
    //public void InterfaceStream()
    //{
    //    T[] data = GenerateRandomArray<T>(16);
    //    using MemoryStream stream = new();
    //    WriteableStreamWrapper writeStream = stream.AsWriteableStream();
    //    ReadableStreamWrapper readStream = stream.AsReadableStream();
    //    UInt32 written = ByteSerializer.Serialize(writeStream, data);
    //    stream.Position = 0;
    //    UInt32 read = ByteSerializer.Deserialize(readStream, out T[]? deserialized);

    //    Assert.AreEqual(written, read);
    //    Assert.IsNotNull(deserialized);
    //    Assert.IsTrue(data.SequenceEqual(deserialized));
    //}

    //[TestMethod]
    //public async Task InterfaceStreamAsynchronous()
    //{
    //    T[] data = GenerateRandomArray<T>(16);
    //    using MemoryStream stream = new();
    //    WriteableStreamWrapper writeStream = stream.AsWriteableStream();
    //    ReadableStreamWrapper readStream = stream.AsReadableStream();
    //    UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, data);
    //    stream.Position = 0;
    //    AsynchronousDeserializationResult<T[]?> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, T[]>(readStream);

    //    Assert.AreEqual(written, result.BytesRead);
    //    Assert.IsNotNull(result.Result);
    //    Assert.IsTrue(data.SequenceEqual(result.Result));
    //}

    static private T[] GenerateRandomArray<T>(Int32 length)
        where T : unmanaged
    {
        T[] array = new T[length];

        for (Int32 index = 0;
             index < length;
             index++)
        {
            // Generate a random value of type T using the Random class
            T value = default;
            if (typeof(T) == typeof(Boolean))
            {
                value = (T)Convert.ChangeType(Random.Shared.Next(0, 2) == 1, typeof(T));
            }
            else if (typeof(T) == typeof(SByte))
            {
                value = (T)Convert.ChangeType(Random.Shared.Next(SByte.MinValue, SByte.MaxValue + 1), typeof(T));
            }
            else if (typeof(T) == typeof(Char))
            {
                value = (T)Convert.ChangeType(Random.Shared.Next(Char.MinValue, Char.MaxValue + 1), typeof(T));
            }
            else if (typeof(T) == typeof(UInt64))
            {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                value = (T)Convert.ChangeType(((UInt64)Random.Shared.Next() << 32) | (UInt64)Random.Shared.Next(), typeof(T));
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
            }
            else if (typeof(T) == typeof(Double))
            {
                value = (T)Convert.ChangeType(Random.Shared.NextDouble(), typeof(T));
            }
            else if (typeof(T) == typeof(DateOnly))
            {
                value = (T)Convert.ChangeType(new DateOnly(Random.Shared.Next(1, 9999), Random.Shared.Next(1, 13), Random.Shared.Next(1, 29)), typeof(T));
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                value = (T)Convert.ChangeType(new TimeSpan(Random.Shared.Next(0, 24), Random.Shared.Next(0, 60), Random.Shared.Next(0, 60)), typeof(T));
            }
            else if (typeof(T) == typeof(Guid))
            {
                value = (T)Convert.ChangeType(Guid.NewGuid(), typeof(T));
            }
            else if (typeof(T) == typeof(ConsoleColor))
            {
                value = (T)Convert.ChangeType((ConsoleColor)Random.Shared.Next(0, 16), typeof(T));
            }

            array[index] = value;
        }

        return array;
    }
}