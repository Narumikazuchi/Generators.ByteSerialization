using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.Generators.ByteSerialization.Analyzers;
using Narumikazuchi.InputOutput;

namespace Tests;

public sealed class AnalyzerTest : CSharpAnalyzerTest<TypeAnalyzer, MSTestVerifier>
{
    static public async Task VerifyAnalyzerAsynchronously(String[] sources,
                                                          params DiagnosticResult[] expected)
    {
        AnalyzerTest test = new()
        {
            TestState =
            {
                ReferenceAssemblies = Net7.Assemblies,
                AdditionalReferences =
                {
                    typeof(ByteSerializer).Assembly.Location,
                    typeof(IStream).Assembly.Location
                }
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
                ReferenceAssemblies = Net7.Assemblies,
                AdditionalReferences =
                {
                    typeof(ByteSerializer).Assembly.Location,
                    typeof(IStream).Assembly.Location
                }
            }
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }

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
}