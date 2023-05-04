using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.ConsiderUnmanaged;

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
        return ByteSerializer.Deserialize<StoreMap>(buffer, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(8, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
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
            result = ByteSerializer.Deserialize<StoreMap>(pointer, out _);
        }
        return result;
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(11, 22),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
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
        return ByteSerializer.Deserialize<StoreMap>(stream, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(9, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
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
        return ByteSerializer.Deserialize<TStream, StoreMap>(stream, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(11, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
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
        _ = await ByteSerializer.DeserializeAsynchronously<StoreMap>(stream, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(11, 19),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
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
        _ = await ByteSerializer.DeserializeAsynchronously<TStream, StoreMap>(stream, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(13, 19),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }
}