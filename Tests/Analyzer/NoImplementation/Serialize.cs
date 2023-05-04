using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.NoImplementation;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Serialize
{
    [TestMethod]
    public async Task SimpleSerialize()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public Byte[] Run(ITest graph)
    {
        return ByteSerializer.Serialize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(13, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeSafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public UInt32 Run(ITest graph)
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(14, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeUnsafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public unsafe Byte[] Run(ITest graph)
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
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(16, 13),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public void Run(ITest graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(15, 9),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public void Run(ITest graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(16, 9),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsync()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public async Task Run(ITest graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(17, 15),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamAsync()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public async Task Run(ITest graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(18, 15),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}