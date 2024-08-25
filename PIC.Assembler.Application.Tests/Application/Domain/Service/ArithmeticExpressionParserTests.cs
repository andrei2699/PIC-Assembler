using FluentAssertions;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Domain.Service;

namespace PIC.Assembler.Application.Tests.Application.Domain.Service;

public class ArithmeticExpressionParserTests
{
    private readonly ArithmeticExpressionParser _parser = new();

    [Fact]
    public void GivenNumberToken_WhenEvaluate_ThenComputedValue()
    {
        var arithmeticExpression = _parser.Parse(new TokenList([new NumberValueToken(123)]));

        arithmeticExpression.Evaluate().Should().Be(123);
    }

    #region Addition Expression

    [Fact]
    public void GivenAddition_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([new NumberValueToken(1), new PlusToken(), new NumberValueToken(6)]));

        arithmeticExpression.Evaluate().Should().Be(7);
    }

    [Fact]
    public void GivenAdditionWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new PlusToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenAdditionWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new PlusToken(), new NumberValueToken(2)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenAdditionWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new NumberValueToken(2), new PlusToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Subtraction Expression

    [Fact]
    public void GivenSubtraction_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([new NumberValueToken(3), new MinusToken(), new NumberValueToken(1)]));

        arithmeticExpression.Evaluate().Should().Be(2);
    }

    [Fact]
    public void GivenSubtractionWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new MinusToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenSubtractionWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new MinusToken(), new NumberValueToken(2)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenSubtractionWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new NumberValueToken(2), new MinusToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Left Shfit Expression

    [Fact]
    public void GivenLeftShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([new NumberValueToken(1), new LeftShiftToken(), new NumberValueToken(2)]));

        arithmeticExpression.Evaluate().Should().Be(4);
    }

    [Fact]
    public void GivenLeftShiftWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new LeftShiftToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenLeftShiftWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new LeftShiftToken(), new NumberValueToken(2)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenLeftShiftWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new NumberValueToken(2), new LeftShiftToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Right Shift Expression

    [Fact]
    public void GivenRightShift_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([new NumberValueToken(10), new RightShiftToken(), new NumberValueToken(1)]));

        arithmeticExpression.Evaluate().Should().Be(5);
    }

    [Fact]
    public void GivenRightShiftWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new RightShiftToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenRightShiftWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new RightShiftToken(), new NumberValueToken(2)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenRightShiftWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new NumberValueToken(2), new RightShiftToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Bitwise And Expression

    [Fact]
    public void GivenBitwiseAnd_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([new NumberValueToken(3), new AmpersandToken(), new NumberValueToken(1)]));

        arithmeticExpression.Evaluate().Should().Be(1);
    }

    [Fact]
    public void GivenBitwiseAndWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new AmpersandToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseAndWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new AmpersandToken(), new NumberValueToken(2)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseAndWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new NumberValueToken(2), new AmpersandToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Bitwise Or Expression

    [Fact]
    public void GivenBitwiseOr_WhenEvaluate_ThenReturnComputedValue()
    {
        var arithmeticExpression =
            _parser.Parse(new TokenList([new NumberValueToken(1), new BarToken(), new NumberValueToken(2)]));

        arithmeticExpression.Evaluate().Should().Be(3);
    }

    [Fact]
    public void GivenBitwiseOrWithoutOperands_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new BarToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseOrWithoutLeftOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new BarToken(), new NumberValueToken(2)]));

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenBitwiseOrWithoutRightOperand_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([new NumberValueToken(2), new BarToken()]));

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    #region Invalid Expression

    [Fact]
    public void GivenEmptyTokenList_ThenThrowInstructionParseException()
    {
        var func = () => _parser.Parse(new TokenList([]));

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
        yield return [new CharacterValueToken('c')];
        yield return [new StringValueToken("text")];
        yield return [new OpenParenthesisToken()];
        yield return [new ClosedParenthesisToken()];
        yield return [new DollarToken()];
        yield return [new EquateToken()];
        yield return [new EquateToken()];
        yield return [new ConfigToken()];
        yield return [new DefineToken()];
        yield return [new EndToken()];
        yield return [new IncludeToken()];
        yield return [new LabelToken("label")];
        yield return [new OrgToken()];
    }

    #endregion
}
