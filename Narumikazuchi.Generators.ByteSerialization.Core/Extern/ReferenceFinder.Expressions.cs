using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.CodeAnalysis;

public partial class ReferenceFinder
{
    static public ImmutableArray<TResult> FindInExpression<TResult>(ExpressionSyntax expression,
                                                                    SemanticModel semanticModel,
                                                                    SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(expression);
        if (symbolInfo.Symbol is not null ||
            symbolInfo.CandidateSymbols.Length > 0)
        {
            ImmutableArray<TResult> filtered = filter.Invoke(symbolInfo);
            builder.AddRange(filtered);
        }

        if (expression is AnonymousFunctionExpressionSyntax anonymousFunction)
        {
            if (anonymousFunction.Block is not null)
            {
                ImmutableArray<TResult> results = FindInStatement(statement: anonymousFunction.Block,
                                                                  semanticModel: semanticModel,
                                                                  filter: filter);
                builder.AddRange(results);
            }
            else
            {
                ImmutableArray<TResult> results = FindInExpression(expression: anonymousFunction.ExpressionBody,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is AnonymousMethodExpressionSyntax anonymousMethod)
        {
            if (anonymousMethod.Block is not null)
            {
                ImmutableArray<TResult> results = FindInStatement(statement: anonymousMethod.Block,
                                                                  semanticModel: semanticModel,
                                                                  filter: filter);
                builder.AddRange(results);
            }
            else
            {
                ImmutableArray<TResult> results = FindInExpression(expression: anonymousMethod.ExpressionBody,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }

            if (anonymousMethod.ParameterList is not null)
            {
                ImmutableArray<TResult> results = FindInParameters(parameters: anonymousMethod.ParameterList.Parameters,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is AnonymousObjectCreationExpressionSyntax anonymousObjectCreation)
        {
            ImmutableArray<TResult> result = FindInExpressions(expressions: anonymousObjectCreation.Initializers.Select(declarator => declarator.Expression),
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(result);
        }
        else if (expression is ArrayCreationExpressionSyntax arrayCreation)
        {
            symbolInfo = semanticModel.GetSymbolInfo(arrayCreation.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);

            if (arrayCreation.Initializer is not null)
            {
                results = FindInExpressions(expressions: arrayCreation.Initializer.Expressions,
                                            semanticModel: semanticModel,
                                            filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is ArrayTypeSyntax arrayType)
        {
            symbolInfo = semanticModel.GetSymbolInfo(arrayType.ElementType);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is AssignmentExpressionSyntax assignment)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: assignment.Left,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: assignment.Right,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);
        }
        else if (expression is AwaitExpressionSyntax @await)
        {
            ImmutableArray<TResult> result = FindInExpression(expression: @await.Expression,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(result);
        }
        else if (expression is BaseObjectCreationExpressionSyntax baseObjectCreation)
        {
            if (baseObjectCreation.Initializer is not null)
            {
                ImmutableArray<TResult> results = FindInExpression(expression: baseObjectCreation.Initializer,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }

            if (baseObjectCreation.ArgumentList is not null)
            {
                ImmutableArray<TResult> results = FindInArguments(arguments: baseObjectCreation.ArgumentList.Arguments,
                                                                  semanticModel: semanticModel,
                                                                  filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is BinaryExpressionSyntax binary)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: binary.Left,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: binary.Right,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);
        }
        else if (expression is CastExpressionSyntax cast)
        {
            symbolInfo = semanticModel.GetSymbolInfo(cast.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);

            results = FindInExpression(expression: cast.Expression,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);
        }
        else if (expression is CheckedExpressionSyntax @checked)
        {
            ImmutableArray<TResult> result = FindInExpression(expression: @checked.Expression,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(result);
        }
        else if (expression is ConditionalAccessExpressionSyntax conditionalAccess)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: conditionalAccess.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: conditionalAccess.WhenNotNull,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);
        }
        else if (expression is ConditionalExpressionSyntax conditional)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: conditional.Condition,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: conditional.WhenFalse,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: conditional.WhenTrue,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);
        }
        else if (expression is DeclarationExpressionSyntax declaration)
        {
            symbolInfo = semanticModel.GetSymbolInfo(declaration.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is DefaultExpressionSyntax @default)
        {
            symbolInfo = semanticModel.GetSymbolInfo(@default.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is ElementAccessExpressionSyntax elementAccess)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: elementAccess.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInArguments(arguments: elementAccess.ArgumentList.Arguments,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (expression is ElementBindingExpressionSyntax elementBinding)
        {
            ImmutableArray<TResult> result = FindInArguments(arguments: elementBinding.ArgumentList.Arguments,
                                                             semanticModel: semanticModel,
                                                             filter: filter);
            builder.AddRange(result);
        }
        else if (expression is FunctionPointerTypeSyntax functionPointer)
        {
            foreach (FunctionPointerParameterSyntax parameter in functionPointer.ParameterList.Parameters)
            {
                symbolInfo = semanticModel.GetSymbolInfo(parameter.Type);
                ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
                builder.AddRange(results);
            }
        }
        else if (expression is ImplicitArrayCreationExpressionSyntax implicitArrayCreation)
        {
            ImmutableArray<TResult> result = FindInExpression(expression: implicitArrayCreation.Initializer,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(result);
        }
        else if (expression is ImplicitElementAccessSyntax implicitElementAccess)
        {
            ImmutableArray<TResult> result = FindInArguments(arguments: implicitElementAccess.ArgumentList.Arguments,
                                                             semanticModel: semanticModel,
                                                             filter: filter);
            builder.AddRange(result);
        }
        else if (expression is ImplicitObjectCreationExpressionSyntax implicitObjectCreation)
        {
            if (implicitObjectCreation.Initializer is not null)
            {
                ImmutableArray<TResult> results = FindInExpression(expression: implicitObjectCreation.Initializer,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);

                results = FindInArguments(arguments: implicitObjectCreation.ArgumentList.Arguments,
                                          semanticModel: semanticModel,
                                          filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is ImplicitStackAllocArrayCreationExpressionSyntax implicitStackAllocArrayCreation)
        {
            ImmutableArray<TResult> result = FindInExpression(expression: implicitStackAllocArrayCreation.Initializer,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(result);
        }
        else if (expression is InitializerExpressionSyntax initializer)
        {
            ImmutableArray<TResult> result = FindInExpressions(expressions: initializer.Expressions,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(result);
        }
        else if (expression is InvocationExpressionSyntax invocation)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: invocation.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInArguments(arguments: invocation.ArgumentList.Arguments,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (expression is IsPatternExpressionSyntax isPattern)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: isPattern.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInPattern(pattern: isPattern.Pattern,
                                    semanticModel: semanticModel,
                                    filter: filter);
            builder.AddRange(results);
        }
        else if (expression is LambdaExpressionSyntax lambda)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: lambda.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            if (lambda.Block is not null)
            {
                results = FindInStatement(statement: lambda.Block,
                                          semanticModel: semanticModel,
                                          filter: filter);
                builder.AddRange(results);
            }
            else
            {
                results = FindInExpression(expression: lambda.ExpressionBody,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);

            }
        }
        else if (expression is MakeRefExpressionSyntax makeRef)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: makeRef.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is MemberAccessExpressionSyntax memberAccess)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: memberAccess.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is NullableTypeSyntax nullable)
        {
            symbolInfo = semanticModel.GetSymbolInfo(nullable.ElementType);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is ObjectCreationExpressionSyntax objectCreation)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: objectCreation.Initializer,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
            results = FindInArguments(arguments: objectCreation.ArgumentList.Arguments,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);

            symbolInfo = semanticModel.GetSymbolInfo(objectCreation.Type);
            results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: parenthesized.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: parenthesizedLambda.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInParameters(parameters: parenthesizedLambda.ParameterList.Parameters,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            if (parenthesizedLambda.ReturnType is not null)
            {
                symbolInfo = semanticModel.GetSymbolInfo(parenthesizedLambda.ReturnType);
                results = filter.Invoke(symbolInfo);
                builder.AddRange(results);
            }

            if (parenthesizedLambda.Block is not null)
            {
                results = FindInStatement(statement: parenthesizedLambda.Block,
                                          semanticModel: semanticModel,
                                          filter: filter);
                builder.AddRange(results);
            }
            else
            {
                results = FindInExpression(expression: parenthesizedLambda.ExpressionBody,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is PostfixUnaryExpressionSyntax postfixUnary)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: postfixUnary.Operand,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is PointerTypeSyntax pointerType)
        {
            symbolInfo = semanticModel.GetSymbolInfo(pointerType.ElementType);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is PrefixUnaryExpressionSyntax prefixUnary)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: prefixUnary.Operand,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is RangeExpressionSyntax range)
        {
            if (range.LeftOperand is not null)
            {
                ImmutableArray<TResult> results = FindInExpression(expression: range.LeftOperand,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }

            if (range.RightOperand is not null)
            {
                ImmutableArray<TResult> results = FindInExpression(expression: range.RightOperand,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is RefExpressionSyntax @ref)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: @ref.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is RefTypeExpressionSyntax Expression)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: Expression.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is RefTypeSyntax refType)
        {
            symbolInfo = semanticModel.GetSymbolInfo(refType.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is RefValueExpressionSyntax refValue)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: refValue.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            symbolInfo = semanticModel.GetSymbolInfo(refValue.Type);
            results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is SimpleLambdaExpressionSyntax simpleLambda)
        {
            ImmutableArray<TResult> results = FindInParameter(parameter: simpleLambda.Parameter,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(results);

            results = FindInAttributes(attributeLists: simpleLambda.AttributeLists,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            if (simpleLambda.Block is not null)
            {
                results = FindInStatement(statement: simpleLambda.Block,
                                          semanticModel: semanticModel,
                                          filter: filter);
                builder.AddRange(results);
            }
            else
            {
                results = FindInExpression(expression: simpleLambda.ExpressionBody,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is SizeOfExpressionSyntax @sizeof)
        {
            symbolInfo = semanticModel.GetSymbolInfo(@sizeof.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is StackAllocArrayCreationExpressionSyntax stackAllocArrayCreation)
        {
            symbolInfo = semanticModel.GetSymbolInfo(stackAllocArrayCreation.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);

            if (stackAllocArrayCreation.Initializer is not null)
            {
                results = FindInExpression(expression: stackAllocArrayCreation.Initializer,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (expression is SwitchExpressionSyntax @switch)
        {
            ImmutableArray<TResult> results = FindInExpressions(expressions: @switch.Arms.Select(arm => arm.Expression),
                                                                semanticModel: semanticModel,
                                                                filter: filter);
            builder.AddRange(results);
        }
        else if (expression is ThrowExpressionSyntax @throw)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: @throw.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (expression is TupleExpressionSyntax tuple)
        {
            ImmutableArray<TResult> results = FindInArguments(arguments: tuple.Arguments,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(results);
        }
        else if (expression is TupleTypeSyntax tupleType)
        {
            foreach (TupleElementSyntax element in tupleType.Elements)
            {
                symbolInfo = semanticModel.GetSymbolInfo(element.Type);
                ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
                builder.AddRange(results);
            }
        }
        else if (expression is TypeOfExpressionSyntax @typeof)
        {
            symbolInfo = semanticModel.GetSymbolInfo(@typeof.Type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is TypeSyntax type)
        {
            symbolInfo = semanticModel.GetSymbolInfo(type);
            ImmutableArray<TResult> results = filter.Invoke(symbolInfo);
            builder.AddRange(results);
        }
        else if (expression is WithExpressionSyntax with)
        {
            if (with.Expression is not null)
            {
                ImmutableArray<TResult> results = FindInExpression(expression: with.Expression,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }

            if (with.Initializer is not null)
            {
                ImmutableArray<TResult> results = FindInExpression(expression: with.Initializer,
                                                                   semanticModel: semanticModel,
                                                                   filter: filter);
                builder.AddRange(results);
            }
        }

        return builder.ToImmutable();
    }
}