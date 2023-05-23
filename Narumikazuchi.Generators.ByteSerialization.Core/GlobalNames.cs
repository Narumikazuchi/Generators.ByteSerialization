using Microsoft.CodeAnalysis;
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
    public const String IGNOREFORSERIALIZATIONATTRIBUTE = NAMESPACE + ".IgnoreForSerializationAttribute";
    public const String ISERIALIZATIONHANDLER = "ISerializationHandler`1";
    public const String SERIALIZEFROMMEMBERATTRIBUTE = NAMESPACE + ".SerializeFromMemberAttribute";
    public const String SERIALIZEDEFAULTATTRIBUTE = NAMESPACE + ".SerializeDefaultAttribute";
    public const String DATALAYOUTATTRIBUTE = NAMESPACE + ".DataLayoutAttribute";
    public const String DATALAYOUTPOSITIONATTRIBUTE = NAMESPACE + ".DataLayoutPositionAttribute";
}