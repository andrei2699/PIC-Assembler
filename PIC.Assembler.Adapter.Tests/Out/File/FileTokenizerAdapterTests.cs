using FluentAssertions;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;

namespace PIC.Assembler.Adapter.Tests.Out.File;

public class FileTokenizerAdapterTests
{
    private readonly FileTokenizerAdapter _fileTokenizerAdapter = new();

    [Fact]
    public void GivenMissingFile_ShouldThrowException()
    {
        var func = () => _fileTokenizerAdapter.Tokenize("missing-file.asm");

        func.Should().Throw<FileNotFoundException>();
    }

    [Theory]
    [FileDataPath("TestData/EmptyLines/Empty.asm")]
    [FileDataPath("TestData/EmptyLines/EmptyWithNewLines.asm")]
    [FileDataPath("TestData/EmptyLines/EmptyWithNewLinesAndComments.asm")]
    public void GivenEmptyFile_ShouldReturnEmptyList(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().BeEmpty();
    }

    [Theory]
    [FileDataPath("TestData/End.asm")]
    [FileDataPath("TestData/EndUppercase.asm")]
    public void GivenEnd_ShouldReturnListWithEndToken(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new EndToken());
    }

    [Theory]
    [FileDataPath("TestData/Values/ConstantEquateDecimalValue.asm")]
    [FileDataPath("TestData/Values/ConstantEquateDecimalValueUppercase.asm")]
    public void GivenConstantEquateWithDecimalValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2));
    }

    [Theory]
    [FileDataPath("TestData/Values/ConstantEquateHexadecimalValue.asm")]
    [FileDataPath("TestData/Values/ConstantEquateHexadecimalValueUppercase.asm")]
    public void GivenConstantEquateWithHexadecimalValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should()
            .Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(10));
    }

    [Theory]
    [FileDataPath("TestData/Values/ConstantEquateBinaryValue.asm")]
    [FileDataPath("TestData/Values/ConstantEquateBinaryValueUppercase.asm")]
    public void GivenConstantEquateWithBinaryValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(5));
    }

    [Theory]
    [FileDataPath("TestData/DefineCharacterValue.asm")]
    [FileDataPath("TestData/DefineCharacterValueUppercase.asm")]
    public void GivenDefineWithCharacterValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should()
            .Equal(new DefineToken(), new NameConstantToken("VARIABLE"), new CharacterValueToken('a'));
    }

    [Theory]
    [FileDataPath("TestData/Include.asm")]
    [FileDataPath("TestData/IncludeUppercase.asm")]
    public void GivenInclude_ShouldReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new IncludeToken(), new StringValueToken("file.inc"));
    }

    [Theory]
    [FileDataPath("TestData/Org.asm")]
    [FileDataPath("TestData/OrgUppercase.asm")]
    public void GivenOrg_ShouldReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new OrgToken(), new NumberValueToken(0));
    }

    [Theory]
    [FileDataPath("TestData/Config.asm")]
    [FileDataPath("TestData/ConfigUppercase.asm")]
    public void GivenConfig_ShouldReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new ConfigToken(), new NameConstantToken("_WDT_OFF"));
    }

    [Theory]
    [FileDataPath("TestData/Label.asm")]
    [FileDataPath("TestData/LabelUppercase.asm")]
    public void GivenLabel_ShouldReturnListWith1Token(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new LabelToken("LABEL"));
    }


    [Theory]
    [FileDataPath("TestData/MnemonicWithComma.asm")]
    [FileDataPath("TestData/MnemonicWithCommaCompact.asm")]
    public void GivenMnemonicWithComma_ShouldReturnListWith4Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("MOV"), new NameConstantToken("A"), new CommaToken(),
            new NameConstantToken("B"));
    }

    [Theory]
    [FileDataPath("TestData/Operation/LeftShiftExpression.asm")]
    [FileDataPath("TestData/Operation/LeftShiftExpressionCompact.asm")]
    public void GivenLeftShiftExpression_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(1),
            new LeftShiftToken(), new NumberValueToken(3));
    }

    [Theory]
    [FileDataPath("TestData/Operation/RightShiftExpression.asm")]
    [FileDataPath("TestData/Operation/RightShiftExpressionCompact.asm")]
    public void GivenRightShiftExpression_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(1),
            new RightShiftToken(), new NumberValueToken(3));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithParenthesis.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithParenthesisCompact.asm")]
    public void GivenConstantEquateWithParenthesis_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(),
            new OpenParenthesisToken(), new NumberValueToken(2), new ClosedParenthesisToken());
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithAddition.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithAdditionCompact.asm")]
    public void GivenConstantEquateWithAddition_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new PlusToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithAdditionWithOtherConstant.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithAdditionWithOtherConstantCompact.asm")]
    public void GivenConstantEquateWithAdditionWithOtherConstant_ShouldReturnListWith10Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(2);
        tokens[0].Tokens.Should()
            .Equal(new NameConstantToken("VARIABLE1"), new EquateToken(), new NumberValueToken(5));
        tokens[1].Tokens.Should()
            .Equal(new NameConstantToken("VARIABLE2"), new EquateToken(), new OpenParenthesisToken(),
                new NameConstantToken("VARIABLE1"), new PlusToken(), new NumberValueToken(2),
                new ClosedParenthesisToken());
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithSubtraction.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithSubtractionCompact.asm")]
    public void GivenConstantEquateWithSubtraction_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new MinusToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithAndBitwise.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithAndBitwiseCompact.asm")]
    public void GivenConstantEquateWithAndBitwise_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new AmpersandToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithOrBitwise.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithOrBitwiseCompact.asm")]
    public void GivenConstantEquateWithOrBitwise_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new BarToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithXorBitwise.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithXorBitwiseCompact.asm")]
    public void GivenConstantEquateWithXorBitwise_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new XorToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithNegationBitwise.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithNegationBitwiseCompact.asm")]
    public void GivenConstantEquateWithXorBitwise_ShouldReturnListWith4Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new TildaToken(),
            new NumberValueToken(5));
    }

    [Theory]
    [FileDataPath("TestData/Operation/GotoWithNextAddress.asm")]
    [FileDataPath("TestData/Operation/GotoWithNextAddressCompact.asm")]
    public void GivenGotoWithNextAddress_ShouldReturnListWith4Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("GOTO"), new DollarToken(), new PlusToken(),
            new NumberValueToken(1));
    }
}
