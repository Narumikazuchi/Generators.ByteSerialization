using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

public class Program
{
    static public void Main(String[] args)
    {
        Data data = new("root-user", 69m, DateTime.Now, new(Guid.NewGuid(), "Lorem ipsum", Flags.No8), new(Guid.NewGuid(), "John", "Doe"));

        Byte[] bytes = ByteSerializer.Serialize(data);

        _ = ByteSerializer.Deserialize(bytes, out Data remote);
        Boolean equals = remote.Equals(data);

        Console.WriteLine(equals);
        Console.ReadLine();
    }
}