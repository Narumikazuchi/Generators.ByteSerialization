namespace Narumikazuchi.Generators.ByteSerialization;

static public class Extensions
{
    static public String ToTypename(this ITypeSymbol type)
    {
        if (type.ContainingNamespace is null &&
            type is IArrayTypeSymbol array)
        {
            return array.ElementType.ToTypename() + "[]";
        }
        else if (type.ContainingNamespace is not null &&
                 type.ContainingNamespace.ToDisplayString() is "System")
        {
            return type.Name;
        }
        else
        {
            return type.ToDisplayString();
        }
    }

    static public Int32 UnmanagedSize(this ITypeSymbol type)
    {
        if (type.TypeKind is TypeKind.Enum)
        {
            if (type is INamedTypeSymbol named)
            {
                return named.EnumUnderlyingType.UnmanagedSize();
            }
            else
            {
                return 4;
            }
        }

        String typename = type.ToTypename();
        return typename switch
        {
            nameof(Boolean) or 
            nameof(Byte) or 
            nameof(SByte) => 1,

            nameof(Char) or 
            nameof(Int16) or 
            nameof(UInt16) => 2,

            nameof(Int32) or 
            nameof(Single) or 
            nameof(UInt32) => 4,

            nameof(Double) or 
            nameof(Int64) or 
            nameof(UInt64) => 8,

            nameof(Decimal) => 16,

            _ => type.GetMembers()
                     .OfType<IFieldSymbol>()
                     .Sum(field => field.Type.UnmanagedSize()),
        };
    }

    static public String EnumerableCount(this ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol ||
            type.ToTypename().StartsWith("System.Collections.Immutable.ImmutableArray<"))
        {
            return ".Length";
        }
        else if (type.AllInterfaces.Any(@interface => @interface.ToDisplayString()
                                                                .StartsWith("System.Collections.Generic.ICollection<") ||
                                                      @interface.ToDisplayString()
                                                                .StartsWith("System.Collections.Generic.IReadOnlyCollection<")))
        {
            return ".Count";
        }
        else
        {
            return ".Count()";
        }
    }

    static public Boolean IsSerializable(this ITypeSymbol type)
    {
        return type.GetAttributes().Any(attribute => attribute.AttributeClass is not null &&
                                                     attribute.AttributeClass.ToDisplayString() is Generators.SerializableGenerator.BYTESERIALIZABLE_ATTRIBUTE);
    }

    static public ImmutableArray<ISymbol> AllMembers(this ITypeSymbol type)
    {
        ImmutableArray<ISymbol>.Builder builder = ImmutableArray.CreateBuilder<ISymbol>();
        ITypeSymbol symbol = type;
        while (symbol is not null)
        {
            foreach (ISymbol member in symbol.GetMembers())
            {
                builder.Add(member);
            }

            symbol = symbol.BaseType;
        }

        if (type.BaseType is null)
        {
            foreach (ITypeSymbol @interface in type.AllInterfaces)
            {
                foreach (ISymbol member in @interface.GetMembers())
                {
                    builder.Add(member);
                }
            }
        }

        return builder.ToImmutable();
    }

    static public Boolean IsEnumerable(this ITypeSymbol type,
                                       out ITypeSymbol elementType)
    {
        if (type is IArrayTypeSymbol array)
        {
            elementType = array.ElementType;
            return true;
        }
        else
        {
            ImmutableArray<IMethodSymbol> methods = type.AllMembers()
                                                        .OfType<IMethodSymbol>()
                                                        .Where(method => method.Name.Contains(nameof(IEnumerable.GetEnumerator)))
                                                        .Where(method => method.Parameters.Length is 0)
                                                        .Where(method => !method.ReturnsVoid)
                                                        .ToImmutableArray();
            foreach (IMethodSymbol method in methods)
            {
                if (method.MethodKind is MethodKind.ExplicitInterfaceImplementation)
                {
                    continue;
                }

                if (method.ReturnType.IsEnumerator(out elementType))
                {
                    return true;
                }
            }

            foreach (IMethodSymbol method in methods)
            {
                if (method.ReturnType.IsEnumerator(out elementType))
                {
                    return true;
                }
            }

            elementType = null;
            return false;
        }
    }

    static public Boolean IsSimpleEnumerable(this ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol ||
            type.ToTypename().StartsWith("System.Collections.Immutable.ImmutableArray<"))
        {
            return false;
        }
        else if (type.AllInterfaces.Any(@interface => @interface.ToDisplayString()
                                                                .StartsWith("System.Collections.Generic.ICollection<") ||
                                                      @interface.ToDisplayString()
                                                                .StartsWith("System.Collections.Generic.IReadOnlyCollection<")))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    static public Boolean IsEnumerator(this ITypeSymbol type,
                                       out ITypeSymbol elementType)
    {
        IMethodSymbol moveNext = type.AllMembers()
                                     .OfType<IMethodSymbol>()
                                     .Where(method => method.Name.Contains(nameof(IEnumerator.MoveNext)))
                                     .Where(method => method.ReturnType.ToTypename() is "Boolean")
                                     .FirstOrDefault();
        IPropertySymbol current = type.AllMembers()
                                      .OfType<IPropertySymbol>()
                                      .Where(property => property.Name.Contains(nameof(IEnumerator.Current)))
                                      .FirstOrDefault();
        if (current is not null)
        {
            elementType = current.Type;
        }
        else
        {
            elementType = null;
        }

        return moveNext is not null &&
               current is not null;
    }
}