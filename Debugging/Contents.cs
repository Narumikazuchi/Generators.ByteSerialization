using Narumikazuchi.Generators.ByteSerialization;
using System;

namespace Debugging;

[ByteSerializable]
public partial record struct Contents(Guid CreatedBy,
                                      String Text,
                                      Flags Flags);

public enum Flags : Int64
{
    No1 = 0x1,
    No2 = 0x2,
    No4 = 0x4,
    No8 = 0x8
}