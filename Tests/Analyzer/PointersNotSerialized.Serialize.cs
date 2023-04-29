using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
public partial class PointersNotSerialized
{
    [TestMethod]
    public async Task SimpleSerializeIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public Byte[] Run()
    {
        return ByteSerializer.Serialize(IntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(8, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(8, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SimpleSerializeUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public Byte[] Run()
    {
        return ByteSerializer.Serialize(UIntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(8, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(8, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public UInt32 Run()
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, IntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(9, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(9, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public UInt32 Run()
    {
        Byte[] buffer = new Byte[16];
        return ByteSerializer.Serialize(buffer, UIntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(9, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(9, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeUnsafeIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public unsafe Byte[] Run()
    {
        Byte[] buffer = new Byte[16];
        fixed (Byte* pointer = buffer)
        {
            ByteSerializer.Serialize(pointer, IntPtr.Zero);
        }
        return buffer;
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 13),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 13),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeUnsafeUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public unsafe Byte[] Run()
    {
        Byte[] buffer = new Byte[16];
        fixed (Byte* pointer = buffer)
        {
            ByteSerializer.Serialize(pointer, UIntPtr.Zero);
        }
        return buffer;
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 13),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 13),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, IntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(10, 9),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(10, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, UIntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(10, 9),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(10, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), IntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 9),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream.AsWriteableStream(), UIntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(11, 9),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(11, 9),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsyncIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, IntPtr.Zero, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(12, 15),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(12, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeIOStreamAsyncUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream, UIntPtr.Zero, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(12, 15),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(12, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamAsyncIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), IntPtr.Zero, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(13, 15),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(13, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task SerializeStreamAsyncUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.InputOutput;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Application
{
    static public async Task Run(CancellationToken cancellationToken)
    {
        using MemoryStream stream = new MemoryStream();
        await ByteSerializer.SerializeAsynchronously(stream.AsWriteableStream(), UIntPtr.Zero, cancellationToken);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(13, 15),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(13, 15),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}