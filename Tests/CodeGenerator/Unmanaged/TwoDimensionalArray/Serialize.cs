using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.Unmanaged.TwoDimensionalArray;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Serialize
{
    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task SimpleSerialize(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public Byte[] Run({type}[,] graph)
    {{
        return ByteSerializer.Serialize(graph);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }

    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task SerializeSafe(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public UInt32 Run({type}[,] graph)
    {{
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, graph);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }

    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task SerializeUnsafe(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public unsafe Byte[] Run({type}[,] graph)
    {{
        Byte[] buffer = new Byte[16];
        fixed (Byte* pointer = buffer)
        {{
            ByteSerializer.Serialize(pointer, graph);
        }}
        return buffer;
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }

    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task SerializeIOStream(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{{
    static public void Run({type}[,] graph)
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, graph);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }

    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task SerializeStream(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application
{{
    static public void Run({type}[,] graph)
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), graph);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }

    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task SerializeIOStreamAsync(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{{
    static public async Task Run({type}[,] graph, CancellationToken cancellationToken)
    {{
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, graph, cancellationToken);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }

    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task SerializeStreamAsync(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{{
    static public async Task Run({type}[,] graph, CancellationToken cancellationToken)
    {{
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), graph, cancellationToken);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }
}