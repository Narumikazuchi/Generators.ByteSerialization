namespace Tests.Analyzer;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
public partial class NoDiagnostics
{
    [TestMethod]
    public async Task ClassOfEnumTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial class Test
{
    public DayOfWeek DayOfWeek { get; set; }
    public ConsoleColor ConsoleColor { get; set; }
    public PlatformID PlatformID { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ClassOfEnumTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial class Test
{
    public DayOfWeek DayOfWeek;
    public ConsoleColor ConsoleColor;
    public PlatformID PlatformID;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfEnumTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial class Test
{
    public DayOfWeek DayOfWeek { get; set; }
    public ConsoleColor ConsoleColor { get; set; }
    public PlatformID PlatformID { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfEnumTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial class Test
{
    public DayOfWeek DayOfWeek;
    public ConsoleColor ConsoleColor;
    public PlatformID PlatformID;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordClassOfEnumTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial record class Test(DayOfWeek DayOfWeek,
                                 ConsoleColor ConsoleColor,
                                 PlatformID PlatformID);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedRecordClassOfEnumTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial record class Test(DayOfWeek DayOfWeek,
                                        ConsoleColor ConsoleColor,
                                        PlatformID PlatformID);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfEnumTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial record struct Test(DayOfWeek DayOfWeek,
                                  ConsoleColor ConsoleColor,
                                  PlatformID PlatformID);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfEnumTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public readonly partial record struct Test(DayOfWeek DayOfWeek,
                                           ConsoleColor ConsoleColor,
                                           PlatformID PlatformID);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}