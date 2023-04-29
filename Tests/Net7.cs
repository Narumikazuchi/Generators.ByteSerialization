using Microsoft.CodeAnalysis.Testing;

namespace Tests;

static public class Net7
{
    static public ReferenceAssemblies Assemblies { get; } = new("net7.0",
                                                                new PackageIdentity("Microsoft.NETCore.App.Ref",
                                                                                    "7.0.4"),
                                                                Path.Combine("ref", "net7.0"));
}