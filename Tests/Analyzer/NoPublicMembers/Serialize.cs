using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.NoPublicMembers;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class Serialize
{
    [TestMethod]
    public async Task SimpleSerializeNoPublicMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Test
{
    public Test(String value)
    {
        m_Value = value;
    }

    private readonly String m_Value;
}

public class Application
{
    static public Byte[] Run(Test graph)
    {
        return ByteSerializer.Serialize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(18, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeNoPublicMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Test
{
    public Test(String value)
    {
        m_Value = value;
    }

    private readonly String m_Value;
}

public class Application
{
    static public UInt32 Run(Test graph)
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(19, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeUnsafeNoPublicMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Test
{
    public Test(String value)
    {
        m_Value = value;
    }

    private readonly String m_Value;
}

public class Application
{
    static public unsafe Byte[] Run(Test graph)
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
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(21, 13),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamNoPublicMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Test
{
    public Test(String value)
    {
        m_Value = value;
    }

    private readonly String m_Value;
}

public class Application
{
    static public void Run(Test graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(20, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamNoPublicMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Test
{
    public Test(String value)
    {
        m_Value = value;
    }

    private readonly String m_Value;
}

public class Application
{
    static public void Run(Test graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(21, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsyncNoPublicMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Test
{
    public Test(String value)
    {
        m_Value = value;
    }

    private readonly String m_Value;
}

public class Application
{
    static public async Task Run(Test graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(22, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamAsyncNoPublicMembers()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Test
{
    public Test(String value)
    {
        m_Value = value;
    }

    private readonly String m_Value;
}

public class Application
{
    static public async Task Run(Test graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(23, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}