using Microsoft.CodeAnalysis;

namespace Narumikazuchi.CodeAnalysis;

public delegate ImmutableArray<TResult> SyntaxFilter<TResult>(SymbolInfo symbolInfo);