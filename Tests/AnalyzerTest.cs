using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.Generators.ByteSerialization.Analyzers;
using Narumikazuchi.InputOutput;

namespace Tests;

public sealed class InvocationAnalyzerTest : CSharpAnalyzerTest<InvocationAnalyzer, MSTestVerifier>
{
    static public async Task VerifyAnalyzerAsynchronously(String[] sources,
                                                          params DiagnosticResult[] expected)
    {
        InvocationAnalyzerTest test = new()
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

        InvocationAnalyzerTest test = new()
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

    public InvocationAnalyzerTest()
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

public sealed class TypeAnalyzerTest : CSharpAnalyzerTest<TypeAnalyzer, MSTestVerifier>
{
    static public async Task VerifyAnalyzerAsynchronously(String[] sources,
                                                          params DiagnosticResult[] expected)
    {
        TypeAnalyzerTest test = new()
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

        TypeAnalyzerTest test = new()
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

    public TypeAnalyzerTest()
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