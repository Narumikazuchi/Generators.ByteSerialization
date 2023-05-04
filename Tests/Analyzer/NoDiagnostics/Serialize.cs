using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.NoDiagnostics;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public partial class Serialize
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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task StructOfProperties(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{{
    public {type} Property {{ get; set; }}
}}

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task StructOfInitProperties(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{{
    public {type} Property {{ get; init; }}
}}

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task ReadonlyStructOfProperties(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public readonly struct Test
{{
    public {type} Property {{ get; init; }}
}}

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task StructOfFields(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Test
{{
    public {type} Field;
}}

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task RecordStructOfParameters(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test({type} Property);

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task RecordStructOfProperties(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test()
{{
    public {type} Property {{ get; set; }} = default;
}}

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task RecordStructOfInitProperties(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public record struct Test()
{{
    public {type} Property {{ get; init; }} = default;
}}

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

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
    [DataRow("DayOfWeek")]
    [DataRow("ConsoleColor")]
    [DataRow("PlatformID")]
    [DataRow("String")]
    public async Task ReadonlyRecordStructOfProperties(String type)
    {
        String source = $@"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public readonly record struct Test()
{{
    public {type} Property {{ get; init; }} = default;
}}

public class Application
{{
    static public void Run()
    {{
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }}
}}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}