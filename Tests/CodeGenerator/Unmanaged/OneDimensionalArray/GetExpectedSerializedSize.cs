using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CodeGenerator.Unmanaged.OneDimensionalArray;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public class GetExpectedSerializedSize
{
    [TestMethod]
    [DataRow("Boolean")]
    [DataRow("Byte")]
    [DataRow("SByte")]
    [DataRow("Char")]
    [DataRow("Half")]
    [DataRow("Int16")]
    [DataRow("UInt16")]
    [DataRow("Int32")]
    [DataRow("UInt32")]
    [DataRow("Single")]
    [DataRow("Int64")]
    [DataRow("UInt64")]
    [DataRow("Double")]
    [DataRow("Decimal")]
    [DataRow("DateOnly")]
    [DataRow("DateTime")]
    [DataRow("DateTimeOffset")]
    [DataRow("TimeSpan")]
    [DataRow("TimeOnly")]
    [DataRow("Guid")]
    public async Task GetExpectedSize(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;

public class Application
{{
    static public Int32 Run({type}[] graph)
    {{
        return ByteSerializer.GetExpectedSerializedSize(graph);
    }}
}}";

        String expectedFilename = ExpectedSource.HANDLER_FILENAME.Replace("{0}", type);
        SourceText expectedSource = SourceText.From(text: ExpectedSource.HANDLER_SOURCE.Replace("{0}", type),
                                                    encoding: Encoding.UTF8);
        await GeneratorTest.VerifySourceGeneratorAsynchronously(source, AssemblySource.ExpectedSource, (expectedFilename, expectedSource));
    }
}