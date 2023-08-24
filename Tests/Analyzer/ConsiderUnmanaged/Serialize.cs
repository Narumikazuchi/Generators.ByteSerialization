using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.ConsiderUnmanaged;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Serialize
{
    [TestMethod]
    public async Task SimpleSerialize()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public ReadOnlySpan<Byte> Run(StoreMap graph)
    {
        return ByteSerializer.Serialize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(8, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }

    [TestMethod]
    public async Task SerializeSafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public UInt32 Run(StoreMap graph)
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(9, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }

    [TestMethod]
    public async Task SerializeUnsafe()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public unsafe Byte[] Run(StoreMap graph)
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
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(11, 13),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }

    [TestMethod]
    public async Task SerializeIOStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{
    static public void Run(StoreMap graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(10, 9),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }

    [TestMethod]
    public async Task SerializeStream()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application
{
    static public void Run(StoreMap graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(11, 9),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsync()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(StoreMap graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(12, 15),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
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

public class Application
{
    static public async Task Run(StoreMap graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(13, 15),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }
}