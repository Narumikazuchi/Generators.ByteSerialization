using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.Deserialize;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class NoAbstractMembers
{
    [TestMethod]
    public async Task DeserializeNoAbstractMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public interface ITest
{
    protected String Value { get; }
}

public class Test : ITest
{
    String ITest.Value { get; } = String.Empty;
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
            new DiagnosticResult("NCG013", DiagnosticSeverity.Info).WithLocation(18, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeUnsafeNoAbstractMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public interface ITest
{
    protected String Value { get; }
}

public class Test : ITest
{
    String ITest.Value { get; } = String.Empty;
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
            new DiagnosticResult("NCG013", DiagnosticSeverity.Info).WithLocation(21, 22),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeIOStreamNoAbstractMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public interface ITest
{
    protected String Value { get; }
}

public class Test : ITest
{
    String ITest.Value { get; } = String.Empty;
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
            new DiagnosticResult("NCG013", DiagnosticSeverity.Info).WithLocation(19, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeStreamNoAbstractMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public interface ITest
{
    protected String Value { get; }
}

public class Test : ITest
{
    String ITest.Value { get; } = String.Empty;
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
            new DiagnosticResult("NCG013", DiagnosticSeverity.Info).WithLocation(21, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeIOStreamAsyncNoAbstractMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ITest
{
    protected String Value { get; }
}

public class Test : ITest
{
    String ITest.Value { get; } = String.Empty;
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
            new DiagnosticResult("NCG013", DiagnosticSeverity.Info).WithLocation(21, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeStreamAsyncNoAbstractMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ITest
{
    protected String Value { get; }
}

public class Test : ITest
{
    String ITest.Value { get; } = String.Empty;
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
            new DiagnosticResult("NCG013", DiagnosticSeverity.Info).WithLocation(23, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}