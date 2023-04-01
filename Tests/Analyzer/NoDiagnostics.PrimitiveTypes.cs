namespace Tests.Analyzer;

#pragma warning disable IDE1006 // No need to add postfix 'Asynchronously' here
[TestClass]
public partial class NoDiagnostics
{
    [TestMethod]
    public async Task ClassOfPrimitiveTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial class Test
{
    public Boolean Boolean { get; set; }
    public Byte Byte { get; set; }
    public SByte SByte { get; set; }
    public Char Char { get; set; }
    public Int16 Int16 { get; set; }
    public UInt16 UInt16 { get; set; }
    public Int32 Int32 { get; set; }
    public UInt32 UInt32 { get; set; }
    public Single Single { get; set; }
    public Double Double { get; set; }
    public Int64 Int64 { get; set; }
    public UInt64 UInt64 { get; set; }
    public Decimal Decimal { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ClassOfPrimitiveTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial class Test
{
    public Boolean Boolean;
    public Byte Byte;
    public SByte SByte;
    public Char Char;
    public Int16 Int16;
    public UInt16 UInt16;
    public Int32 Int32;
    public UInt32 UInt32;
    public Single Single;
    public Double Double;
    public Int64 Int64;
    public UInt64 UInt64;
    public Decimal Decimal;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfPrimitiveTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial class Test
{
    public Boolean Boolean { get; set; }
    public Byte Byte { get; set; }
    public SByte SByte { get; set; }
    public Char Char { get; set; }
    public Int16 Int16 { get; set; }
    public UInt16 UInt16 { get; set; }
    public Int32 Int32 { get; set; }
    public UInt32 UInt32 { get; set; }
    public Single Single { get; set; }
    public Double Double { get; set; }
    public Int64 Int64 { get; set; }
    public UInt64 UInt64 { get; set; }
    public Decimal Decimal { get; set; }
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedClassOfPrimitiveTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial class Test
{
    public Boolean Boolean;
    public Byte Byte;
    public SByte SByte;
    public Char Char;
    public Int16 Int16;
    public UInt16 UInt16;
    public Int32 Int32;
    public UInt32 UInt32;
    public Single Single;
    public Double Double;
    public Int64 Int64;
    public UInt64 UInt64;
    public Decimal Decimal;
}";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordClassOfPrimitiveTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial record class Test(Boolean Boolean,
                                 Byte Byte,
                                 SByte SByte,
                                 Char Char,
                                 Int16 Int16,
                                 UInt16 UInt16,
                                 Int32 Int32,
                                 UInt32 UInt32,
                                 Single Single,
                                 Double Double,
                                 Int64 Int64,
                                 UInt64 UInt64,
                                 Decimal Decimal);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task SealedRecordClassOfPrimitiveTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public sealed partial record class Test(Boolean Boolean,
                                        Byte Byte,
                                        SByte SByte,
                                        Char Char,
                                        Int16 Int16,
                                        UInt16 UInt16,
                                        Int32 Int32,
                                        UInt32 UInt32,
                                        Single Single,
                                        Double Double,
                                        Int64 Int64,
                                        UInt64 UInt64,
                                        Decimal Decimal);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task RecordStructOfPrimitiveTypeProperties()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public partial record struct Test(Boolean Boolean,
                                  Byte Byte,
                                  SByte SByte,
                                  Char Char,
                                  Int16 Int16,
                                  UInt16 UInt16,
                                  Int32 Int32,
                                  UInt32 UInt32,
                                  Single Single,
                                  Double Double,
                                  Int64 Int64,
                                  UInt64 UInt64,
                                  Decimal Decimal);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }

    [TestMethod]
    public async Task ReadonlyRecordStructOfPrimitiveTypeFields()
    {
        String source = @"using Narumikazuchi.Generators.ByteSerialization;
using System;

[ByteSerializable]
public readonly partial record struct Test(Boolean Boolean,
                                           Byte Byte,
                                           SByte SByte,
                                           Char Char,
                                           Int16 Int16,
                                           UInt16 UInt16,
                                           Int32 Int32,
                                           UInt32 UInt32,
                                           Single Single,
                                           Double Double,
                                           Int64 Int64,
                                           UInt64 UInt64,
                                           Decimal Decimal);";

        await AnalyzerTest.VerifyAnalyzerAsynchronously(source);
    }
}