using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.Deserialize;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class NoImplementation
{
    [TestMethod]
    public async Task DeserializeNoImplementation()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public UInt32 Run(Byte[] buffer)
    {
        return ByteSerializer.Deserialize<ITest>(buffer, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(13, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeUnsafeNoImplementation()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public interface ITest
{
    public String Value { get; set; }
}

public class Application
{
    static public unsafe UInt32 Run(Byte[] buffer)
    {
        UInt32 result;
        fixed (Byte* pointer = buffer)
        {
            result = ByteSerializer.Deserialize<ITest>(pointer, out _);
        }
        return result;
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(16, 22),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeIOStreamNoImplementation()
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
    static public UInt32 Run(Stream stream)
    {
        return ByteSerializer.Deserialize<ITest>(stream, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(14, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeStreamNoImplementation()
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
    static public UInt32 Run<TStream>(TStream stream)
        where TStream : IReadableStream
    {
        return ByteSerializer.Deserialize<TStream, ITest>(stream, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(16, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeIOStreamAsyncNoImplementation()
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
    static public async Task Run(Stream stream, CancellationToken cancellationToken)
    {
        _ = await ByteSerializer.DeserializeAsynchronously<ITest>(stream, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(16, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeStreamAsyncNoImplementation()
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
    static public async Task Run<TStream>(TStream stream, CancellationToken cancellationToken)
        where TStream : IReadableStream
    {
        _ = await ByteSerializer.DeserializeAsynchronously<TStream, ITest>(stream, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG011", DiagnosticSeverity.Error).WithLocation(18, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}