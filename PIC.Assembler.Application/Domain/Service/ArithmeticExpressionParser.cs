using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;

namespace PIC.Assembler.Application.Domain.Service;

public class ArithmeticExpressionParser
{
    public IArithmeticExpression Parse(TokenList tokens)
    {
        var (expression, nextIndex) = ParseExpression(tokens, 0);

        if (nextIndex < tokens.Tokens.Count)
        {
            throw new InstructionParseException("unexpected token detected");
        }

        return expression;
    }

    private (IArithmeticExpression expression, int nextIndex) ParseExpression(TokenList tokenList, int index)
    {
        var (leftExpression, operandNextIndex) = ParseTerm(tokenList, index);

        var operatorToken = tokenList.GetTokenOption<Token>(operandNextIndex);
        if (!operatorToken.HasValue())
        {
            return (leftExpression, operandNextIndex + 1);
        }

        var (rightExpression, rightOperandNextIndex) = ParseTerm(tokenList, operandNextIndex + 1);

        return operatorToken.Get() switch
        {
            PlusToken => (new AdditionExpression(leftExpression, rightExpression), rightOperandNextIndex),
            MinusToken => (new SubtractionExpression(leftExpression, rightExpression), rightOperandNextIndex),
            LeftShiftToken => (new LeftShiftExpression(leftExpression, rightExpression), rightOperandNextIndex),
            RightShiftToken => (new RightShiftExpression(leftExpression, rightExpression), rightOperandNextIndex),
            AmpersandToken => (new BitwiseAndExpression(leftExpression, rightExpression), rightOperandNextIndex),
            BarToken => (new BitwiseOrExpression(leftExpression, rightExpression), rightOperandNextIndex),
            _ => throw new InstructionParseException("operator not implemented")
        };
    }

    private (IArithmeticExpression expression, int nextIndex) ParseTerm(TokenList tokenList, int index)
    {
        var tokenOption = tokenList.GetTokenOption<Token>(index);

        if (!tokenOption.HasValue())
        {
            throw new InstructionParseException("missing token");
        }

        var token = tokenOption.Get();
        switch (token)
        {
            case NumberValueToken numberValueToken:
                return (new ConstantExpression(numberValueToken.Value), index + 1);
            case OpenParenthesisToken:
                var (expression, nextIndex) = ParseExpression(tokenList, index + 1);

                tokenList.GetTokenOption<ClosedParenthesisToken>(nextIndex)
                    .OrElseThrow(new InstructionParseException("missing close parenthesis"));

                return (expression, nextIndex + 2);
            default:
            {
                if (IsTokenValid(token))
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
}
