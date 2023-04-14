namespace Narumikazuchi.Generators.ByteSerialization.Generators;

[Generator(LanguageNames.CSharp)]
public sealed partial class SerializableGenerator
{
    static private void GenerateSerializationCode(SourceProductionContext context,
                                                  (Compilation, ImmutableArray<ITypeSymbol>) compilationAndTypes)
    {
        (Compilation compilation, ImmutableArray<ITypeSymbol> types) = compilationAndTypes;
        if (types.IsDefaultOrEmpty)
        {
            return;
        }

        types = types.Distinct((IEqualityComparer<ITypeSymbol>)SymbolEqualityComparer.Default)
                     .Where(t => t.BaseType is not null)
                     .ToImmutableArray();

        EmitAssemblyHandler(context: context,
                            compilation: compilation);

        IAssemblySymbol assembly = compilation.Assembly;

        foreach (ITypeSymbol type in types)
            {
            if (type is INamedTypeSymbol named)
                {
                GenerateSources(assembly: assembly,
                                symbol: named,
                                   context: context);
                }
                catch { }
            }
        }
    }

    static private void GenerateSources(IAssemblySymbol assembly,
                                        INamedTypeSymbol symbol,
                                       SourceProductionContext context)
    {
        ImmutableArray<IFieldSymbol> fields = __Shared.GetFieldsToSerialize(symbol);

        if (symbol.BaseType is not null)
        {
            GenerateConstructorMediatorSource(symbol: symbol,
                                    fields: fields,
                                    context: context);

            GenerateConstructorSource(symbol: symbol,
                                    fields: fields,
                                    context: context);
        }
        
        GenerateHandlerSource(assembly: assembly, 
                              symbol: symbol,
                                  fields: fields,
                              context: context);
    }
}