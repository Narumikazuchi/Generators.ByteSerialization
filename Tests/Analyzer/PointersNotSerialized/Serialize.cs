using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.PointersNotSerialized;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Serialize
{
    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task SimpleSerializePointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public Byte[] Run()
    {{
        return ByteSerializer.Serialize({type}.Zero);
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
    public async Task SerializePointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public UInt32 Run()
    {{
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, {type}.Zero);
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
    public async Task SerializeUnsafePointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public unsafe Byte[] Run()
    {{
        Byte[] buffer = new Byte[16];
        fixed (Byte* pointer = buffer)
        {{
            ByteSerializer.Serialize(pointer, {type}.Zero);
        }}
        return buffer;
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 13),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 13),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task SerializeIOStreamPointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, {type}.Zero);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(10, 9),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(10, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task SerializeStreamPointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), {type}.Zero);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 9),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task SerializeIOStreamAsyncPointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{{
    static public async Task Run(CancellationToken cancellationToken)
    {{
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, {type}.Zero, cancellationToken);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(12, 15),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(12, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task SerializeStreamAsyncPointersNotSerialized(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{{
    static public async Task Run(CancellationToken cancellationToken)
    {{
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), {type}.Zero, cancellationToken);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(13, 15),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(13, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}