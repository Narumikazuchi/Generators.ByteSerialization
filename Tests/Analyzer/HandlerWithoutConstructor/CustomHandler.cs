using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.HandlerWithoutConstructor;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class CustomHandler
{
    [TestMethod]
    public async Task CustomHandler_PrivateConstructor()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class CustomHandler : ISerializationHandler<Uri>
{
    private CustomHandler()
    { }

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
            new DiagnosticResult("NCG017", DiagnosticSeverity.Error).WithLocation(4, 14),
        };

        await TypeAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task CustomHandler_ProtectedConstructor()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class CustomHandler : ISerializationHandler<Uri>
{
    protected CustomHandler()
    { }

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
            new DiagnosticResult("NCG017", DiagnosticSeverity.Error).WithLocation(4, 14),
        };

        await TypeAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task CustomHandler_ParameterizedConstructor()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class CustomHandler : ISerializationHandler<Uri>
{
    public CustomHandler(Int32 any = default)
    {
        Placeholder = any;
    }

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

    public Int32 Placeholder { get; }
}";

        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG017", DiagnosticSeverity.Error).WithLocation(4, 14),
        };

        await TypeAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}