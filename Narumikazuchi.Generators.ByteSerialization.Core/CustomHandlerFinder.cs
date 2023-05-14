using Microsoft.CodeAnalysis;

namespace Narumikazuchi.Generators.ByteSerialization;

static public class CustomHandlerFinder
{
    static public ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> FindTypesWithCustomHandlerIn(Compilation compilation)
    {
        INamedTypeSymbol @interface = compilation.GetTypeByMetadataName($"{GlobalNames.NAMESPACE}.{GlobalNames.ISERIALIZATIONHANDLER}");

        ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>>.Builder builder = ImmutableDictionary.CreateBuilder<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>>(SymbolEqualityComparer.Default);
        ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> results;
        foreach (MetadataReference reference in compilation.ExternalReferences)
        {
            ISymbol assemblyOrModule = compilation.GetAssemblyOrModuleSymbol(reference);
            if (assemblyOrModule is IAssemblySymbol assembly &&
                !assembly.Name.StartsWith("System"))
            {
                results = TypesWithCustomHandlerIn(@namespace: assembly.GlobalNamespace,
                                                   @interface: @interface);
                foreach (KeyValuePair<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> element in results)
                {
                    try
                    {
                        builder.Add(key: element.Key,
                                    value: element.Value);
                    }
                    catch { }
                }
            }
            else if (assemblyOrModule is IModuleSymbol module &&
                     !module.Name.StartsWith("System"))
            {
                results = TypesWithCustomHandlerIn(@namespace: module.GlobalNamespace,
                                                   @interface: @interface);
                foreach (KeyValuePair<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> element in results)
                {
                    try
                    {
                        builder.Add(key: element.Key,
                                    value: element.Value);
                    }
                    catch { }
                }
            }
        }

        results = TypesWithCustomHandlerIn(@namespace: compilation.Assembly.GlobalNamespace,
                                           @interface: @interface);
        foreach (KeyValuePair<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> element in results)
        {
            try
            {
                builder.Add(key: element.Key,
                            value: element.Value);
            }
            catch { }
        }

        return builder.ToImmutable();
    }

    static private ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> TypesWithCustomHandlerIn(INamespaceSymbol @namespace,
                                                                                                                 INamedTypeSymbol @interface)
    {
        ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>>.Builder builder = ImmutableDictionary.CreateBuilder<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>>(SymbolEqualityComparer.Default);
        foreach (INamespaceOrTypeSymbol member in @namespace.GetMembers())
        {
            if (member is INamespaceSymbol symbol &&
                !symbol.Name.StartsWith("System"))
            {
                ImmutableDictionary<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> results = TypesWithCustomHandlerIn(@namespace: symbol,
                                                                                                                        @interface: @interface);
                foreach (KeyValuePair<ITypeSymbol, ImmutableHashSet<INamedTypeSymbol>> element in results)
                {
                    try
                    {
                        builder.Add(key: element.Key,
                                    value: element.Value);
                    }
                    catch { }
                }
            }
            else if (member is INamedTypeSymbol type)
            {
                if (type.IsCompilerGenerated())
                {
                    continue;
                }

                foreach (INamedTypeSymbol implements in type.AllInterfaces.Where(implements => implements.MetadataName is GlobalNames.ISERIALIZATIONHANDLER))
                {
                    if (!SymbolEqualityComparer.Default.Equals(@interface, implements.ConstructedFrom))
                    {
                        continue;
                    }

                    if (builder.TryGetValue(key: implements.TypeArguments[0],
                                            value: out ImmutableHashSet<INamedTypeSymbol> types))
                    {
                        builder[implements.TypeArguments[0]] = types.Add(type);
                    }
                    else
                    {
                        builder.Add(key: implements.TypeArguments[0],
                                    value: ImmutableHashSet.Create<INamedTypeSymbol>(SymbolEqualityComparer.Default, type));
                    }
                }
            }
        }

        return builder.ToImmutable();
    }
}