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
    public async Task GetExpectedSize(String type)
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
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(8, 16)
        };

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}