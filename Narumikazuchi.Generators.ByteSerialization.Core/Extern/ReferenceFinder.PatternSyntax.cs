using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Narumikazuchi.CodeAnalysis;

public partial class ReferenceFinder
{
    static public ImmutableArray<TResult> FindInPatterns<TResult>(IEnumerable<PatternSyntax> patterns,
                                                                  SemanticModel semanticModel,
                                                                  SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        foreach (PatternSyntax pattern in patterns)
        {
            ImmutableArray<TResult> results = FindInPattern(pattern: pattern,
                                                            semanticModel: semanticModel,
                                                            filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }

    static public ImmutableArray<TResult> FindInPattern<TResult>(PatternSyntax pattern,
                                                                 SemanticModel semanticModel,
                                                                 SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(pattern);
        if (symbolInfo.Symbol is not null ||
            symbolInfo.CandidateSymbols.Length > 0)
        {
            ImmutableArray<TResult> filtered = filter.Invoke(symbolInfo);
            builder.AddRange(filtered);
        }

        if (pattern is BinaryPatternSyntax binary)
        {
            ImmutableArray<TResult> results = FindInPattern(pattern: binary.Left,
                                                            semanticModel: semanticModel,
                                                            filter: filter);
            builder.AddRange(results);

            results = FindInPattern(pattern: binary.Right,
                                    semanticModel: semanticModel,
                                    filter: filter);
            builder.AddRange(results);
        }
        else if (pattern is ConstantPatternSyntax constant)
        {
            return FindInExpression(expression: constant.Expression,
                                    semanticModel: semanticModel,
                                    filter: filter);
        }
        else if (pattern is DeclarationPatternSyntax declaration)
        {
            symbolInfo = semanticModel.GetSymbolInfo(declaration.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (pattern is ListPatternSyntax list)
        {
            ImmutableArray<TResult> results = FindInPatterns(patterns: list.Patterns,
                                                             semanticModel: semanticModel,
                                                             filter: filter);
            builder.AddRange(results);
        }
        else if (pattern is ParenthesizedPatternSyntax parenthesized)
        {
            ImmutableArray<TResult> results = FindInPattern(pattern: parenthesized.Pattern,
                                                            semanticModel: semanticModel,
                                                            filter: filter);
            builder.AddRange(results);
        }
        else if (pattern is RecursivePatternSyntax recursive)
        {
            symbolInfo = semanticModel.GetSymbolInfo(recursive.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);

            if (recursive.PositionalPatternClause is not null)
            {
                foreach (SubpatternSyntax subpattern in recursive.PositionalPatternClause.Subpatterns)
                {
                    results = FindInExpression(expression: subpattern.ExpressionColon.Expression,
                                               semanticModel: semanticModel,
                                               filter: filter);
                    builder.AddRange(results);

                    results = FindInPattern(pattern: subpattern.Pattern,
                                            semanticModel: semanticModel,
                                            filter: filter);
                    builder.AddRange(results);
                }
            }

            if (recursive.PropertyPatternClause is not null)
            {
                foreach (SubpatternSyntax subpattern in recursive.PropertyPatternClause.Subpatterns)
                {
                    results = FindInExpression(expression: subpattern.ExpressionColon.Expression,
                                               semanticModel: semanticModel,
                                               filter: filter);
                    builder.AddRange(results);

                    results = FindInPattern(pattern: subpattern.Pattern,
                                            semanticModel: semanticModel,
                                            filter: filter);
                    builder.AddRange(results);
                }
            }
        }
        else if (pattern is RelationalPatternSyntax relational)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: relational.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (pattern is SlicePatternSyntax slice)
        {
            ImmutableArray<TResult> results = FindInPattern(pattern: slice.Pattern,
                                                            semanticModel: semanticModel,
                                                            filter: filter);
            builder.AddRange(results);
        }
        else if (pattern is TypePatternSyntax type)
        {
            symbolInfo = semanticModel.GetSymbolInfo(type.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (pattern is UnaryPatternSyntax unary)
        {
            ImmutableArray<TResult> results = FindInPattern(pattern: unary.Pattern,
                                                            semanticModel: semanticModel,
                                                            filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }
}