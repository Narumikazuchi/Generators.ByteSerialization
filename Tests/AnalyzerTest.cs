using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.Generators.ByteSerialization.Analyzers;

namespace Tests;

public sealed class AnalyzerTest : CSharpAnalyzerTest<TypeAnalyzer, MSTestVerifier>
{
    public AnalyzerTest()
    {
        this.SolutionTransforms.Add((solution, projectId) =>
        {
            CompilationOptions compilationOptions = solution.GetProject(projectId)!.CompilationOptions!;
            compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(compilationOptions.SpecificDiagnosticOptions.SetItem("CS1705", ReportDiagnostic.Hidden));
            solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

            return solution;
        });
    }

    static public async Task VerifyAnalyzerAsynchronously(String[] sources,
                                                          params DiagnosticResult[] expected)
    {
        AnalyzerTest test = new()
        {
            TestState =
            {
                AdditionalReferences = { typeof(ByteSerializableAttribute).Assembly.Location.Replace("net6.0", "netstandard2.0") }
            }
        };

        foreach (String source in sources)
        {
            test.TestState.Sources.Add(source);
        }

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }
    static public async Task VerifyAnalyzerAsynchronously(String source,
                                                          params DiagnosticResult[] expected)
    {
        AnalyzerTest test = new()
        {
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { typeof(ByteSerializableAttribute).Assembly.Location.Replace("net6.0", "netstandard2.0") }
            }
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }
}