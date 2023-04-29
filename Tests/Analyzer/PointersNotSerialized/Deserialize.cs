using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.PointersNotSerialized;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Deserialize
{
    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task DeserializePointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public UInt32 Run(Byte[] buffer)
    {{
        return ByteSerializer.Deserialize<{type}>(buffer, out _);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(8, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(8, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task DeserializeUnsafePointersNotSerialized(String type)
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
            result = ByteSerializer.Deserialize<{type}>(pointer, out _);
        }}
        return result;
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 22),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 22),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task DeserializeIOStreamPointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{{
    static public UInt32 Run(Stream stream)
    {{
        return ByteSerializer.Deserialize<{type}>(stream, out _);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(9, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(9, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task DeserializeStreamPointersNotSerialized(String type)
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
        return ByteSerializer.Deserialize<TStream, {type}>(stream, out _);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task DeserializeIOStreamAsyncPointersNotSerialized(String type)
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
        _ = await ByteSerializer.DeserializeAsynchronously<{type}>(stream, cancellationToken);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 19),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task DeserializeStreamAsyncPointersNotSerialized(String type)
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
        _ = await ByteSerializer.DeserializeAsynchronously<TStream, {type}>(stream, cancellationToken);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(13, 19),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(13, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}