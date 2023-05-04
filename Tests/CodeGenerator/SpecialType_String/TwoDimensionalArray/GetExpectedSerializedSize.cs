using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.SpecialType_String.TwoDimensionalArray;

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
    static public Int32 Run(String[,] graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }
}