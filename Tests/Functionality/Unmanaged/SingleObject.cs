using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;

namespace Tests.Functionality.Unmanaged;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class SingleObject
{
    [TestMethod]
    [DataRow("true", DisplayName = "ByteArray (Boolean)")]
    [DataRow("-64", DisplayName = "ByteArray (SByte)")]
    [DataRow("F", DisplayName = "ByteArray (Char)")]
    [DataRow("256,128", DisplayName = "ByteArray (Double)")]
    [DataRow("420", DisplayName = "ByteArray (UInt64)")]
    [DataRow("06/08/2022", DisplayName = "ByteArray (DateOnly)")]
    [DataRow("1.00:30", DisplayName = "ByteArray (TimeSpan)")]
    [DataRow("e3f947f2-cf32-441c-b31a-74e780cfc6f6", DisplayName = "ByteArray (Guid)")]
    [DataRow("DarkGreen", DisplayName = "ByteArray (ConsoleColor)")]
    public void ByteArray(String value)
    {
        if (Boolean.TryParse(value, out Boolean booleanResult))
        {
            ByteArray(booleanResult);
        }
        else if (SByte.TryParse(value, out SByte sbyteResult))
        {
            ByteArray(sbyteResult);
        }
        else if (Char.TryParse(value, out Char charResult))
        {
            ByteArray(charResult);
        }
        else if (UInt64.TryParse(value, out UInt64 uint64Result))
        {
            ByteArray(uint64Result);
        }
        else if (Double.TryParse(value, out Double doubleResult))
        {
            ByteArray(doubleResult);
        }
        else if (DateOnly.TryParse(value, out DateOnly dateOnlyResult))
        {
            ByteArray(dateOnlyResult);
        }
        else if (TimeSpan.TryParse(value, out TimeSpan timeSpanResult))
        {
            ByteArray(timeSpanResult);
        }
        else if (Guid.TryParse(value, out Guid guidResult))
        {
            ByteArray(guidResult);
        }
        else if (Enum.TryParse(value, out ConsoleColor consoleColorResult))
        {
            ByteArray(consoleColorResult);
        }
    }

    static private void ByteArray<T>(T value)
        where T : unmanaged
    {
        ReadOnlySpan<Byte> buffer = ByteSerializer.Serialize(value);
        _ = ByteSerializer.Deserialize(buffer, out T deserialized);

        Assert.AreEqual(value, deserialized);
    }

    [TestMethod]
    [DataRow("true", DisplayName = "ByteSpan (Boolean)")]
    [DataRow("-64", DisplayName = "ByteSpan (SByte)")]
    [DataRow("F", DisplayName = "ByteSpan (Char)")]
    [DataRow("256,128", DisplayName = "ByteSpan (Double)")]
    [DataRow("420", DisplayName = "ByteSpan (UInt64)")]
    [DataRow("06/08/2022", DisplayName = "ByteSpan (DateOnly)")]
    [DataRow("1.00:30", DisplayName = "ByteSpan (TimeSpan)")]
    [DataRow("e3f947f2-cf32-441c-b31a-74e780cfc6f6", DisplayName = "ByteSpan (Guid)")]
    [DataRow("DarkGreen", DisplayName = "ByteSpan (ConsoleColor)")]
    public void ByteSpan(String value)
    {
        if (Boolean.TryParse(value, out Boolean booleanResult))
        {
            ByteSpan(booleanResult);
        }
        else if (SByte.TryParse(value, out SByte sbyteResult))
        {
            ByteSpan(sbyteResult);
        }
        else if (Char.TryParse(value, out Char charResult))
        {
            ByteSpan(charResult);
        }
        else if (UInt64.TryParse(value, out UInt64 uint64Result))
        {
            ByteSpan(uint64Result);
        }
        else if (Double.TryParse(value, out Double doubleResult))
        {
            ByteSpan(doubleResult);
        }
        else if (DateOnly.TryParse(value, out DateOnly dateOnlyResult))
        {
            ByteSpan(dateOnlyResult);
        }
        else if (TimeSpan.TryParse(value, out TimeSpan timeSpanResult))
        {
            ByteSpan(timeSpanResult);
        }
        else if (Guid.TryParse(value, out Guid guidResult))
        {
            ByteSpan(guidResult);
        }
        else if (Enum.TryParse(value, out ConsoleColor consoleColorResult))
        {
            ByteSpan(consoleColorResult);
        }
    }

