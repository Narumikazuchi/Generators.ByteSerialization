namespace Tests.Analyzer;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public partial class PointersNotSerialized
{
    [TestMethod]
    public async Task GetSizeIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public Int32 Run()
    {
        return ByteSerializer.GetExpectedSerializedSize(IntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(8, 16),
            new DiagnosticResult("NCG011", DiagnosticSeverity.Warning).WithLocation(8, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }

    [TestMethod]
    public async Task GetSizeUIntPtr()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{
    static public Int32 Run()
    {
        return ByteSerializer.GetExpectedSerializedSize(UIntPtr.Zero);
    }
}";
        DiagnosticResult[] results = new DiagnosticResult[]
        {
            new DiagnosticResult("NCG009", DiagnosticSeverity.Error).WithLocation(8, 16),
            new DiagnosticResult("NCG011", DiagnosticSeverity.Warning).WithLocation(8, 16),
        };

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source, results);
    }
}