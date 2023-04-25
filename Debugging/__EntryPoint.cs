using Narumikazuchi.Generators.ByteSerialization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Debugging;

public class BaseClass<T>
    where T : unmanaged, IComparable<T>
{
    public BaseClass(T value)
    {
        M_1 = value;
    }

    public T M_1 { get; set; }
}

public sealed class Arity<T1, T2, T3> : BaseClass<T1>
    where T1 : unmanaged, IComparable<T1>
    where T2 : struct, IComparable<T2>, IEquatable<T2>
    where T3 : class
{
    public Arity(T1 value) : base(value)
    { }

    private T2 m_2;
    private T3 m_3;
}

internal class __EntryPoint
{
    static internal unsafe async Task Main(String[] args)
    {
        ByteSerializer.Serialize(new Vector2D[] { new() { X = 420, Y = 69 }, new() { Y = 420, X = 69 } });
        ByteSerializer.Serialize(new List<Primitive> { });
        ByteSerializer.Serialize(new Dictionary<Int32, Double> { });
        ByteSerializer.Serialize(Array.Empty<Dictionary<Int32, Unmanaged>>());
        ByteSerializer.Serialize(true);
        ByteSerializer.Serialize(420);
        ByteSerializer.Serialize(DateTime.UtcNow);
        ByteSerializer.Serialize(DateTimeOffset.UtcNow);
        ByteSerializer.Serialize("Foobar");
        ByteSerializer.Serialize(new Vector2D { X = 420, Y = 69 });
        ByteSerializer.Serialize(new RecordStruct());
        Byte[] data = ByteSerializer.Serialize<IBase>(new Derived());
        ByteSerializer.Serialize<Derived>(new Sealed());
        ByteSerializer.Serialize(new Arity<Guid, Int32, Sealed>(Guid.NewGuid()));
        ByteSerializer.Serialize(ConsoleColor.White);
        ByteSerializer.Serialize(UIntPtr.Zero);

        ByteSerializer.Serialize(Array.Empty<DateTimeOffset>());
        ByteSerializer.Serialize(new DateTimeOffset[,,] { });
        ByteSerializer.Serialize(new DateTimeOffset[][][] { });
        ByteSerializer.Serialize(new DateTimeOffset[][,][] { });
        ByteSerializer.Serialize(new DateTimeOffset[][][,] { });
        ByteSerializer.Serialize(Array.Empty<String>());
        ByteSerializer.Serialize(Array.Empty<Int64>());
        ByteSerializer.Serialize(Array.Empty<Vector2D>());
        ByteSerializer.Serialize(new List<Vector2D>());
        ByteSerializer.Serialize(Array.Empty<List<Vector2D>>());
        ByteSerializer.Serialize(Array.Empty<Dictionary<Guid, Vector2D>>());
        ByteSerializer.Serialize(Array.Empty<NonRecord>());
        ByteSerializer.Serialize(Array.Empty<RecordStruct>());
        /*
        TryComposite();
        TryEnumerables();
        TryEnums();
        TryIntrinsic();
        TryNonRecord();
        TryPrimitive();
        TryRecordStruct();
        TryUnmanaged();
        TryCollection();
        */
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadLine();
    }
    /*
    static private void TryComposite()
    {
        Console.WriteLine("Starting Composite...");
        Composite source = new(new Enumerables(new Int64[] { 42, 69 },
                                               ImmutableArray.Create(new String[] { "Foo", "Bar" }),
                                               new() { new Vector2D { X = 42, Y = 69 } },
                                               new(new Enums[] { new(AttributeTargets.Delegate, ConsoleColor.Green) }),
                                               new AttributeTargets[] { AttributeTargets.Enum, AttributeTargets.GenericParameter },
                                               new() { { 64, 420d } }),
                               new Enums(AttributeTargets.Class,
                                         ConsoleColor.Cyan),
                               new Intrinsic(DateTime.Now,
                                             DateTimeOffset.UtcNow,
                                             "Hello World!",
                                             new Derived()),
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
                                             new Vector2D { X = -69, Y = 69 },
                                             42));

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Composite? control);

        Console.WriteLine(source.Equals(control));
    }
    static private void TryEnumerables()
    {
        Console.WriteLine("Starting Enumerables...");
        Enumerables source = new(new Int64[] { 42, 69 },
                                 ImmutableArray.Create(new String[] { "Foo", "Bar" }),
                                 new() { new Vector2D { X = 42, Y = 69 } },
                                 new(new Enums[] { new(AttributeTargets.Delegate, ConsoleColor.Green) }),
                                 new AttributeTargets[] { AttributeTargets.Enum, AttributeTargets.GenericParameter },
                                 new() { { 64, 420d } });

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Enumerables control);

        Console.WriteLine(source.Equals(control));
    }
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

        _ = ByteSerializer.Deserialize(buffer, out Enums? control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryIntrinsic()
    {
        Console.WriteLine("Starting Intrinsic...");
        Intrinsic source = new(DateTime.Now,
                               DateTimeOffset.UtcNow,
                               "Hello World!",
                               new Sealed());

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Intrinsic? control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryNonRecord()
    {
        Console.WriteLine("Starting NonRecord...");
        NonRecord source = new() { Id = Guid.NewGuid(), Value = 420 };

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out NonRecord? control);

        Console.WriteLine(source.Equals(control));
    }

    static private void TryPrimitive()
    {
        Console.WriteLine("Starting Primitive...");
        Primitive source = new(true,
                               0x90,
                               0x7F,
                               'A',
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

        _ = ByteSerializer.Deserialize(buffer, out Primitive? control);

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

    static private void TryUnmanaged()
    {
        Console.WriteLine("Starting Unmanaged...");
        Unmanaged source = new((Half)512.256f,
                               Guid.NewGuid(),
                               new Vector2D { X = -69, Y = 69 },
                               42);

        Byte[] buffer = ByteSerializer.Serialize(source);

        _ = ByteSerializer.Deserialize(buffer, out Unmanaged? control);

        Console.WriteLine(source.Equals(control));
    }
  */
}