using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Narumikazuchi.CodeAnalysis;

public partial class ReferenceFinder
{
    static public ImmutableArray<TResult> FindInStatement<TResult>(StatementSyntax statement,
                                                                   SemanticModel semanticModel,
                                                                   SyntaxFilter<TResult> filter)
    {
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>();
        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(statement);
        if (symbolInfo.Symbol is not null ||
            symbolInfo.CandidateSymbols.Length > 0)
        {
            ImmutableArray<TResult> filtered = filter.Invoke(symbolInfo);
            builder.AddRange(filtered);
        }

        if (statement is BlockSyntax block)
        {
            ImmutableArray<TResult> results = FindInStatements(statements: block.Statements,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (statement is BreakStatementSyntax @break)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @break.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (statement is CheckedStatementSyntax @checked)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @checked.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: @checked.Block,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is CommonForEachStatementSyntax commonForEach)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: commonForEach.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: commonForEach.Expression,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: commonForEach.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is ContinueStatementSyntax @continue)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @continue.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (statement is DoStatementSyntax @do)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @do.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: @do.Condition,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: @do.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is EmptyStatementSyntax empty)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: empty.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (statement is ExpressionStatementSyntax expression)
        {
            ImmutableArray<TResult> results = FindInExpression(expression: expression.Expression,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
        }
        else if (statement is FixedStatementSyntax @fixed)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @fixed.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            symbolInfo = semanticModel.GetSymbolInfo(@fixed.Declaration.Type);
            results = filter.Invoke(symbolInfo);
            builder.AddRange(results);

            foreach (VariableDeclaratorSyntax variable in @fixed.Declaration.Variables)
            {
                if (variable.ArgumentList is not null)
                {
                    results = FindInArguments(arguments: variable.ArgumentList.Arguments,
                                              semanticModel: semanticModel,
                                              filter: filter);
                    builder.AddRange(results);
                }

                if (variable.Initializer is not null)
                {
                    results = FindInExpression(expression: variable.Initializer.Value,
                                               semanticModel: semanticModel,
                                               filter: filter);
                    builder.AddRange(results);
                }
            }

            results = FindInStatement(statement: @fixed.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is ForEachStatementSyntax @foreach)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @foreach.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            symbolInfo = semanticModel.GetSymbolInfo(@foreach.Type);
            results = filter.Invoke(symbolInfo);
            builder.AddRange(results);

            results = FindInExpression(expression: @foreach.Expression,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: @foreach.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is ForEachVariableStatementSyntax foreachVariable)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: foreachVariable.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
            
            results = FindInExpression(expression: foreachVariable.Variable,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: foreachVariable.Expression,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: foreachVariable.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is ForStatementSyntax @for)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @for.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpressions(expressions: @for.Incrementors,
                                        semanticModel: semanticModel,
                                        filter: filter);
            builder.AddRange(results);

            results = FindInExpressions(expressions: @for.Initializers,
                                        semanticModel: semanticModel,
                                        filter: filter);
            builder.AddRange(results);

            if (@for.Declaration is not null)
            {
                foreach (VariableDeclaratorSyntax declarator in @for.Declaration.Variables)
                {
                    if (declarator.Initializer is null)
                    {
                        continue;
                    }

                    results = FindInExpression(expression: declarator.Initializer.Value,
                                               semanticModel: semanticModel,
                                               filter: filter);
                    builder.AddRange(results);
                }
            }

            if (@for.Condition is not null)
            {
                results = FindInExpression(expression: @for.Condition,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (statement is GotoStatementSyntax @goto)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @goto.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            if (@goto.Expression is not null)
            {
                results = FindInExpression(expression: @goto.Expression,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (statement is IfStatementSyntax @if)
        {
            ImmutableArray<TResult> results = FindInStatement(statement: @if.Statement,
                                                              semanticModel: semanticModel,
                                                              filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: @if.Condition,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);
        }
        else if (statement is LabeledStatementSyntax label)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: label.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: label.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is LocalDeclarationStatementSyntax declaration)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: declaration.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            foreach (VariableDeclaratorSyntax variable in declaration.Declaration.Variables)
            {
                if (variable.Initializer is null)
                {
                    continue;
                }

                results = FindInExpression(expression: variable.Initializer.Value,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (statement is LocalFunctionStatementSyntax localFunction)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: localFunction.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
            
            if (localFunction.Body is not null)
            {
                results = FindInStatement(statement: localFunction.Body,
                                          semanticModel: semanticModel,
                                          filter: filter);
                builder.AddRange(results);
            }
            else
            {
                results = FindInExpression(expression: localFunction.ExpressionBody.Expression,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }

            foreach (ParameterSyntax parameter in localFunction.ParameterList.Parameters)
            {
                results = FindInAttributes(attributeLists: parameter.AttributeLists,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);

                if (parameter.Default is not null)
                {
                    results = FindInExpression(expression: parameter.Default.Value,
                                               semanticModel: semanticModel,
                                               filter: filter);
                    builder.AddRange(results);
                }

                if (parameter.Type is not null)
                {
                    symbolInfo = semanticModel.GetSymbolInfo(parameter.Type);
                    results = filter.Invoke(symbolInfo);
                    builder.AddRange(results);
                }
            }
        }
        else if (statement is LockStatementSyntax @lock)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @lock.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: @lock.Expression,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: @lock.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is ReturnStatementSyntax @return)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @return.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            if (@return.Expression is not null)
            {
                results = FindInExpression(expression: @return.Expression,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (statement is SwitchStatementSyntax @switch)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @switch.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            foreach (SwitchSectionSyntax switchSection in @switch.Sections)
            {
                results = FindInStatements(statements: switchSection.Statements,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (statement is ThrowStatementSyntax @throw)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @throw.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            if (@throw.Expression is not null)
            {
                results = FindInExpression(expression: @throw.Expression,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }
        else if (statement is TryStatementSyntax @try)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @try.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: @try.Block,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);

            if (@try.Finally is not null)
            {
                results = FindInStatement(statement: @try.Finally.Block,
                                          semanticModel: semanticModel,
                                          filter: filter);
                builder.AddRange(results);
            }

            foreach (CatchClauseSyntax catchClause in @try.Catches)
            {
                if (catchClause.Declaration is not null)
                {
                    symbolInfo = semanticModel.GetSymbolInfo(catchClause.Declaration.Type);
                    results = filter.Invoke(symbolInfo);
                    builder.AddRange(results);
                }

                if (catchClause.Filter is not null)
                {
                    results = FindInExpression(expression: catchClause.Filter.FilterExpression,
                                               semanticModel: semanticModel,
                                               filter: filter);
                    builder.AddRange(results);
                }
            }

            return builder.ToImmutable();
        }
        else if (statement is UnsafeStatementSyntax @unsafe)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @unsafe.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: @unsafe.Block,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is UsingStatementSyntax @using)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @using.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);
            
            if (@using.Declaration is not null)
            {
                symbolInfo = semanticModel.GetSymbolInfo(@using.Declaration.Type);
                results = filter.Invoke(symbolInfo);
                builder.AddRange(results);
            }

            results = FindInStatement(statement: @using.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is WhileStatementSyntax @while)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @while.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            results = FindInExpression(expression: @while.Condition,
                                       semanticModel: semanticModel,
                                       filter: filter);
            builder.AddRange(results);

            results = FindInStatement(statement: @while.Statement,
                                      semanticModel: semanticModel,
                                      filter: filter);
            builder.AddRange(results);
        }
        else if (statement is YieldStatementSyntax @yield)
        {
            ImmutableArray<TResult> results = FindInAttributes(attributeLists: @yield.AttributeLists,
                                                               semanticModel: semanticModel,
                                                               filter: filter);
            builder.AddRange(results);

            if (@yield.Expression is not null)
            {
                results = FindInExpression(expression: @yield.Expression,
                                           semanticModel: semanticModel,
                                           filter: filter);
                builder.AddRange(results);
            }
        }

        return builder.ToImmutable();
    }
}
