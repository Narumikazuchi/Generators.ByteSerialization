﻿using Microsoft.CodeAnalysis;
using Narumikazuchi.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

static public class GlobalNames
{
    static public String ISerializationHandler(ITypeSymbol ofType)
    {
        return $"Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{ofType.ToFrameworkString()}>";
    }

    static public String ISerializationHandler(String ofType)
    {
        return $"Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<{ofType}>";
    }

    public const String NAMESPACE = "Narumikazuchi.Generators.ByteSerialization";
    public const String BYTESERIALIZER = NAMESPACE + ".ByteSerializer";
    public const String IBYTESERIALIZER = NAMESPACE + ".IByteSerializer";
}