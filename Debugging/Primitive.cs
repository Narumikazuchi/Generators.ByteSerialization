using System;

namespace Debugging;

public sealed partial record class Primitive(Boolean Boolean,
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
                                             Decimal Decimal);