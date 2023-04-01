using Narumikazuchi.Generators.ByteSerialization;

namespace Debugging;

[ByteSerializable]
public sealed partial record class Composite(Enumerables Enumerables,
                                             Enums Enums,
                                             Intrinsic Intrinsic,
                                             NonRecord NonRecord,
                                             Primitive Primitive,
                                             RecordStruct RecordStruct,
                                             Unmanaged Unmanaged,
                                             WithStrategy WithStrategy);