using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsing.Parsers;
    using static TO2ParserCommon;
    using static TO2ParserLiterals;

    public static class TO2ParserExpressions {
        public static readonly Parser<Expression> expression = new Parser<Expression>(expressionImpl);

        public static readonly Parser<bool> letOrConst = Alt(Tag("let").Map(_ => false), Tag("const").Map(_ => true));

        public static readonly Parser<IBlockItem> variableDeclaration = Seq(
            letOrConst, WhiteSpaces1.Then(Alt(
                declarationParameter.Map(item => (true, new List<DeclarationParameter> { item })),
                Delimited1(declarationParameterOrPlaceholder, commaDelimiter).Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => (false, items))
            )), WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(expression)
        ).Map((items, start, end) => {
            if (items.Item2.Item1) return new VariableDeclaration(items.Item2.Item2[0], items.Item1, items.Item3, start, end);
            return new TupleDeconstructDeclaration(items.Item2.Item2, items.Item1, items.Item3, start, end) as IBlockItem;
        });

        public static readonly Parser<Expression> returnExpression = Seq(
            Tag("return"), Opt(Spacing0.Then(expression))
        ).Map((items, start, end) => {
            if (items.Item2.IsDefined) return new ReturnValue(items.Item2.Value, start, end);
            return new ReturnEmpty(start, end) as Expression;
        });

        public static readonly Parser<Expression> whileExpression = Seq(
            Tag("while").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(expression), WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(expression)
        ).Map((items, start, end) => new While(items.Item1, items.Item2, start, end));

        public static readonly Parser<Expression> forInExpression = Seq(
            Tag("for").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(Alt(
                declarationParameter.Map(item => (true, new List<DeclarationParameter> { item })),
                Delimited1(declarationParameterOrPlaceholder, commaDelimiter).Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => (false, items))
            )),
            WhiteSpaces1.Then(Tag("in")).Then(WhiteSpaces1).Then(expression),
            WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(expression)
        ).Map((items, start, end) => {
            if (items.Item1.Item1) return new ForIn(items.Item1.Item2[0].target, items.Item1.Item2[0].type, items.Item2, items.Item3, start, end);
            return new ForInDeconstruct(items.Item1.Item2, items.Item2, items.Item3, start, end) as Expression;
        });

        public static readonly Parser<Expression> breakExpression = Tag("break").Map((_, start, end) => new Break(start, end));

        public static readonly Parser<Expression> continueExpression = Tag("continue").Map((_, start, end) => new Continue(start, end));

        public static readonly Parser<Expression> block = Char('{').Then(WhiteSpaces0).Then(DelimitedUntil(Alt<IBlockItem>(
            expression,
            lineComment,
            variableDeclaration,
            returnExpression,
            whileExpression,
            forInExpression,
            breakExpression,
            continueExpression
        ), WhiteSpaces1, Char('}'))).Map(expressions => new Block(expressions));

        public static readonly Parser<List<Expression>> callArguments = Char('(').Then(WhiteSpaces0).Then(DelimitedUntil(expression, commaDelimiter, WhiteSpaces0.Then(Char(')'))));

        public static readonly Parser<Expression> variableRefOrCall = Seq(
            identifier, Many0(Tag("::").Then(identifier)), Opt(Spacing0.Then(callArguments))
        ).Map((items, start, end) => {
            List<string> fullName = items.Item2;
            fullName.Insert(0, items.Item1);
            if (items.Item3.IsDefined) {
                return new Call(fullName, items.Item3.Value, start, end);
            }
            return new VariableGet(fullName, start, end) as Expression;
        });

        public static readonly Parser<Expression> tupleCreate = DelimitedN_M(2, null, expression, commaDelimiter, "<expression>").
            Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).
            Map((expressions, start, end) => new TupleCreate(expressions, start, end));

        public static readonly Parser<Expression> recordCreate = Seq(
            Opt(typeRef.Between(Char('<').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>')).Then(Spacing0))),
            Delimited1(
                Seq(identifier, Spacing0.Then(Char(':')).Then(Spacing0).Then(expression)),
                commaDelimiter, "<expression>").Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')))
        ).Map((items, start, end) => new RecordCreate(items.Item1.IsDefined ? items.Item1.Value : null, items.Item2, start, end));

        public static readonly Parser<Expression> arrayCreate = Seq(
            Opt(typeRef.Between(Char('<').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>')).Then(Spacing0))),
            Char('[').Then(WhiteSpaces0).Then(DelimitedUntil(expression, commaDelimiter, WhiteSpaces0.Then(Char(']'))))
        ).Map((items, start, end) => new ArrayCreate(items.Item1.IsDefined ? items.Item1.Value : null, items.Item2, start, end));

        public static readonly Parser<FunctionParameter> lambdaParameter = Seq(
            identifier, Opt(typeSpec)
        ).Map(param => new FunctionParameter(param.Item1, param.Item2.IsDefined ? param.Item2.Value : null));

        public static readonly Parser<List<FunctionParameter>> lambdaParameters = Char('(').Then(WhiteSpaces0).Then(DelimitedUntil(lambdaParameter, commaDelimiter, WhiteSpaces0.Then(Char(')'))));

        public static readonly Parser<Expression> lambda = Seq(
            Tag("fn").Then(Spacing0).Then(lambdaParameters), WhiteSpaces0.Then(Tag("->")).Then(WhiteSpaces0).Then(expression)
        ).Map((items, start, end) => new Lambda(items.Item1, items.Item2, start, end));

        public static readonly Parser<Expression> bracketTerm = expression.Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map((expression, start, end) => new Bracket(expression, start, end));

        public static readonly Parser<Expression> range_create = Seq(
            Alt(literalInt, bracketTerm, variableRefOrCall), Spacing0.Then(Tag("..")).Then(Opt(Char('.'))), Spacing0.Then(Alt(literalInt, bracketTerm, variableRefOrCall))
        ).Map((items, start, end) => new RangeCreate(items.Item1, items.Item3, items.Item2.IsDefined, start, end));

        public static readonly Parser<Expression> term = Alt<Expression>(
            literalBool,
            literalFloat,
            range_create,
            literalInt,
            literalString,
            bracketTerm,
            block,
            arrayCreate,
            tupleCreate,
            recordCreate,
            variableRefOrCall,
            lambda
        );

        public static readonly Parser<IndexSpec> indexSpec = expression.Map(expression => new IndexSpec(expression));

        private static readonly Parser<Operator> suffixOp = Char('?').Map(_ => Operator.Unwrap);

        public static Parser<ISuffixOperation> suffixOps = Alt(
            Seq(WhiteSpaces0.Then(Char('.')).Then(WhiteSpaces0).Then(identifier), Opt(Spacing0.Then(callArguments))).Map(item => {
                if (item.Item2.IsDefined) return new MethodCallSuffix(item.Item1, item.Item2.Value);
                return new FieldGetSuffix(item.Item1) as ISuffixOperation;
            }),
            Spacing0.Then(indexSpec.Between(Char('[').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(']')))).Map(indexSpec => new IndexGetSuffix(indexSpec) as ISuffixOperation),
            Spacing0.Then(suffixOp).Map(op => new OperatorSuffix(op) as ISuffixOperation)
        );

        public static readonly Parser<Expression> termWithSuffixOps = term.Fold0(suffixOps, (target, suffixOp, start, end) => {
            switch (suffixOp) {
            case IndexGetSuffix indexGet: return new IndexGet(target, indexGet.indexSpec, start, end);
            case MethodCallSuffix methodCall: return new MethodCall(target, methodCall.methodName, methodCall.arguments, start, end);
            case FieldGetSuffix fieldGet: return new FieldGet(target, fieldGet.fieldName, start, end);
            case OperatorSuffix operatorSuffix: return new UnarySuffix(target, operatorSuffix.op, start, end);
            default: throw new ParseException(start, new string[] { "<valid suffix>" });
            }
        });

        public static readonly Parser<Operator> unaryPrefixOp = Alt(
            Char('-').Map(_ => Operator.Neg),
            Char('!').Map(_ => Operator.Not),
            Char('~').Map(_ => Operator.BitNot)
        );

        public static readonly Parser<Expression> unaryPrefixExpr = Alt(
            Seq(unaryPrefixOp, WhiteSpaces0.Then(termWithSuffixOps)).Map((items, start, end) => new UnaryPrefix(items.Item1, items.Item2, start, end)),
            termWithSuffixOps
        );

        public static readonly Parser<Operator> mulDivBinaryOp = Alt(
            Char('*').Map(_ => Operator.Mul),
            Char('/').Map(_ => Operator.Div),
            Char('%').Map(_ => Operator.Mod)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        public static readonly Parser<Expression> mulDivBinaryExpr = Chain(unaryPrefixExpr, mulDivBinaryOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

        public static readonly Parser<Operator> addSubBinaryOp = Alt(
            Char('+').Map(_ => Operator.Add),
            Char('-').Map(_ => Operator.Sub)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        public static readonly Parser<Expression> addSubBinaryExpr = Chain(mulDivBinaryExpr, addSubBinaryOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

        public static readonly Parser<Operator> bitOp = Alt(
            Char('&').Map(_ => Operator.BitAnd),
            Char('|').Map(_ => Operator.BitOr),
            Char('^').Map(_ => Operator.BitXor)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        public static readonly Parser<Expression> bitBinaryExpr = Chain(addSubBinaryExpr, bitOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

        public static readonly Parser<Operator> compareOp = Alt(
            Tag("==").Map(_ => Operator.Eq),
            Tag("!=").Map(_ => Operator.NotEq),
            Tag("<=").Map(_ => Operator.Le),
            Tag(">=").Map(_ => Operator.Ge),
            Char('<').Map(_ => Operator.Lt),
            Char('>').Map(_ => Operator.Gt)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        public static readonly Parser<Expression> compareExpr = Chain(bitBinaryExpr, compareOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

        public static readonly Parser<Operator> booleanOp = Alt(
            Tag("&&").Map(_ => Operator.BoolAnd),
            Tag("||").Map(_ => Operator.BoolOr)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        public static readonly Parser<Expression> booleanExpr = Chain(compareExpr, booleanOp, (left, op, right, start, end) => new BinaryBool(left, op, right, start, end));

        public static readonly Parser<Expression> ifBody = Alt(expression, returnExpression, breakExpression, continueExpression);

        public static readonly Parser<Expression> ifExpr = Seq(
            Tag("if").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(booleanExpr),
            WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(Alt(ifBody)),
            Opt(WhiteSpaces1.Then(Tag("else")).Then(WhiteSpaces1).Then(ifBody))
        ).Map((items, start, end) => {
            if (items.Item3.IsDefined)
                return new IfThenElse(items.Item1, items.Item2, items.Item3.Value, start, end);
            return new IfThen(items.Item1, items.Item2, start, end) as Expression;
        });

        public static readonly Parser<Operator> assignOp = Alt(
            Tag("=").Map(_ => Operator.Assign),
            Tag("+=").Map(_ => Operator.AddAssign),
            Tag("-=").Map(_ => Operator.SubAssign),
            Tag("*=").Map(_ => Operator.MulAssign),
            Tag("/=").Map(_ => Operator.DivAssign),
            Tag("|=").Map(_ => Operator.BitOrAssign),
            Tag("&=").Map(_ => Operator.BitAndAssign),
            Tag("^=").Map(_ => Operator.BitXorAssign)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        public static readonly Parser<Expression> variableAssignment = Seq(
            identifier, assignOp, Alt(booleanExpr, ifExpr)
        ).Map((items, start, end) => new VariableAssign(items.Item1, items.Item2, items.Item3, start, end));

        public static readonly Parser<List<(string source, string target)>> sourceTargetList = Delimited1(Alt(
            Seq(identifier, Spacing0.Then(Char('@')).Then(Spacing0).Then(identifier)),
            Char('_').Map(_ => ("", "")),
            identifier.Map(id => (id, id))
        ), commaDelimiter).Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')));

        public static readonly Parser<Expression> tupleDeconstructAssignment = Seq(
            sourceTargetList, WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(Alt(booleanExpr, ifExpr))
        ).Map((items, start, end) => new TupleDeconstructAssign(items.Item1, items.Item2, start, end));

        private static readonly Parser<Expression> topLevelExpression = Alt(
            tupleDeconstructAssignment,
            variableAssignment,
            ifExpr,
            booleanExpr
        );

        private static IResult<Expression> expressionImpl(IInput input) => topLevelExpression(input);
    }
}
