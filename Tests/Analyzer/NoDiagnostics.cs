using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public partial class NoDiagnostics
{
    [TestMethod]
    public async Task StructOfStringProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{
    public String String { get; set; }
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task StructOfStringPropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{
    public String String { get; init; }
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyStructOfStringPropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public readonly struct Test
{
    public String String { get; init; }
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task StructOfStringFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{
    public String String;
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfStringParameters()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test(String String);

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfStringProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test()
{
    public String String { get; set; } = default;
};

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfStringPropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test()
{
    public String String { get; init; } = default;
};

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfStringProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public readonly record struct Test()
{
    public String String { get; init; } = default;
};

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}