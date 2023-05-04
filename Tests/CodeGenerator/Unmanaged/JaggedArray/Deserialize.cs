using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.Unmanaged.JaggedArray;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Deserialize
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
    public async Task DeserializeSafe(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public UInt32 Run(Byte[] buffer)
    {{
        return ByteSerializer.Deserialize<{type}[][]>(buffer, out _);
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
    public async Task DeserializeUnsafe(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public unsafe UInt32 Run(Byte[] buffer)
    {{
        UInt32 result;
        fixed (Byte* pointer = buffer)
        {{
            result = ByteSerializer.Deserialize<{type}[][]>(pointer, out _);
        }}
        return result;
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
    public async Task DeserializeIOStream(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{{
    static public UInt32 Run(Stream stream)
    {{
        return ByteSerializer.Deserialize<{type}[][]>(stream, out _);
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
    public async Task DeserializeStream(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application
{{
    static public UInt32 Run<TStream>(TStream stream)
        where TStream : IReadableStream
    {{
        return ByteSerializer.Deserialize<TStream, {type}[][]>(stream, out _);
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
    public async Task DeserializeIOStreamAsync(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{{
    static public async Task Run(Stream stream, CancellationToken cancellationToken)
    {{
        _ = await ByteSerializer.DeserializeAsynchronously<{type}[][]>(stream, cancellationToken);
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
    public async Task DeserializeStreamAsync(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{{
    static public async Task Run<TStream>(TStream stream, CancellationToken cancellationToken)
        where TStream : IReadableStream
    {{
        _ = await ByteSerializer.DeserializeAsynchronously<TStream, {type}[][]>(stream, cancellationToken);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }
}