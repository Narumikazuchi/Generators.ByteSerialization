using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.Deserialize;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class ConsiderUnmanaged
{
    [TestMethod]
    public async Task DeserializeConsiderUnmanaged()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public sealed class Test
{
    public Test(Int32 value)
    {
        m_Value = value;
    }

    public Int32 m_Value;
}

public class Application
{
    static public UInt32 Run(Byte[] buffer)
    {
        return ByteSerializer.Deserialize<Test>(buffer, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(18, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeUnsafeConsiderUnmanaged()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public sealed class Test
{
    public Test(Int32 value)
    {
        m_Value = value;
    }

    public Int32 m_Value;
}

public class Application
{
    static public unsafe UInt32 Run(Byte[] buffer)
    {
        UInt32 result;
        fixed (Byte* pointer = buffer)
        {
            result = ByteSerializer.Deserialize<Test>(pointer, out _);
        }
        return result;
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(21, 22),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeIOStreamConsiderUnmanaged()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public sealed class Test
{
    public Test(Int32 value)
    {
        m_Value = value;
    }

    public Int32 m_Value;
}

public class Application
{
    static public UInt32 Run(Stream stream)
    {
        return ByteSerializer.Deserialize<Test>(stream, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(19, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeStreamConsiderUnmanaged()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public sealed class Test
{
    public Test(Int32 value)
    {
        m_Value = value;
    }

    public Int32 m_Value;
}

public class Application
{
    static public UInt32 Run<TStream>(TStream stream)
        where TStream : IReadableStream
    {
        return ByteSerializer.Deserialize<TStream, Test>(stream, out _);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(21, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeIOStreamAsyncConsiderUnmanaged()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public sealed class Test
{
    public Test(Int32 value)
    {
        m_Value = value;
    }

    public Int32 m_Value;
}

public class Application
{
    static public async Task Run(Stream stream, CancellationToken cancellationToken)
    {
        _ = await ByteSerializer.DeserializeAsynchronously<Test>(stream, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(21, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task DeserializeStreamAsyncConsiderUnmanaged()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public sealed class Test
{
    public Test(Int32 value)
    {
        m_Value = value;
    }

    public Int32 m_Value;
}

public class Application
{
    static public async Task Run<TStream>(TStream stream, CancellationToken cancellationToken)
        where TStream : IReadableStream
    {
        _ = await ByteSerializer.DeserializeAsynchronously<TStream, Test>(stream, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(23, 19),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}