using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.ManagedWithDefaultConstructor.SingleObject;

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
        return ByteSerializer.Deserialize<Person>(buffer, out _);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
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
            result = ByteSerializer.Deserialize<Person>(pointer, out _);
        }
        return result;
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
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
        return ByteSerializer.Deserialize<Person>(stream, out _);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
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
        return ByteSerializer.Deserialize<TStream, Person>(stream, out _);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
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
        _ = await ByteSerializer.DeserializeAsynchronously<Person>(stream, cancellationToken);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
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
        _ = await ByteSerializer.DeserializeAsynchronously<TStream, Person>(stream, cancellationToken);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }
}