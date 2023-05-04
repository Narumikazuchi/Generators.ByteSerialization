using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.ConsiderUnmanaged;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class GetExpectedSerializedSize
{
    [TestMethod]
    public async Task GetExpectedSize()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public Int32 Run(StoreMap graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG016", DiagnosticSeverity.Info).WithLocation(8, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(new String[] { source, ClassSources.STOREMAP_FILE_SOURCE }, results);
    }
}