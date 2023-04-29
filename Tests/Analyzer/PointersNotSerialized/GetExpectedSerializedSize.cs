using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.PointersNotSerialized;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class GetExpectedSerializedSize
{
    [TestMethod]
    [DataRow("IntPtr")]
    [DataRow("UIntPtr")]
    public async Task GetExpectedSerializedSizeIntPtr(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public Int32 Run()
    {{
        return ByteSerializer.GetExpectedSerializedSize({type}.Zero);
    }}
}}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(8, 16),
            new DiagnosticResult("NCG012", DiagnosticSeverity.Warning).WithLocation(8, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}