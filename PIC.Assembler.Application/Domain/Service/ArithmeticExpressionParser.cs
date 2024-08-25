using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;

namespace PIC.Assembler.Application.Domain.Service;

public class ArithmeticExpressionParser
{
    public IArithmeticExpression Parse(TokenList tokenList)
    {
        var (expression, nextIndex) = ParseExpression(tokenList, 0, 0);

        if (nextIndex < tokenList.Tokens.Count)
        {
            throw new InstructionParseException("unexpected remaining tokens");
        }

        return expression;
    }

    private (IArithmeticExpression expression, int nextIndex) ParseExpression(TokenList tokenList, int index,
        int precedence)
    {
        var (left, nextIndex) = ParseUnary(tokenList, index);

        while (tokenList.GetTokenOption<Token>(nextIndex).HasValue() &&
               GetPrecedence(tokenList.GetTokenOption<Token>(nextIndex).Get()) >= precedence)
        {
            var operatorToken = tokenList.GetTokenOption<Token>(nextIndex).Get();

            (var right, nextIndex) = ParseExpression(tokenList, nextIndex + 1, GetPrecedence(operatorToken) + 1);

            left = operatorToken switch
            {
                PlusToken => new AdditionExpression(left, right),
                MinusToken => new SubtractionExpression(left, right),
                LeftShiftToken => new LeftShiftExpression(left, right),
                RightShiftToken => new RightShiftExpression(left, right),
                AmpersandToken => new BitwiseAndExpression(left, right),
                BarToken => new BitwiseOrExpression(left, right),
                XorToken => new BitwiseXorExpression(left, right),
                _ => throw new InstructionParseException("operator not implemented")
            };
        }

        return (left, nextIndex);
    }

    private (IArithmeticExpression expression, int nextIndex) ParseUnary(TokenList tokenList, int index)
    {
        var token = tokenList.GetTokenOption<Token>(index).OrElseThrow(new InstructionParseException("missing token"));
        switch (token)
        {
            case TildaToken:
                var (expression, nextIndex) = ParseUnary(tokenList, index + 1);
                return (new BitwiseNegationExpression(expression), nextIndex);
            default:
                return ParsePrimary(tokenList, index);
        }
    }

    private (IArithmeticExpression expression, int nextIndex) ParsePrimary(TokenList tokenList, int index)
    {
        var token = tokenList.GetTokenOption<Token>(index).OrElseThrow(new InstructionParseException("missing token"));

        switch (token)
        {
            case NumberValueToken numberValueToken:
                return (new ConstantExpression(numberValueToken.Value), index + 1);
            case OpenParenthesisToken:
                var (expression, nextIndex) = ParseExpression(tokenList, index + 1, 0);

                tokenList.GetTokenOption<ClosedParenthesisToken>(nextIndex)
                    .OrElseThrow(new InstructionParseException("missing close parenthesis"));

                return (expression, nextIndex + 1);
            default:
            {
                if (!IsTokenValid(token))
                {
                    throw new InstructionParseException($"invalid token {token}");
                }

                throw new InstructionParseException($"invalid arithmetic expression {token}");
            }
        }
    }

    private static bool IsTokenValid(Token token)
    {
        return token switch
        {
            NumberValueToken or AmpersandToken or BarToken or ClosedParenthesisToken or DollarToken or LeftShiftToken
                or MinusToken or OpenParenthesisToken or PlusToken or RightShiftToken or NameConstantToken => true,
            _ => false
        };
    }

    private static int GetPrecedence(Token token)
    {
        return token switch
        {
            OpenParenthesisToken => 10,
            ClosedParenthesisToken => -1,
            PlusToken or MinusToken => 5,
            LeftShiftToken or RightShiftToken => 4,
            AmpersandToken => 3,
            XorToken => 2,
            BarToken => 1,
            _ => 0
        };
    }
}
