﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.Serialize;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class ConsiderUnmanaged
{
    [TestMethod]
    public async Task SimpleSerializeConsiderUnmanaged()
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
    static public Byte[] Run(Test graph)
    {
        return ByteSerializer.Serialize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(18, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeConsiderUnmanaged()
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
    static public UInt32 Run(Test graph)
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(19, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeUnsafeConsiderUnmanaged()
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
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(21, 13),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamConsiderUnmanaged()
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
    static public void Run(Test graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(20, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamConsiderUnmanaged()
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
    static public void Run(Test graph)
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(21, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsyncConsiderUnmanaged()
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
    static public async Task Run(Test graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(22, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamAsyncConsiderUnmanaged()
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
    static public async Task Run(Test graph, CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), graph, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(23, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}