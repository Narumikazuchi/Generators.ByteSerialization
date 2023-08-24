﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.Managed_ICollection.JaggedArray;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Serialize
{
    [TestMethod]
    public async Task SimpleSerialize()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections.Generic;

public class Application
{
    static public Byte[] Run(List<Person>[][] graph)
    {
        return ByteSerializer.Serialize(graph);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task SerializeSafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections.Generic;

public class Application
{
    static public UInt32 Run(List<Person>[][] graph)
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, graph);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task SerializeUnsafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections.Generic;

public class Application
{
    static public unsafe Byte[] Run(List<Person>[][] graph)
    {
        Byte[] buffer = new Byte[16];
        fixed (Byte* pointer = buffer)
        {
            ByteSerializer.Serialize(pointer, graph);
        }
        return buffer;
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task SerializeIOStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections.Generic;
using System.IO;

public class Application
{
    static public void Run(List<Person>[][] graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, graph);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task SerializeStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.Collections.Generic;
using System.IO;

public class Application
{
    static public void Run(List<Person>[][] graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), graph);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsync()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(List<Person>[][] graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, graph, cancellationToken);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }

    [TestMethod]
    public async Task SerializeStreamAsync()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(List<Person>[][] graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), graph, cancellationToken);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }
}