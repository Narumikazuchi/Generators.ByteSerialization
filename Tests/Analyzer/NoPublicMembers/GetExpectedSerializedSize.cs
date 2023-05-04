using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.NoPublicMembers;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class GetExpectedSerializedSize
{
    [TestMethod]
    public async Task GetExpectedSize()
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
    static public Int32 Run(Test graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG014", DiagnosticSeverity.Warning).WithLocation(18, 16),
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}