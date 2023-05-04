using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.MultipleHandlersForType;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class CustomHandler
{
    [TestMethod]
    public async Task CustomHandler_SameFile()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class CustomHandler : ISerializationHandler<Uri>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Uri? result)
    {
        throw new NotImplementedException();
    }

    public Int32 GetExpectedArraySize(Uri? graph)
    {
        throw new NotImplementedException();
    }

    public UInt32 Serialize(Span<Byte> buffer, Uri? graph)
    {
        throw new NotImplementedException();
    }
}

public class CustomHandler2 : ISerializationHandler<Uri>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Uri? result)
    {
        throw new NotImplementedException();
    }

    public Int32 GetExpectedArraySize(Uri? graph)
    {
        throw new NotImplementedException();
    }

    public UInt32 Serialize(Span<Byte> buffer, Uri? graph)
    {
        throw new NotImplementedException();
    }
}";

        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG018", DiagnosticSeverity.Error).WithLocation(4, 14),
            new DiagnosticResult("NCG018", DiagnosticSeverity.Error).WithLocation(22, 14),
        };

        await TypeAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task CustomHandler_DifferentFiles()
    {
        String source1 = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class CustomHandler : ISerializationHandler<Uri>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Uri? result)
    {
        throw new NotImplementedException();
    }

    public Int32 GetExpectedArraySize(Uri? graph)
    {
        throw new NotImplementedException();
    }

    public UInt32 Serialize(Span<Byte> buffer, Uri? graph)
    {
        throw new NotImplementedException();
    }
}";
        String source2 = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class CustomHandler2 : ISerializationHandler<Uri>
{
    public UInt32 Deserialize(ReadOnlySpan<Byte> buffer, out Uri? result)
    {
        throw new NotImplementedException();
    }

    public Int32 GetExpectedArraySize(Uri? graph)
    {
        throw new NotImplementedException();
    }

    public UInt32 Serialize(Span<Byte> buffer, Uri? graph)
    {
        throw new NotImplementedException();
    }
}";

        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG018", DiagnosticSeverity.Error).WithLocation("/0/Test0.cs", 4, 14),
            new DiagnosticResult("NCG018", DiagnosticSeverity.Error).WithLocation("/0/Test1.cs", 4, 14),
        };

        await TypeAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source1, source2 }, results);
    }
}