using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Debugging;

public class Program
{
    static public void Main(String[] args)
    {
        TryComposite();
        //TryEnumerables();
        TryEnums();
        TryIntrinsic();
        TryNonRecord();
        TryPrimitive();
        TryRecordStruct();
        TryUnmanaged();
        TryWithStrategy();
        TryCollection();

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadLine();
    }

    static private unsafe void TryComposite()
    {
        Console.WriteLine("Starting Composite...");
        Composite source = new(/*new Enumerables(new Int64[] { 42, 69 },
                                               ImmutableArray.Create(new String[] { "Foo", "Bar" }),
                                               new() { new Vector2D { X = 42, Y = 69 } },
                                               new(new Enums[] { new(AttributeTargets.Delegate, ConsoleColor.Green) }),
                                               new AttributeTargets[] { AttributeTargets.Enum, AttributeTargets.GenericParameter },
                                               ImmutableArray.Create(new Contract { Contents = "MIT License" }),
                                               new() { new Version(3, 2, 1, 1254) }),*/
                               new Enums(AttributeTargets.Class,
                                         ConsoleColor.Cyan),
                               new Intrinsic(new DateOnly(2023, 5, 25),
                                             DateTime.Now,
                                             DateTimeOffset.UtcNow,
                                             new TimeOnly(2, 30, 45),
                                             TimeSpan.FromMilliseconds(69),
                                             "Hello World!"),
                               new NonRecord { Id = Guid.NewGuid(), Value = 420 },
                               new Primitive(true,
                                             0x90,
                                             0x7F,
                                             'Z',
                                             30225,
                                             61423,
                                             2001426541,
                                             4135746452,
                                             420.69f,
                                             69.42d,
                                             7777777777777L,
                                             7777777777777UL,
                                             99.99m),
                               new RecordStruct(new Vector2D { X = 69, Y = -420 },
                                                123L),
                               new Unmanaged((Half)512.256f,
                                             Guid.NewGuid(),
                                             new Vector2D { X = -69, Y = 69 }),
                               new WithStrategy(new Contract { Contents = "Open GNU License" },
                                                new Version(9, 4, 6, 12348)));

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Composite control);

        Console.WriteLine(source.Equals(control));
    }
    /*
    static private void TryEnumerables()
    {
        Console.WriteLine("Starting Composite...");
        Enumerables source = new(new Int64[] { 42, 69 },
                                 ImmutableArray.Create(new String[] { "Foo", "Bar" }),
                                 new() { new Vector2D { X = 42, Y = 69 } },
                                 new(new Enums[] { new(AttributeTargets.Delegate, ConsoleColor.Green) }),
                                 new AttributeTargets[] { AttributeTargets.Enum, AttributeTargets.GenericParameter },
                                 ImmutableArray.Create(new Contract { Contents = "MIT License" }),
                                 new() { new Version(3, 2, 1, 1254) });

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Enumerables control);

        Console.WriteLine(source.Equals(control));
    }
    */

    static private void TryCollection()
    {
        Console.WriteLine("Starting Collection...");
        Enums[] source = new Enums[]
        {
            new(AttributeTargets.Class,
                ConsoleColor.Cyan),
            new(AttributeTargets.Delegate,
                ConsoleColor.Magenta),
            new(AttributeTargets.Property,
                ConsoleColor.Yellow)
        };

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out ImmutableArray<Enums> control);

        Console.WriteLine(source.SequenceEqual(control));
    }

    static private void TryEnums()
    {
        Console.WriteLine("Starting Enums...");
        Enums source = new(AttributeTargets.Class,
                           ConsoleColor.Cyan);

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Enums control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryIntrinsic()
    {
        Console.WriteLine("Starting Intrinsic...");
        Intrinsic source = new(new DateOnly(2023, 5, 25),
                               DateTime.Now,
                               DateTimeOffset.UtcNow,
                               new TimeOnly(2, 30, 45),
                               TimeSpan.FromMilliseconds(69),
                               "Hello World!");

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Intrinsic control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryNonRecord()
    {
        Console.WriteLine("Starting NonRecord...");
        NonRecord source = new() { Id = Guid.NewGuid(), Value = 420 };

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out NonRecord control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryPrimitive()
    {
        Console.WriteLine("Starting Primitive...");
        Primitive source = new(true,
                               0x90,
                               0x7F,
                               'Z',
                               30225,
                               61423,
                               2001426541,
                               4135746452,
                               420.69f,
                               69.42d,
                               7777777777777L,
                               7777777777777UL,
                               99.99m);

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Primitive control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryRecordStruct()
    {
        Console.WriteLine("Starting RecordStruct...");
        RecordStruct source = new(new Vector2D { X = 69, Y = -420 },
                                  123L);

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out RecordStruct control);

        Console.WriteLine(source.Equals(control));
    }

    static private unsafe void TryUnmanaged()
    {
        Console.WriteLine("Starting Unmanaged...");
        Unmanaged source = new((Half)512.256f,
                               Guid.NewGuid(),
                               new Vector2D { X = -69, Y = 69 });

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Unmanaged control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryWithStrategy()
    {
        Console.WriteLine("Starting WithStrategy...");
        WithStrategy source = new(new Contract { Contents = "Open GNU License" },
                                  new Version(9, 4, 6, 12348));

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out WithStrategy control);

        Console.WriteLine(source.Equals(control));
    }
}