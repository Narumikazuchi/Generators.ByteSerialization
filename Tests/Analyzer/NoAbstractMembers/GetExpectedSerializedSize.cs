using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.NoAbstractMembers;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class GetExpectedSerializedSize
{
    [TestMethod]
    public async Task GetExpectedSerializedSizeNoAbstractMembers()
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
    static public Int32 Run(ITest graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG013", DiagnosticSeverity.Info).WithLocation(18, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}