using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.ConsiderUnmanaged;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class GetExpectedSerializedSize
{
    [TestMethod]
    public async Task GetExpectedSerializedSizeConsiderUnmanaged()
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
    static public Int32 Run(Test graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Info).WithLocation(18, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}