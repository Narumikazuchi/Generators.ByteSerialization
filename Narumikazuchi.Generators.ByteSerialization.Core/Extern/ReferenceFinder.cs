using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.CodeAnalysis;

static public partial class ReferenceFinder
{
    static public ImmutableArray<TResult> FindInArguments<TResult>(IEnumerable<ArgumentSyntax> arguments,
                                                                   SemanticModel semanticModel,
                                                                   SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        foreach (ArgumentSyntax argument in arguments)
        {
            ImmutableArray<TResult> results = FindInArgument(argument: argument,
                                                             semanticModel: semanticModel,
                                                             filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }

    static public ImmutableArray<TResult> FindInArgument<TResult>(ArgumentSyntax argument,
                                                                  SemanticModel semanticModel,
                                                                  SyntaxFilter<TResult> filter)
    {
        return FindInExpression(expression: argument.Expression,
                                semanticModel: semanticModel,
                                filter: filter);
    }

    static public ImmutableArray<TResult> FindInAttributes<TResult>(IEnumerable<AttributeListSyntax> attributeLists,
                                                                    SemanticModel semanticModel,
                                                                    SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        foreach (AttributeListSyntax attributeList in attributeLists)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributes: attributeList.Attributes,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }
    static public ImmutableArray<TResult> FindInAttributes<TResult>(IEnumerable<AttributeSyntax> attributes,
                                                                    SemanticModel semanticModel,
                                                                    SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        foreach (AttributeSyntax attribute in attributes)
        {
            ImmutableArray<TResult> results = FindInAttribute(attribute: attribute,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }

    static public ImmutableArray<TResult> FindInAttribute<TResult>(AttributeSyntax attribute,
                                                                   SemanticModel semanticModel,
                                                                   SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        if (attribute.ArgumentList is not null)
        {
            foreach (AttributeArgumentSyntax argument in attribute.ArgumentList.Arguments)
            {
                ImmutableArray<TResult> result = FindInExpression(expression: argument.Expression,
                                                                  semanticModel: semanticModel,
                                                                  filter: filter);
                builder.AddRange(result);
            }
        }

        return builder.ToImmutable();
    }

    static public ImmutableArray<TResult> FindInExpressions<TResult>(IEnumerable<ExpressionSyntax> expressions,
                                                                     SemanticModel semanticModel,
                                                                     SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        foreach (ExpressionSyntax expression in expressions)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }

    static public ImmutableArray<TResult> FindInStatements<TResult>(IEnumerable<StatementSyntax> statements,
                                                                    SemanticModel semanticModel,
                                                                    SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        foreach (StatementSyntax statement in statements)
        {
            ImmutableArray<TResult> results = FindInStatement(statement: statement,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }

    static public ImmutableArray<TResult> FindInParameters<TResult>(IEnumerable<ParameterSyntax> parameters,
                                                                    SemanticModel semanticModel,
                                                                    SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        foreach (ParameterSyntax parameter in parameters)
        {
            ImmutableArray<TResult> results = FindInParameter(parameter: parameter,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(results);
        }

        return builder.ToImmutable();
    }

    static public ImmutableArray<TResult> FindInParameter<TResult>(ParameterSyntax parameter,
                                                                   SemanticModel semanticModel,
                                                                   SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(parameter);
        ImmutableArray<TResult> result = filter.Invoke(symbolInfo);
        builder.AddRange(result);

        foreach (AttributeListSyntax attributeList in parameter.AttributeLists)
        {
            result = FindInAttributes(attributes: attributeList.Attributes,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(result);
        }

        symbolInfo = semanticModel.GetSymbolInfo(parameter.Type);
        result = filter.Invoke(symbolInfo);
        builder.AddRange(result);

        if (parameter.Default is not null)
        {
            result = FindInExpression(expression: parameter.Default.Value,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(result);
        }

        return builder.ToImmutable();
    }
}