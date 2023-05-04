﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.ManagedWithDefaultConstructor.JaggedArray;

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
    static public Int32 Run(Person[][] graph)
    {
        return ByteSerializer.GetExpectedSerializedSize(graph);
    }
}";

        await GeneratorTest.VerifySourceGeneratorAsynchronously(new String[] { ClassSources.PERSON_FILE_SOURCE, source }, AssemblySource.ExpectedSource, ExpectedSource.Handler);
    }
}