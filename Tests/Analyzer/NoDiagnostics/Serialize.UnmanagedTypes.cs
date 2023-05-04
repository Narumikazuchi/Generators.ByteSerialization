using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Analyzer.NoDiagnostics;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
public partial class Serialize
{
    [TestMethod]
    public async Task StructOfUnmanagedTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public struct Test
{
    public Byte Byte { get; set; }
    public Char Char { get; set; }
    public Single Single { get; set; }
    public Int64 Int64 { get; set; }
    public Decimal Decimal { get; set; }
    public Guid Guid { get; set; }
    public Vector2D Vector2D { get; set; }
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task StructOfUnmanagedTypePropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public struct Test
{
    public Byte Byte { get; init; }
    public Char Char { get; init; }
    public Single Single { get; init; }
    public Int64 Int64 { get; init; }
    public Decimal Decimal { get; init; }
    public Guid Guid { get; init; }
    public Vector2D Vector2D { get; init; }
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyStructOfUnmanagedTypePropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public readonly struct Test
{
    public Byte Byte { get; init; }
    public Char Char { get; init; }
    public Single Single { get; init; }
    public Int64 Int64 { get; init; }
    public Decimal Decimal { get; init; }
    public Guid Guid { get; init; }
    public Vector2D Vector2D { get; init; }
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task StructOfUnmanagedTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public struct Test
{
    public Byte Byte;
    public Char Char;
    public Single Single;
    public Int64 Int64;
    public Decimal Decimal;
    public Guid Guid;
    public Vector2D Vector2D;
}

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfUnmanagedTypeParameters()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public record struct Test(Byte Byte,
                          Char Char,
                          Single Single,
                          Int64 Int64,
                          Decimal Decimal,
                          Guid Guid,
                          Vector2D Vector2D);

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfUnmanagedTypeParameters()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public readonly record struct Test(Byte Byte,
                                   Char Char,
                                   Single Single,
                                   Int64 Int64,
                                   Decimal Decimal,
                                   Guid Guid,
                                   Vector2D Vector2D);

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfUnmanagedTypeParametersAndProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public record struct Test(Byte Byte,
                          Char Char,
                          Single Single,
                          Int64 Int64,
                          Decimal Decimal)
{
    public Guid Guid { get; set; } = default;
    public Vector2D Vector2D { get; set; } = default;
};

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfUnmanagedTypeParametersAndPropertiesInit()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public record struct Test(Byte Byte,
                          Char Char,
                          Single Single,
                          Int64 Int64,
                          Decimal Decimal)
{
    public Guid Guid { get; init; } = default;
    public Vector2D Vector2D { get; init; } = default;
};

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfUnmanagedTypeParametersAndProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.IO;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}

public readonly record struct Test(Byte Byte,
                                   Char Char,
                                   Single Single,
                                   Int64 Int64,
                                   Decimal Decimal)
{
    public Guid Guid { get; init; } = default;
    public Vector2D Vector2D { get; init; } = default;
};

public class Application
{
    static public void Run()
    {
        using MemoryStream stream = new MemoryStream();
        ByteSerializer.Serialize(stream, new Test());
    }
}";

        await InvocationAnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}