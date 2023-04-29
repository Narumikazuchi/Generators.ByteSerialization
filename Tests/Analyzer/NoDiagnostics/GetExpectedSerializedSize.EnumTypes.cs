using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.NoDiagnostics;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
public partial class GetExpectedSerializedSize
{
    [TestMethod]
    public async Task StructOfEnumTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{
    public DayOfWeek DayOfWeek { get; set; }
    public ConsoleColor ConsoleColor { get; set; }
    public PlatformID PlatformID { get; set; }
}

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task StructOfEnumTypePropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{
    public DayOfWeek DayOfWeek { get; init; }
    public ConsoleColor ConsoleColor { get; init; }
    public PlatformID PlatformID { get; init; }
}

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyStructOfEnumTypePropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public readonly struct Test
{
    public DayOfWeek DayOfWeek { get; init; }
    public ConsoleColor ConsoleColor { get; init; }
    public PlatformID PlatformID { get; init; }
}

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task StructOfEnumTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{
    public DayOfWeek DayOfWeek;
    public ConsoleColor ConsoleColor;
    public PlatformID PlatformID;
}

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfEnumTypeParameters()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test(DayOfWeek DayOfWeek,
                          ConsoleColor ConsoleColor,
                          PlatformID PlatformID);

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfEnumTypeParameters()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public readonly record struct Test(DayOfWeek DayOfWeek,
                                   ConsoleColor ConsoleColor,
                                   PlatformID PlatformID);

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfEnumTypeParametersAndProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test(DayOfWeek DayOfWeek,
                          ConsoleColor ConsoleColor)
{
    public PlatformID PlatformID { get; set; } = default;
};

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfEnumTypeParametersAndPropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test(DayOfWeek DayOfWeek,
                          ConsoleColor ConsoleColor)
{
    public PlatformID PlatformID { get; init; } = default;
};

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfEnumTypeParametersAndProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public readonly record struct Test(DayOfWeek DayOfWeek,
                                   ConsoleColor ConsoleColor)
{
    public PlatformID PlatformID { get; init; } = default;
};

public class Application
{
    static public void Run()
    {
        ByteSerializer.GetExpectedSerializedSize(new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}