using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.SpecialType_String.JaggedArray;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Deserialize
{
    [TestMethod]
    public async Task DeserializeSafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public UInt32 Run(Byte[] buffer)
    {
        return ByteSerializer.Deserialize<String[][]>(buffer, out _);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task DeserializeUnsafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public unsafe UInt32 Run(Byte[] buffer)
    {
        UInt32 result;
        fixed (Byte* pointer = buffer)
        {
            result = ByteSerializer.Deserialize<String[][]>(pointer, out _);
        }
        return result;
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task DeserializeIOStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{
    static public UInt32 Run(Stream stream)
    {
        return ByteSerializer.Deserialize<String[][]>(stream, out _);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task DeserializeStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application
{
    static public UInt32 Run<TStream>(TStream stream)
        where TStream : IReadableStream
    {
        return ByteSerializer.Deserialize<TStream, String[][]>(stream, out _);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task DeserializeIOStreamAsync()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(Stream stream, CancellationToken cancellationToken)
    {
        _ = await ByteSerializer.DeserializeAsynchronously<String[][]>(stream, cancellationToken);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task DeserializeStreamAsync()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run<TStream>(TStream stream, CancellationToken cancellationToken)
        where TStream : IReadableStream
    {
        _ = await ByteSerializer.DeserializeAsynchronously<TStream, String[][]>(stream, cancellationToken);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }
}