namespace Tests.Analyzer;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
public partial class NoDiagnostics
{
    [TestMethod]
    public async Task ClassOfInstrinsicTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial class Test
{
    public DateTime DateTime { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }
    public String String { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ClassOfInstrinsicTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial class Test
{
    public DateTime DateTime;
    public DateTimeOffset DateTimeOffset;
    public String String;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfInstrinsicTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial class Test
{
    public DateTime DateTime { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }
    public String String { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfInstrinsicTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial class Test
{
    public DateTime DateTime;
    public DateTimeOffset DateTimeOffset;
    public String String;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordClassOfInstrinsicTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial record class Test(DateTime DateTime,
                                 DateTimeOffset DateTimeOffset,
                                 String String);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedRecordClassOfInstrinsicTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial record class Test(DateTime DateTime,
                                        DateTimeOffset DateTimeOffset,
                                        String String);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfInstrinsicTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial record struct Test(DateTime DateTime,
                                  DateTimeOffset DateTimeOffset,
                                  String String);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfInstrinsicTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public readonly partial record struct Test(DateTime DateTime,
                                           DateTimeOffset DateTimeOffset,
                                           String String);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}