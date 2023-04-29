using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.Serialize;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class OpenGenericsUnsupported
{
    [TestMethod]
    public async Task SimpleSerializeOpenGeneric()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application<T>
{
    static public Byte[] Run(ArraySegment<T> graph)
    {
        return ByteSerializer.Serialize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG010", DiagnosticSeverity.Error).WithLocation(8, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeOpenGeneric()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application<T>
{
    static public UInt32 Run(ArraySegment<T> graph)
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG010", DiagnosticSeverity.Error).WithLocation(9, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeUnsafeOpenGeneric()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application<T>
{
    static public unsafe Byte[] Run(ArraySegment<T> graph)
    {
        Byte[] buffer = new Byte[16];
        fixed (Byte* pointer = buffer)
        {
            ByteSerializer.Serialize(pointer, graph);
        }
        return buffer;
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG010", DiagnosticSeverity.Error).WithLocation(11, 13),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamOpenGeneric()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application<T>
{
    static public void Run(ArraySegment<T> graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG010", DiagnosticSeverity.Error).WithLocation(10, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamOpenGeneric()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application<T>
{
    static public void Run(ArraySegment<T> graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG010", DiagnosticSeverity.Error).WithLocation(11, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsyncOpenGeneric()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application<T>
{
    static public async Task Run(ArraySegment<T> graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG010", DiagnosticSeverity.Error).WithLocation(12, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamAsyncOpenGeneric()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application<T>
{
    static public async Task Run(ArraySegment<T> graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG010", DiagnosticSeverity.Error).WithLocation(13, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}