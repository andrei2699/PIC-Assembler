using FluentAssertions;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Domain.Service;

namespace PIC.Assembler.Application.Tests.Application.Domain.Service;

public class ArithmeticExpressionParserTests
{
    private static readonly FileInformation FileInformation = new("file-path");
    private readonly ArithmeticExpressionParser _parser = new();

    [Fact]
    public void GivenNumberToken_WhenEvaluate_ThenComputedValue()
    {
        var arithmeticExpression = _parser.Parse(new TokenList([new NumberValueToken(123, FileInformation)]));

        arithmeticExpression.Evaluate().Should().Be(123);
    }

    #region Numeric Negation

    [Fact]
    public void GivenNegativeNumberToken_WhenEvaluate_ThenComputedValue()
    {
        var arithmeticExpression = _parser.Parse(new TokenList([
            new MinusToken(FileInformation), new NumberValueToken(123, FileInformation)
        ]));

        arithmeticExpression.Evaluate().Should().Be(-123);
    }

    [Fact]
    public void GivenNegativeNumberTokenMultipleTimes_WhenEvaluate_ThenComputedValue()
    {
        var arithmeticExpression = _parser.Parse(new TokenList([
            new MinusToken(FileInformation), new MinusToken(FileInformation), new MinusToken(FileInformation),
            new NumberValueToken(123, FileInformation)
        ]));

        arithmeticExpression.Evaluate().Should().Be(-123);
    }

    #endregion

    #region Addition Expression

    [Fact]
    public void GivenAddition_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(6, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(7);
    }

