using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    using static Parsing.Parsers;
    using static TO2ParserCommon;
    using static TO2ParserLiterals;

    public static class TO2ParserExpressions {
        public static readonly Parser<Expression> Expression = new Parser<Expression>(expressionImpl);

        private static readonly Parser<bool> LetOrConst = Alt(Tag("let").Map(_ => false), Tag("const").Map(_ => true));

        private static readonly Parser<IBlockItem> VariableDeclaration = Seq(
            LetOrConst, WhiteSpaces1.Then(Alt(
                DeclarationParameter.Map(item => (true, new List<DeclarationParameter> {item})),
                Delimited1(DeclarationParameterOrPlaceholder, CommaDelimiter)
                    .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => (false, items))
            )), WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(Expression)
        ).Map((items, start, end) => {
            if (items.Item2.Item1)
                return new VariableDeclaration(items.Item2.Item2[0], items.Item1, items.Item3, start, end);
            return new TupleDeconstructDeclaration(items.Item2.Item2, items.Item1, items.Item3, start, end) as
                IBlockItem;
        });

        private static readonly Parser<Expression> ReturnExpression = Seq(
            Tag("return"), Opt(Spacing0.Then(Expression))
        ).Map((items, start, end) => {
            if (items.Item2.IsDefined) return new ReturnValue(items.Item2.Value, start, end);
            return new ReturnEmpty(start, end) as Expression;
        });

        private static readonly Parser<Expression> WhileExpression = Seq(
            Tag("while").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(Expression),
            WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(Expression)
        ).Map((items, start, end) => new While(items.Item1, items.Item2, start, end));

        private static readonly Parser<Expression> ForInExpression = Seq(
            Tag("for").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(Alt(
                DeclarationParameter.Map(item => (true, new List<DeclarationParameter> {item})),
                Delimited1(DeclarationParameterOrPlaceholder, CommaDelimiter)
                    .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map(items => (false, items))
            )),
            WhiteSpaces1.Then(Tag("in")).Then(WhiteSpaces1).Then(Expression),
            WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(Expression)
        ).Map((items, start, end) => {
            if (items.Item1.Item1)
                return new ForIn(items.Item1.Item2[0].target, items.Item1.Item2[0].type, items.Item2, items.Item3,
                    start, end);
            return new ForInDeconstruct(items.Item1.Item2, items.Item2, items.Item3, start, end) as Expression;
        });

        private static readonly Parser<Expression> BreakExpression =
            Tag("break").Map((_, start, end) => new Break(start, end));

        private static readonly Parser<Expression> ContinueExpression =
            Tag("continue").Map((_, start, end) => new Continue(start, end));

        private static readonly Parser<Expression> Block = Char('{').Then(WhiteSpaces0).Then(DelimitedUntil(
            Alt<IBlockItem>(
                Expression,
                LineComment,
                VariableDeclaration,
                ReturnExpression,
                WhileExpression,
                ForInExpression,
                BreakExpression,
                ContinueExpression
            ), WhiteSpaces1, Char('}'))).Map(expressions => new Block(expressions));

        private static readonly Parser<List<Expression>> CallArguments = Char('(').Then(WhiteSpaces0)
            .Then(DelimitedUntil(Expression, CommaDelimiter, WhiteSpaces0.Then(Char(')'))));

        private static readonly Parser<Expression> VariableRefOrCall = Seq(
            Identifier, Many0(Tag("::").Then(Identifier)), Opt(Spacing0.Then(CallArguments))
        ).Map((items, start, end) => {
            List<string> fullName = items.Item2;
            fullName.Insert(0, items.Item1);
            if (items.Item3.IsDefined) {
                return new Call(fullName, items.Item3.Value, start, end);
            }

            return new VariableGet(fullName, start, end) as Expression;
        });

        private static readonly Parser<Expression> TupleCreate =
            DelimitedN_M(2, null, Expression, CommaDelimiter, "<expression>")
                .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')'))).Map((expressions, start, end) =>
                    new TupleCreate(expressions, start, end));

        private static readonly Parser<Expression> RecordCreate = Seq(
            Opt(TypeRef.Between(Char('<').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>')).Then(Spacing0))),
            Delimited1(
                Seq(Identifier, Spacing0.Then(Char(':')).Then(Spacing0).Then(Expression)),
                CommaDelimiter, "<expression>").Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')))
        ).Map((items, start, end) =>
            new RecordCreate(items.Item1.IsDefined ? items.Item1.Value : null, items.Item2, start, end));

        private static readonly Parser<Expression> ArrayCreate = Seq(
            Opt(TypeRef.Between(Char('<').Then(WhiteSpaces0), WhiteSpaces0.Then(Char('>')).Then(Spacing0))),
            Char('[').Then(WhiteSpaces0).Then(DelimitedUntil(Expression, CommaDelimiter, WhiteSpaces0.Then(Char(']'))))
        ).Map((items, start, end) =>
            new ArrayCreate(items.Item1.IsDefined ? items.Item1.Value : null, items.Item2, start, end));

        private static readonly Parser<FunctionParameter> LambdaParameter = Seq(
            Identifier, Opt(TypeSpec)
        ).Map(param => new FunctionParameter(param.Item1, param.Item2.IsDefined ? param.Item2.Value : null));

        private static readonly Parser<List<FunctionParameter>> LambdaParameters = Char('(').Then(WhiteSpaces0)
            .Then(DelimitedUntil(LambdaParameter, CommaDelimiter, WhiteSpaces0.Then(Char(')'))));

        private static readonly Parser<Expression> Lambda = Seq(
            Tag("fn").Then(Spacing0).Then(LambdaParameters),
            WhiteSpaces0.Then(Tag("->")).Then(WhiteSpaces0).Then(Expression)
        ).Map((items, start, end) => new Lambda(items.Item1, items.Item2, start, end));

        private static readonly Parser<Expression> BracketTerm = Expression
            .Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')))
            .Map((expression, start, end) => new Bracket(expression, start, end));

        private static readonly Parser<Expression> RangeCreate = Seq(
            Alt(LiteralInt, BracketTerm, VariableRefOrCall), Spacing0.Then(Tag("..")).Then(Opt(Char('.'))),
            Spacing0.Then(Alt(LiteralInt, BracketTerm, VariableRefOrCall))
        ).Map((items, start, end) => new RangeCreate(items.Item1, items.Item3, items.Item2.IsDefined, start, end));

        private static readonly Parser<Expression> Term = Alt<Expression>(
            LiteralBool,
            LiteralFloat,
            RangeCreate,
            LiteralInt,
            LiteralString,
            BracketTerm,
            Block,
            ArrayCreate,
            TupleCreate,
            RecordCreate,
            VariableRefOrCall,
            Lambda
        );

        private static readonly Parser<IndexSpec> IndexSpec = Expression.Map(expression => new IndexSpec(expression));

        private static readonly Parser<Operator> SuffixOp = Char('?').Map(_ => Operator.Unwrap);

        private static readonly Parser<ISuffixOperation> SuffixOps = Alt(
            Seq(WhiteSpaces0.Then(Char('.')).Then(WhiteSpaces0).Then(Identifier), Opt(Spacing0.Then(CallArguments)))
                .Map(item => {
                    if (item.Item2.IsDefined) return new MethodCallSuffix(item.Item1, item.Item2.Value);
                    return new FieldGetSuffix(item.Item1) as ISuffixOperation;
                }),
            Spacing0.Then(IndexSpec.Between(Char('[').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(']'))))
                .Map(indexSpec => new IndexGetSuffix(indexSpec) as ISuffixOperation),
            Spacing0.Then(SuffixOp).Map(op => new OperatorSuffix(op) as ISuffixOperation)
        );

        private static readonly Parser<Expression> TermWithSuffixOps = Term.Fold0(SuffixOps,
            (target, suffixOp, start, end) => {
                switch (suffixOp) {
                case IndexGetSuffix indexGet: return new IndexGet(target, indexGet.indexSpec, start, end);
                case MethodCallSuffix methodCall:
                    return new MethodCall(target, methodCall.methodName, methodCall.arguments, start, end);
                case FieldGetSuffix fieldGet: return new FieldGet(target, fieldGet.fieldName, start, end);
                case OperatorSuffix operatorSuffix: return new UnarySuffix(target, operatorSuffix.op, start, end);
                default: throw new ParseException(start, new string[] {"<valid suffix>"});
                }
            });

        private static readonly Parser<Operator> UnaryPrefixOp = Alt(
            Char('-').Map(_ => Operator.Neg),
            Char('!').Map(_ => Operator.Not),
            Char('~').Map(_ => Operator.BitNot)
        );

        private static readonly Parser<Expression> UnaryPrefixExpr = Alt(
            Seq(UnaryPrefixOp, WhiteSpaces0.Then(TermWithSuffixOps)).Map((items, start, end) =>
                new UnaryPrefix(items.Item1, items.Item2, start, end)),
            TermWithSuffixOps
        );

        private static readonly Parser<Operator> MulDivBinaryOp = Alt(
            Char('*').Map(_ => Operator.Mul),
            Char('/').Map(_ => Operator.Div),
            Char('%').Map(_ => Operator.Mod)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        private static readonly Parser<Expression> MulDivBinaryExpr = Chain(UnaryPrefixExpr, MulDivBinaryOp,
            (left, op, right, start, end) => new Binary(left, op, right, start, end));

        private static readonly Parser<Operator> AddSubBinaryOp = Alt(
            Char('+').Map(_ => Operator.Add),
            Char('-').Map(_ => Operator.Sub)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        private static readonly Parser<Expression> AddSubBinaryExpr = Chain(MulDivBinaryExpr, AddSubBinaryOp,
            (left, op, right, start, end) => new Binary(left, op, right, start, end));

        private static readonly Parser<Operator> BITOp = Alt(
            Char('&').Map(_ => Operator.BitAnd),
            Char('|').Map(_ => Operator.BitOr),
            Char('^').Map(_ => Operator.BitXor)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        private static readonly Parser<Expression> BITBinaryExpr = Chain(AddSubBinaryExpr, BITOp,
            (left, op, right, start, end) => new Binary(left, op, right, start, end));

        private static readonly Parser<Operator> CompareOp = Alt(
            Tag("==").Map(_ => Operator.Eq),
            Tag("!=").Map(_ => Operator.NotEq),
            Tag("<=").Map(_ => Operator.Le),
            Tag(">=").Map(_ => Operator.Ge),
            Char('<').Map(_ => Operator.Lt),
            Char('>').Map(_ => Operator.Gt)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        private static readonly Parser<Expression> CompareExpr = Chain(BITBinaryExpr, CompareOp,
            (left, op, right, start, end) => new Binary(left, op, right, start, end));

        private static readonly Parser<Operator> BooleanOp = Alt(
            Tag("&&").Map(_ => Operator.BoolAnd),
            Tag("||").Map(_ => Operator.BoolOr)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        private static readonly Parser<Expression> BooleanExpr = Chain(CompareExpr, BooleanOp,
            (left, op, right, start, end) => new BinaryBool(left, op, right, start, end));

        private static readonly Parser<Expression> IfBody = Alt(Expression, ReturnExpression, BreakExpression,
            ContinueExpression);

        private static readonly Parser<Expression> IfExpr = Seq(
            Tag("if").Then(WhiteSpaces0).Then(Char('(')).Then(WhiteSpaces0).Then(BooleanExpr),
            WhiteSpaces0.Then(Char(')')).Then(WhiteSpaces0).Then(Alt(IfBody)),
            Opt(WhiteSpaces1.Then(Tag("else")).Then(WhiteSpaces1).Then(IfBody))
        ).Map((items, start, end) => {
            if (items.Item3.IsDefined)
                return new IfThenElse(items.Item1, items.Item2, items.Item3.Value, start, end);
            return new IfThen(items.Item1, items.Item2, start, end) as Expression;
        });

        private static readonly Parser<Operator> AssignOp = Alt(
            Tag("=").Map(_ => Operator.Assign),
            Tag("+=").Map(_ => Operator.AddAssign),
            Tag("-=").Map(_ => Operator.SubAssign),
            Tag("*=").Map(_ => Operator.MulAssign),
            Tag("/=").Map(_ => Operator.DivAssign),
            Tag("|=").Map(_ => Operator.BitOrAssign),
            Tag("&=").Map(_ => Operator.BitAndAssign),
            Tag("^=").Map(_ => Operator.BitXorAssign)
        ).Between(WhiteSpaces0, WhiteSpaces0);

        private static readonly Parser<Expression> VariableAssignment = Seq(
            Identifier, AssignOp, Alt(BooleanExpr, IfExpr)
        ).Map((items, start, end) => new VariableAssign(items.Item1, items.Item2, items.Item3, start, end));

        private static readonly Parser<List<(string source, string target)>> SourceTargetList = Delimited1(Alt(
            Seq(Identifier, Spacing0.Then(Char('@')).Then(Spacing0).Then(Identifier)),
            Char('_').Map(_ => ("", "")),
            Identifier.Map(id => (id, id))
        ), CommaDelimiter).Between(Char('(').Then(WhiteSpaces0), WhiteSpaces0.Then(Char(')')));

        private static readonly Parser<Expression> TupleDeconstructAssignment = Seq(
            SourceTargetList, WhiteSpaces0.Then(Char('=')).Then(WhiteSpaces0).Then(Alt(BooleanExpr, IfExpr))
        ).Map((items, start, end) => new TupleDeconstructAssign(items.Item1, items.Item2, start, end));

        private static readonly Parser<Expression> TopLevelExpression = Alt(
            TupleDeconstructAssignment,
            VariableAssignment,
            IfExpr,
            BooleanExpr
        );

        private static IResult<Expression> expressionImpl(IInput input) => TopLevelExpression(input);
    }
}
