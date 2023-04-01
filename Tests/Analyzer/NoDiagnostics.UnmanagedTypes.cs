namespace Tests.Analyzer;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
public partial class NoDiagnostics
{
    [TestMethod]
    public async Task ClassOfUnmanagedTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public partial class Test
{
    public DateOnly DateOnly { get; set; }
    public TimeOnly TimeOnly { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public Guid Guid { get; set; }
    public Half Half { get; set; }
    public Vector2D Vector2D { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ClassOfUnmanagedTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public partial class Test
{
    public DateOnly DateOnly;
    public TimeOnly TimeOnly;
    public TimeSpan TimeSpan;
    public Guid Guid;
    public Half Half;
    public Vector2D Vector2D;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfUnmanagedTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public sealed partial class Test
{
    public DateOnly DateOnly { get; set; }
    public TimeOnly TimeOnly { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public Guid Guid { get; set; }
    public Half Half { get; set; }
    public Vector2D Vector2D { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfUnmanagedTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public sealed partial class Test
{
    public DateOnly DateOnly;
    public TimeOnly TimeOnly;
    public TimeSpan TimeSpan;
    public Guid Guid;
    public Half Half;
    public Vector2D Vector2D;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordClassOfUnmanagedTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public partial record class Test(DateOnly DateOnly,
                                 TimeOnly TimeOnly,
                                 TimeSpan TimeSpan,
                                 Guid Guid,
                                 Half Half,
                                 Vector2D Vector2D);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedRecordClassOfUnmanagedTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public sealed partial record class Test(DateOnly DateOnly,
                                        TimeOnly TimeOnly,
                                        TimeSpan TimeSpan,
                                        Guid Guid,
                                        Half Half,
                                        Vector2D Vector2D);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfUnmanagedTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public partial record struct Test(DateOnly DateOnly,
                                  TimeOnly TimeOnly,
                                  TimeSpan TimeSpan,
                                  Guid Guid,
                                  Half Half,
                                  Vector2D Vector2D);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfUnmanagedTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

public struct Vector2D
{
    public Int32 X { get; set; }
    public Int32 Y;
}

[ByteSerializable]
public readonly partial record struct Test(DateOnly DateOnly,
                                           TimeOnly TimeOnly,
                                           TimeSpan TimeSpan,
                                           Guid Guid,
                                           Half Half,
                                           Vector2D Vector2D);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}