using System;

namespace Debugging;

public sealed partial record class Intrinsic(DateTime DateTime,
                                             DateTimeOffset DateTimeOffset,
                                             String String,
                                             IBase Base);