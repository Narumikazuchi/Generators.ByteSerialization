namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public partial class SerializableGenerator
{
    public const String NAMESPACE = "Narumikazuchi.Generators.ByteSerialization";
    public const String BYTESERIALIZER = NAMESPACE + ".ByteSerializer";

    static private Boolean IsEligableTypeSyntax(SyntaxNode syntaxNode,
                                                CancellationToken cancellationToken = default)
    {
        return syntaxNode is MethodDeclarationSyntax;
    }

    static private ImmutableArray<ITypeSymbol> TransformToType(GeneratorSyntaxContext context,
                                                               CancellationToken cancellationToken = default)
    {
        if (s_CachedSymbols.Count is 0)
        {
            IAssemblySymbol generatorAssembly = context.SemanticModel.Compilation.References.Select(context.SemanticModel.Compilation.GetAssemblyOrModuleSymbol)
                                                                                            .OfType<IAssemblySymbol>()
                                                                                            .First(a => a.Name is NAMESPACE);

            s_CachedSymbols.Add(key: CachedType.Serializer,
                                value: generatorAssembly.GetTypeByMetadataName(BYTESERIALIZER));

            s_CachedSymbols.Add(key: CachedType.System_Byte,
                                value: context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Byte));

            s_CachedSymbols.Add(key: CachedType.System_Int32,
                                value: context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Int32));

            s_CachedSymbols.Add(key: CachedType.System_ArrayOf_Byte,
                                value: context.SemanticModel.Compilation.CreateArrayTypeSymbol(s_CachedSymbols[CachedType.System_Byte]));

            s_CachedSymbols.Add(key: CachedType.System_SpanOf_Byte,
                                value: context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(Span<>).FullName)
                                                                        .Construct(s_CachedSymbols[CachedType.System_Byte]));

            s_CachedSymbols.Add(key: CachedType.System_ReadOnlySpanOf_Byte,
                                value: context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(ReadOnlySpan<>).FullName)
                                                                        .Construct(s_CachedSymbols[CachedType.System_Byte]));
        }

        if (s_IgnoredTypeSymbols.Length is 0)
        {
            ImmutableArray<INamedTypeSymbol>.Builder builder = ImmutableArray.CreateBuilder<INamedTypeSymbol>(21);
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Boolean));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Byte));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Char));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_DateTime));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Decimal));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Delegate));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Double));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Enum));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Int16));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Int32));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Int64));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_IntPtr));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_MulticastDelegate));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Object));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_SByte));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Single));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_String));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_UInt16));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_UInt32));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_UInt64));
            builder.Add(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_UIntPtr));
            s_IgnoredTypeSymbols = builder.ToImmutable();
        }

        if (s_MethodSymbols.Length is 0)
        {
            s_MethodSymbols = s_CachedSymbols[CachedType.Serializer].GetMembers()
                                                                    .OfType<IMethodSymbol>()
                                                                    .Where(method => method.Name is "Deserialize"
                                                                                                 or "GetExpectedSerializedSize"
                                                                                                 or "Serialize")
                                                                    .ToImmutableArray();
        }

        MethodDeclarationSyntax methodSyntax = (MethodDeclarationSyntax)context.Node;

        HashSet<ITypeSymbol> eligableTypes = new(SymbolEqualityComparer.Default);
        if (methodSyntax.Body is not null)
        {
            ExtractUsedTypes(context: context,
                             builder: eligableTypes,
                             statements: methodSyntax.Body.Statements);
        }
        else if (methodSyntax.ExpressionBody is not null)
        {
            if (methodSyntax.ExpressionBody.Expression is InvocationExpressionSyntax invocation)
            {
                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
                if (symbolInfo.Symbol is IMethodSymbol method)
                {
                    AddTypeIfSuitable(method: method,
                                      builder: eligableTypes);
                }
                else
                {
                    foreach (IMethodSymbol candidate in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>())
                    {
                        AddTypeIfSuitable(method: candidate,
                                          builder: eligableTypes);
                    }
                }
            }
        }

        HashSet<ITypeSymbol> dependencies = new(SymbolEqualityComparer.Default);
        foreach (INamedTypeSymbol type in eligableTypes.OfType<INamedTypeSymbol>())
        {
            AddDependendTypes(type: type,
                              builder: dependencies);
        }

        foreach (ITypeSymbol type in dependencies)
        {
            eligableTypes.Add(type);
        }

        ImmutableArray<ITypeSymbol> trimmed = eligableTypes.Except(s_IgnoredTypeSymbols)
                                                           .Except(__Shared.IntrinsicTypes.Select(context.SemanticModel.Compilation.GetTypeByMetadataName))
                                                           .Where(type => type.BaseType is not null)
                                                           .Where(type => type.IsEnumerableSerializable() ||
                                                                          !type.CanBeSerialized())
                                                           .ToImmutableArray();

        return trimmed;
    }

    static private void ExtractUsedTypes(GeneratorSyntaxContext context,
                                         HashSet<ITypeSymbol> builder,
                                         SyntaxList<StatementSyntax> statements)
    {
        foreach (StatementSyntax statement in statements.Where(s => s is ExpressionStatementSyntax
                                                                      or LocalDeclarationStatementSyntax
                                                                      or SwitchStatementSyntax))
        {
            if (statement is ExpressionStatementSyntax expressionSyntax)
            {
                ExtractFromExpression(context: context,
                                      builder: builder,
                                      expressionSyntax: expressionSyntax);
            }
            else if (statement is LocalDeclarationStatementSyntax declarationSyntax)
            {
                ExtractFromDeclaration(context: context,
                                       builder: builder,
                                       declarationSyntax: declarationSyntax);
            }
            else if (statement is SwitchStatementSyntax switchSyntax)
            {
                foreach (SwitchSectionSyntax section in switchSyntax.Sections)
                {
                    ExtractUsedTypes(context: context,
                                     builder: builder,
                                     statements: section.Statements);
                }
            }
        }
    }

    static private void ExtractFromExpression(GeneratorSyntaxContext context,
                                              HashSet<ITypeSymbol> builder,
                                              ExpressionStatementSyntax expressionSyntax)
    {
        if (expressionSyntax.Expression is InvocationExpressionSyntax invocation)
        {
            SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            if (symbolInfo.Symbol is IMethodSymbol method)
            {
                AddTypeIfSuitable(method: method,
                                  builder: builder);
            }
            else
            {
                foreach (IMethodSymbol candidate in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>())
                {
                    AddTypeIfSuitable(method: candidate,
                                      builder: builder);
                }
            }
        }
        else if (expressionSyntax.Expression is AssignmentExpressionSyntax assignment)
        {
            ISymbol type = context.SemanticModel.GetSymbolInfo(assignment.Left).Symbol;
            if (SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_Int32]) ||
                SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_ArrayOf_Byte]) ||
                SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_SpanOf_Byte]) ||
                SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_ReadOnlySpanOf_Byte]))
            {
                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(assignment.Right);
                if (symbolInfo.Symbol is IMethodSymbol method)
                {
                    AddTypeIfSuitable(method: method,
                                      builder: builder);
                }
                else
                {
                    foreach (IMethodSymbol candidate in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>())
                    {
                        AddTypeIfSuitable(method: candidate,
                                          builder: builder);
                    }
                }
            }
        }
    }

    static private void ExtractFromDeclaration(GeneratorSyntaxContext context,
                                               HashSet<ITypeSymbol> builder,
                                               LocalDeclarationStatementSyntax declarationSyntax)
    {
        TypeSyntax typeSyntax = declarationSyntax.Declaration.Type;
        ISymbol type = context.SemanticModel.GetSymbolInfo(typeSyntax).Symbol;
        if (!SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_Int32]) &&
            !SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_ArrayOf_Byte]) &&
            !SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_SpanOf_Byte]) &&
            !SymbolEqualityComparer.Default.Equals(type, s_CachedSymbols[CachedType.System_ReadOnlySpanOf_Byte]))
        {
            return;
        }

        foreach (VariableDeclaratorSyntax variable in declarationSyntax.Declaration.Variables)
        {
            if (variable.Initializer is null)
            {
                continue;
            }

            if (variable.Initializer.Value is InvocationExpressionSyntax invocation)
            {
                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
                if (symbolInfo.Symbol is IMethodSymbol method)
                {
                    AddTypeIfSuitable(method: method,
                                      builder: builder);
                }
                else
                {
                    foreach (IMethodSymbol candidate in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>())
                    {
                        AddTypeIfSuitable(method: candidate,
                                          builder: builder);
                    }
                }
            }
            else if (variable.Initializer.Value is SwitchExpressionSyntax switchExpression)
            {
                foreach (SwitchExpressionArmSyntax arm in switchExpression.Arms)
                {
                    if (arm.Expression is InvocationExpressionSyntax armInvocation)
                    {
                        SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(armInvocation);
                        if (symbolInfo.Symbol is IMethodSymbol method)
                        {
                            AddTypeIfSuitable(method: method,
                                              builder: builder);
                        }
                        else
                        {
                            foreach (IMethodSymbol candidate in symbolInfo.CandidateSymbols.OfType<IMethodSymbol>())
                            {
                                AddTypeIfSuitable(method: candidate,
                                                  builder: builder);
                            }
                        }
                    }
                }
            }
        }
    }

    static private void AddTypeIfSuitable(IMethodSymbol method,
                                          HashSet<ITypeSymbol> builder)
    {
        if (!method.IsGenericMethod ||
            !SymbolEqualityComparer.Default.Equals(s_CachedSymbols[CachedType.Serializer], method.ContainingType))
        {
            return;
        }

        IMethodSymbol genericMethod = method.ConstructedFrom;
        foreach (IMethodSymbol serializerMethod in s_MethodSymbols)
        {
            if (SymbolEqualityComparer.Default.Equals(genericMethod, serializerMethod))
            {
                ITypeSymbol candidate = method.TypeArguments.Last();
                builder.Add(candidate);
            }
        }
    }

    static private void AddDependendTypes(INamedTypeSymbol type,
                                          HashSet<ITypeSymbol> builder)
    {
        ImmutableArray<IFieldSymbol> fields = __Shared.GetFieldsToSerialize(type);
        foreach (IFieldSymbol field in fields)
        {
            if (field.Type is INamedTypeSymbol fieldType &&
                fieldType.SpecialType is SpecialType.None
                                      or SpecialType.System_DateTime
                                      or SpecialType.System_Enum
                                      or SpecialType.System_Nullable_T
                                      or SpecialType.System_String)
            {
                builder.Add(fieldType);
                AddDependendTypes(type: fieldType,
                                  builder: builder);
            }
            else if (field.Type is IArrayTypeSymbol array &&
                     array.ElementType is INamedTypeSymbol element)
            {
                builder.Add(array);
                builder.Add(element);
            }
        }

        if (type.IsValueType ||
            type.IsSealed)
        {
            return;
        }

        foreach (INamedTypeSymbol derived in type.GetDerivedTypes())
        {
            builder.Add(derived);
            AddDependendTypes(type: derived,
                              builder: builder);
        }
    }

    static private readonly Dictionary<CachedType, ITypeSymbol> s_CachedSymbols = new();
    static private ImmutableArray<INamedTypeSymbol> s_IgnoredTypeSymbols = ImmutableArray<INamedTypeSymbol>.Empty;
    static private ImmutableArray<IMethodSymbol> s_MethodSymbols = ImmutableArray<IMethodSymbol>.Empty;
}