    [Fact]
    public void GivenAdditionWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new PlusToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenAdditionWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new PlusToken(FileInformation), new NumberValueToken(2, FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenAdditionWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new NumberValueToken(2, FileInformation), new PlusToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Subtraction Expression

    [Fact]
    public void GivenSubtraction_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(3, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(1, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(2);
    }

    [Fact]
    public void GivenSubtractionWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new MinusToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenSubtractionWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new NumberValueToken(2, FileInformation), new MinusToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Left Shfit Expression

    [Fact]
    public void GivenLeftShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(2, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(4);
    }

    [Fact]
    public void GivenLeftShiftWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new LeftShiftToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenLeftShiftWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new LeftShiftToken(FileInformation), new NumberValueToken(2, FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenLeftShiftWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new NumberValueToken(2, FileInformation), new LeftShiftToken(FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Right Shift Expression

    [Fact]
    public void GivenRightShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(10, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(1, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(5);
    }

    [Fact]
    public void GivenRightShiftWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new RightShiftToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenRightShiftWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new RightShiftToken(FileInformation), new NumberValueToken(2, FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenRightShiftWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new NumberValueToken(2, FileInformation), new RightShiftToken(FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Bitwise And Expression

    [Fact]
    public void GivenBitwiseAnd_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(3, FileInformation), new AmpersandToken(FileInformation),
                new NumberValueToken(1, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1);
    }

    [Fact]
    public void GivenBitwiseAndWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new AmpersandToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseAndWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new AmpersandToken(FileInformation), new NumberValueToken(2, FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseAndWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new NumberValueToken(2, FileInformation), new AmpersandToken(FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Bitwise Or Expression

    [Fact]
    public void GivenBitwiseOr_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new BarToken(FileInformation),
                new NumberValueToken(2, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(3);
    }

    [Fact]
    public void GivenBitwiseOrWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new BarToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseOrWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new BarToken(FileInformation), new NumberValueToken(2, FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseOrWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new NumberValueToken(2, FileInformation), new BarToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Bitwise Xor Expression

    [Fact]
    public void GivenBitwiseXor_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new XorToken(FileInformation),
                new NumberValueToken(3, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1 ^ 3);
    }

    [Fact]
    public void GivenBitwiseXorWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new XorToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseXorWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new XorToken(FileInformation), new NumberValueToken(2, FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseXorWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new NumberValueToken(2, FileInformation), new XorToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Bitwise Negation Expression

    [Fact]
    public void GivenBitwiseNegation_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([new TildaToken(FileInformation), new NumberValueToken(5, FileInformation)]));

        arithmeticExpression.Evaluate().Should().Be(~5);
    }


    [Fact]
    public void GivenBitwiseNegationMultipleTimes_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(
                new TokenList([
                    new TildaToken(FileInformation), new TildaToken(FileInformation),
                    new TildaToken(FileInformation), new NumberValueToken(5, FileInformation)
                ]));

        arithmeticExpression.Evaluate().Should().Be(~5);
    }

    [Fact]
    public void GivenBitwiseNegationWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new TildaToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseNegationWithLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([new NumberValueToken(2, FileInformation), new TildaToken(FileInformation)]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Precedence

    [Fact]
    public void GivenAdditionsAndSubtractions_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(2, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(3, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(4, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(5, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(8, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1 + 2 + 3 - 4 - 5 + 8);
    }

    [Fact]
    public void GivenLeftAndRightShifts_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(2, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(7, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(4, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(5, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(8, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1 << 2 << 7 >> 4 >> 5 << 8);
    }

    [Fact]
    public void GivenAdditionAndLeftShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(2, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(3, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(4, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(5, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(8, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1 + 2 + 3 << 4 << 5 + 8);
    }

    [Fact]
    public void GivenAdditionAndRightShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(8, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(9999, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(2, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(1, FileInformation), new PlusToken(FileInformation),
                new NumberValueToken(8, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1 + 8 + 9999 >> 2 >> 1 + 8);
    }

    [Fact]
    public void GivenSubtractionAndLeftShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(999, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(1, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(7, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(2, FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(7, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(4, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(999 - 1 - 7 << 2 << 7 - 4);
    }

    [Fact]
    public void GivenSubtractionAndRightShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(999, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(1, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(7, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(2, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(7, FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(4, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(999 - 1 - 7 >> 2 >> 7 - 4);
    }


    [Fact]
    public void GivenBitwiseAndOrXor_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new NumberValueToken(1, FileInformation), new BarToken(FileInformation),
                new NumberValueToken(5, FileInformation), new XorToken(FileInformation),
                new NumberValueToken(3, FileInformation), new AmpersandToken(FileInformation),
                new NumberValueToken(7, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1 | 5 ^ 3 & 7);
    }

    #endregion

    #region Parenthesis

    [Fact]
    public void GivenNumberInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(1234, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1234);
    }

    [Fact]
    public void GivenNumberInMultipleParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new OpenParenthesisToken(FileInformation),
                new OpenParenthesisToken(FileInformation), new NumberValueToken(1234, FileInformation),
                new ClosedParenthesisToken(FileInformation), new ClosedParenthesisToken(FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1234);
    }

    [Fact]
    public void GivenNegativeNumberInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(1234, FileInformation), new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(-1234);
    }


    [Fact]
    public void GivenNegativeNumberInParenthesisMultipleTimes_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new MinusToken(FileInformation),
                new MinusToken(FileInformation), new MinusToken(FileInformation),
                new NumberValueToken(1234, FileInformation), new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(-1234);
    }

    [Fact]
    public void GivenAdditionInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(1, FileInformation),
                new PlusToken(FileInformation), new NumberValueToken(2, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(3);
    }

    [Fact]
    public void GivenSubtractionInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(3, FileInformation),
                new MinusToken(FileInformation), new NumberValueToken(2, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(1);
    }

    [Fact]
    public void GivenLeftShiftInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(4, FileInformation),
                new LeftShiftToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(8);
    }

    [Fact]
    public void GivenRightShiftInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(10, FileInformation),
                new RightShiftToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(5);
    }

    [Fact]
    public void GivenBitwiseAndInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(10, FileInformation),
                new AmpersandToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(10 & 1);
    }

    [Fact]
    public void GivenBitwiseOrInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(10, FileInformation),
                new BarToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(10 | 1);
    }

    [Fact]
    public void GivenBitwiseXorInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(10, FileInformation),
                new XorToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(10 ^ 1);
    }

    [Fact]
    public void GivenBitwiseNegationInParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new TildaToken(FileInformation),
                new NumberValueToken(1, FileInformation), new ClosedParenthesisToken(FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be(~1);
    }

    [Fact]
    public void GivenComplexExpressionWithParenthesis_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(10, FileInformation),
                new PlusToken(FileInformation), new NumberValueToken(5, FileInformation),
                new ClosedParenthesisToken(FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(3, FileInformation), new MinusToken(FileInformation),
                new OpenParenthesisToken(FileInformation), new NumberValueToken(2, FileInformation),
                new LeftShiftToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation), new PlusToken(FileInformation),
                new OpenParenthesisToken(FileInformation), new OpenParenthesisToken(FileInformation),
                new NumberValueToken(8, FileInformation), new RightShiftToken(FileInformation),
                new NumberValueToken(1, FileInformation), new ClosedParenthesisToken(FileInformation),
                new MinusToken(FileInformation), new NumberValueToken(3, FileInformation),
                new ClosedParenthesisToken(FileInformation), new LeftShiftToken(FileInformation),
                new NumberValueToken(2, FileInformation)
            ]));

        arithmeticExpression.Evaluate().Should().Be((10 + 5) << 3 - (2 << 1) + ((8 >> 1) - 3) << 2);
    }

    #endregion

    #region Invalid Expression

    [Fact]
    public void GivenEmptyTokenList_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithMultipleNumbersWithoutOperators_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new NumberValueToken(2, FileInformation), new NumberValueToken(3, FileInformation),
                new NumberValueToken(6, FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithOneNumberWithMultipleBinaryOperators_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new NumberValueToken(2, FileInformation), new PlusToken(FileInformation),
                new PlusToken(FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListStartingWithCloseParenthesis_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new ClosedParenthesisToken(FileInformation), new NumberValueToken(2, FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithUnclosedParenthesis_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(2, FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithDoubleClosedParenthesis_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parser.Parse(new TokenList([
                new OpenParenthesisToken(FileInformation), new NumberValueToken(2, FileInformation),
                new ClosedParenthesisToken(FileInformation), new ClosedParenthesisToken(FileInformation)
            ]));

        func.Should().Throw<InstructionParseException>();
    }

    [Theory]
    [MemberData(nameof(GetInvalidTokenAtBeginning))]
    public void GivenInvalidTokenAtBeginningOfList_ThenThrowInstructionParseException(Token token)
    {
        var func = () => _parser.Parse(new TokenList([token]));

        func.Should().Throw<InstructionParseException>();
    }

    public static IEnumerable<object[]> GetInvalidTokenAtBeginning()
    {
        yield return [new CharacterValueToken('c', FileInformation)];
        yield return [new StringValueToken("text", FileInformation)];
        yield return [new OpenParenthesisToken(FileInformation)];
        yield return [new ClosedParenthesisToken(FileInformation)];
        yield return [new DollarToken(FileInformation)];
        yield return [new EquateToken(FileInformation)];
        yield return [new EquateToken(FileInformation)];
        yield return [new ConfigToken(FileInformation)];
        yield return [new DefineToken(FileInformation)];
        yield return [new EndToken(FileInformation)];
        yield return [new IncludeToken(FileInformation)];
        yield return [new LabelToken("label", FileInformation)];
        yield return [new OrgToken(FileInformation)];
        yield return [new CommaToken(FileInformation)];
    }

    #endregion
}