    static private void ByteSpan<T>(T value)
        where T : unmanaged
    {
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(value)];
        UInt32 written = ByteSerializer.Serialize(buffer, value);
        UInt32 read = ByteSerializer.Deserialize(buffer, out T deserialized);

        Assert.AreEqual(written, read);
        Assert.AreEqual(value, deserialized);
    }

    [TestMethod]
    [DataRow("true", DisplayName = "BytePointer (Boolean)")]
    [DataRow("-64", DisplayName = "BytePointer (SByte)")]
    [DataRow("F", DisplayName = "BytePointer (Char)")]
    [DataRow("256,128", DisplayName = "BytePointer (Double)")]
    [DataRow("420", DisplayName = "BytePointer (UInt64)")]
    [DataRow("06/08/2022", DisplayName = "BytePointer (DateOnly)")]
    [DataRow("1.00:30", DisplayName = "BytePointer (TimeSpan)")]
    [DataRow("e3f947f2-cf32-441c-b31a-74e780cfc6f6", DisplayName = "BytePointer (Guid)")]
    [DataRow("DarkGreen", DisplayName = "BytePointer (ConsoleColor)")]
    public void BytePointer(String value)
    {
        if (Boolean.TryParse(value, out Boolean booleanResult))
        {
            BytePointer(booleanResult);
        }
        else if (SByte.TryParse(value, out SByte sbyteResult))
        {
            BytePointer(sbyteResult);
        }
        else if (Char.TryParse(value, out Char charResult))
        {
            BytePointer(charResult);
        }
        else if (UInt64.TryParse(value, out UInt64 uint64Result))
        {
            BytePointer(uint64Result);
        }
        else if (Double.TryParse(value, out Double doubleResult))
        {
            BytePointer(doubleResult);
        }
        else if (DateOnly.TryParse(value, out DateOnly dateOnlyResult))
        {
            BytePointer(dateOnlyResult);
        }
        else if (TimeSpan.TryParse(value, out TimeSpan timeSpanResult))
        {
            BytePointer(timeSpanResult);
        }
        else if (Guid.TryParse(value, out Guid guidResult))
        {
            BytePointer(guidResult);
        }
        else if (Enum.TryParse(value, out ConsoleColor consoleColorResult))
        {
            BytePointer(consoleColorResult);
        }
    }

    static private unsafe void BytePointer<T>(T value)
        where T : unmanaged
    {
        T deserialized = default;
        Byte[] buffer = new Byte[ByteSerializer.GetExpectedSerializedSize(value)];
        UInt32 written = 0;
        UInt32 read = 0;
        fixed (Byte* pointer = buffer)
        {
            written = ByteSerializer.Serialize(pointer, value);
            read = ByteSerializer.Deserialize(pointer, out deserialized);
        }

        Assert.AreEqual(written, read);
        Assert.AreEqual(value, deserialized);
    }

    [TestMethod]
    [DataRow("true", DisplayName = "IOStream (Boolean)")]
    [DataRow("-64", DisplayName = "IOStream (SByte)")]
    [DataRow("F", DisplayName = "IOStream (Char)")]
    [DataRow("256,128", DisplayName = "IOStream (Double)")]
    [DataRow("420", DisplayName = "IOStream (UInt64)")]
    [DataRow("06/08/2022", DisplayName = "IOStream (DateOnly)")]
    [DataRow("1.00:30", DisplayName = "IOStream (TimeSpan)")]
    [DataRow("e3f947f2-cf32-441c-b31a-74e780cfc6f6", DisplayName = "IOStream (Guid)")]
    [DataRow("DarkGreen", DisplayName = "IOStream (ConsoleColor)")]
    public void IOStream(String value)
    {
        if (Boolean.TryParse(value, out Boolean booleanResult))
        {
            IOStream(booleanResult);
        }
        else if (SByte.TryParse(value, out SByte sbyteResult))
        {
            IOStream(sbyteResult);
        }
        else if (Char.TryParse(value, out Char charResult))
        {
            IOStream(charResult);
        }
        else if (UInt64.TryParse(value, out UInt64 uint64Result))
        {
            IOStream(uint64Result);
        }
        else if (Double.TryParse(value, out Double doubleResult))
        {
            IOStream(doubleResult);
        }
        else if (DateOnly.TryParse(value, out DateOnly dateOnlyResult))
        {
            IOStream(dateOnlyResult);
        }
        else if (TimeSpan.TryParse(value, out TimeSpan timeSpanResult))
        {
            IOStream(timeSpanResult);
        }
        else if (Guid.TryParse(value, out Guid guidResult))
        {
            IOStream(guidResult);
        }
        else if (Enum.TryParse(value, out ConsoleColor consoleColorResult))
        {
            IOStream(consoleColorResult);
        }
    }

    static private void IOStream<T>(T value)
        where T : unmanaged
    {
        using MemoryStream stream = new();
        UInt32 written = ByteSerializer.Serialize(stream, value);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(stream, out T deserialized);

        Assert.AreEqual(written, read);
        Assert.AreEqual(value, deserialized);
    }

    [TestMethod]
    [DataRow("true", DisplayName = "IOStreamAsynchronous (Boolean)")]
    [DataRow("-64", DisplayName = "IOStreamAsynchronous (SByte)")]
    [DataRow("F", DisplayName = "IOStreamAsynchronous (Char)")]
    [DataRow("256,128", DisplayName = "IOStreamAsynchronous (Double)")]
    [DataRow("420", DisplayName = "IOStreamAsynchronous (UInt64)")]
    [DataRow("06/08/2022", DisplayName = "IOStreamAsynchronous (DateOnly)")]
    [DataRow("1.00:30", DisplayName = "IOStreamAsynchronous (TimeSpan)")]
    [DataRow("e3f947f2-cf32-441c-b31a-74e780cfc6f6", DisplayName = "IOStreamAsynchronous (Guid)")]
    [DataRow("DarkGreen", DisplayName = "IOStreamAsynchronous (ConsoleColor)")]
    public Task IOStreamAsynchronous(String value)
    {
        if (Boolean.TryParse(value, out Boolean booleanResult))
        {
            return IOStreamAsynchronous(booleanResult);
        }
        else if (SByte.TryParse(value, out SByte sbyteResult))
        {
            return IOStreamAsynchronous(sbyteResult);
        }
        else if (Char.TryParse(value, out Char charResult))
        {
            return IOStreamAsynchronous(charResult);
        }
        else if (UInt64.TryParse(value, out UInt64 uint64Result))
        {
            return IOStreamAsynchronous(uint64Result);
        }
        else if (Double.TryParse(value, out Double doubleResult))
        {
            return IOStreamAsynchronous(doubleResult);
        }
        else if (DateOnly.TryParse(value, out DateOnly dateOnlyResult))
        {
            return IOStreamAsynchronous(dateOnlyResult);
        }
        else if (TimeSpan.TryParse(value, out TimeSpan timeSpanResult))
        {
            return IOStreamAsynchronous(timeSpanResult);
        }
        else if (Guid.TryParse(value, out Guid guidResult))
        {
            return IOStreamAsynchronous(guidResult);
        }
        else if (Enum.TryParse(value, out ConsoleColor consoleColorResult))
        {
            return IOStreamAsynchronous(consoleColorResult);
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    static private async Task IOStreamAsynchronous<T>(T value)
        where T : unmanaged
    {
        using MemoryStream stream = new();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(stream, value);
        stream.Position = 0;
        AsynchronousDeserializationResult<T> result = await ByteSerializer.DeserializeAsynchronously<T>(stream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.AreEqual(value, result.Result);
    }

    [TestMethod]
    [DataRow("true", DisplayName = "InterfaceStream (Boolean)")]
    [DataRow("-64", DisplayName = "InterfaceStream (SByte)")]
    [DataRow("F", DisplayName = "InterfaceStream (Char)")]
    [DataRow("256,128", DisplayName = "InterfaceStream (Double)")]
    [DataRow("420", DisplayName = "InterfaceStream (UInt64)")]
    [DataRow("06/08/2022", DisplayName = "InterfaceStream (DateOnly)")]
    [DataRow("1.00:30", DisplayName = "InterfaceStream (TimeSpan)")]
    [DataRow("e3f947f2-cf32-441c-b31a-74e780cfc6f6", DisplayName = "InterfaceStream (Guid)")]
    [DataRow("DarkGreen", DisplayName = "InterfaceStream (ConsoleColor)")]
    public void InterfaceStream(String value)
    {
        if (Boolean.TryParse(value, out Boolean booleanResult))
        {
            InterfaceStream(booleanResult);
        }
        else if (SByte.TryParse(value, out SByte sbyteResult))
        {
            InterfaceStream(sbyteResult);
        }
        else if (Char.TryParse(value, out Char charResult))
        {
            InterfaceStream(charResult);
        }
        else if (UInt64.TryParse(value, out UInt64 uint64Result))
        {
            InterfaceStream(uint64Result);
        }
        else if (Double.TryParse(value, out Double doubleResult))
        {
            InterfaceStream(doubleResult);
        }
        else if (DateOnly.TryParse(value, out DateOnly dateOnlyResult))
        {
            InterfaceStream(dateOnlyResult);
        }
        else if (TimeSpan.TryParse(value, out TimeSpan timeSpanResult))
        {
            InterfaceStream(timeSpanResult);
        }
        else if (Guid.TryParse(value, out Guid guidResult))
        {
            InterfaceStream(guidResult);
        }
        else if (Enum.TryParse(value, out ConsoleColor consoleColorResult))
        {
            InterfaceStream(consoleColorResult);
        }
    }

    static private void InterfaceStream<T>(T value)
        where T : unmanaged
    {
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = ByteSerializer.Serialize(writeStream, value);
        stream.Position = 0;
        UInt32 read = ByteSerializer.Deserialize(readStream, out T deserialized);

        Assert.AreEqual(written, read);
        Assert.AreEqual(value, deserialized);
    }

    [TestMethod]
    [DataRow("true", DisplayName = "InterfaceStreamAsynchronous (Boolean)")]
    [DataRow("-64", DisplayName = "InterfaceStreamAsynchronous (SByte)")]
    [DataRow("F", DisplayName = "InterfaceStreamAsynchronous (Char)")]
    [DataRow("256,128", DisplayName = "InterfaceStreamAsynchronous (Double)")]
    [DataRow("420", DisplayName = "InterfaceStreamAsynchronous (UInt64)")]
    [DataRow("06/08/2022", DisplayName = "InterfaceStreamAsynchronous (DateOnly)")]
    [DataRow("1.00:30", DisplayName = "InterfaceStreamAsynchronous (TimeSpan)")]
    [DataRow("e3f947f2-cf32-441c-b31a-74e780cfc6f6", DisplayName = "InterfaceStreamAsynchronous (Guid)")]
    [DataRow("DarkGreen", DisplayName = "InterfaceStreamAsynchronous (ConsoleColor)")]
    public Task InterfaceStreamAsynchronous(String value)
    {
        if (Boolean.TryParse(value, out Boolean booleanResult))
        {
            return InterfaceStreamAsynchronous(booleanResult);
        }
        else if (SByte.TryParse(value, out SByte sbyteResult))
        {
            return InterfaceStreamAsynchronous(sbyteResult);
        }
        else if (Char.TryParse(value, out Char charResult))
        {
            return InterfaceStreamAsynchronous(charResult);
        }
        else if (UInt64.TryParse(value, out UInt64 uint64Result))
        {
            return InterfaceStreamAsynchronous(uint64Result);
        }
        else if (Double.TryParse(value, out Double doubleResult))
        {
            return InterfaceStreamAsynchronous(doubleResult);
        }
        else if (DateOnly.TryParse(value, out DateOnly dateOnlyResult))
        {
            return InterfaceStreamAsynchronous(dateOnlyResult);
        }
        else if (TimeSpan.TryParse(value, out TimeSpan timeSpanResult))
        {
            return InterfaceStreamAsynchronous(timeSpanResult);
        }
        else if (Guid.TryParse(value, out Guid guidResult))
        {
            return InterfaceStreamAsynchronous(guidResult);
        }
        else if (Enum.TryParse(value, out ConsoleColor consoleColorResult))
        {
            return InterfaceStreamAsynchronous(consoleColorResult);
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    static private async Task InterfaceStreamAsynchronous<T>(T value)
        where T : unmanaged
    {
        using MemoryStream stream = new();
        WriteableStreamWrapper writeStream = stream.AsWriteableStream();
        ReadableStreamWrapper readStream = stream.AsReadableStream();
        UInt32 written = await ByteSerializer.SerializeAsynchronously(writeStream, value);
        stream.Position = 0;
        AsynchronousDeserializationResult<T> result = await ByteSerializer.DeserializeAsynchronously<ReadableStreamWrapper, T>(readStream);

        Assert.AreEqual(written, result.BytesRead);
        Assert.AreEqual(value, result.Result);
    }
}