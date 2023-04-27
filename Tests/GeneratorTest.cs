using Narumikazuchi.Generators.ByteSerialization;
using Narumikazuchi.Generators.ByteSerialization.Generators;
using Narumikazuchi.InputOutput;

namespace Tests;

public sealed class GeneratorTest : SourceGeneratorTest<MSTestVerifier>
{
    static public async Task VerifySourceGeneratorAsynchronously(String[] sources,
                                                                 params (String filename, SourceText content)[] expected)
    {
        GeneratorTest test = new()
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

        foreach ((String filename, SourceText content) item in expected)
        {
            test.TestState.GeneratedSources.Add(item);
        }

        await test.RunAsync(CancellationToken.None);
    }
    static public async Task VerifySourceGeneratorAsynchronously(String source,
                                                                 params (String filename, SourceText content)[] expected)
    {
        GeneratorTest test = new()
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
        foreach ((String filename, SourceText content) item in expected)
        {
            test.TestState.GeneratedSources.Add(item);
        }

        await test.RunAsync(CancellationToken.None);
    }

    public override String Language
    {
        get
        {
            return LanguageNames.CSharp;
        }
    }

    protected override CompilationOptions CreateCompilationOptions()
    {
        return new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary,
                                            allowUnsafe: true);
    }

    protected override GeneratorDriver CreateGeneratorDriver(Project project,
                                                             ImmutableArray<ISourceGenerator> sourceGenerators)
    {
        return CSharpGeneratorDriver.Create(generators: sourceGenerators,
                                            additionalTexts: project.AnalyzerOptions.AdditionalFiles,
                                            parseOptions: (CSharpParseOptions)project.ParseOptions!,
                                            optionsProvider: project.AnalyzerOptions.AnalyzerConfigOptionsProvider);
    }

    protected override ParseOptions CreateParseOptions()
    {
        return new CSharpParseOptions(languageVersion: s_DefaultLanguageVersion,
                                      documentationMode: DocumentationMode.Diagnose);
    }

    protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
    {
        return new ISourceGenerator[] { new SerializableGenerator().AsSourceGenerator() };
    }

    protected override String DefaultFileExt
    {
        get
        {
            return "cs";
        }
    }

    static private readonly LanguageVersion s_DefaultLanguageVersion =
        Enum.TryParse("Default", out LanguageVersion version) ? version : (LanguageVersion)1100;
}