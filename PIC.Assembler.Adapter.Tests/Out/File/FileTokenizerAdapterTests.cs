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
    public void GivenMissingFile_ThenThrowException()
    {
        var func = () => _fileTokenizerAdapter.Tokenize("missing-file.asm");

        func.Should().Throw<FileNotFoundException>();
    }

    [Theory]
    [FileDataPath("TokenizerTestData/EmptyLines/Empty.asm")]
    [FileDataPath("TokenizerTestData/EmptyLines/EmptyWithNewLines.asm")]
    [FileDataPath("TokenizerTestData/EmptyLines/EmptyWithNewLinesAndComments.asm")]
    public void GivenEmptyFile_ThenReturnEmptyList(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().BeEmpty();
    }

    [Theory]
    [FileDataPath("TokenizerTestData/End.asm")]
    [FileDataPath("TokenizerTestData/EndUppercase.asm")]
    public void GivenEnd_ThenReturnListWithEndToken(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new EndToken());
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Values/ConstantEquateDecimalValue.asm")]
    [FileDataPath("TokenizerTestData/Values/ConstantEquateDecimalValueUppercase.asm")]
    public void GivenConstantEquateWithDecimalValue_ThenReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Values/ConstantEquateHexadecimalValue.asm")]
    [FileDataPath("TokenizerTestData/Values/ConstantEquateHexadecimalValueUppercase.asm")]
    public void GivenConstantEquateWithHexadecimalValue_ThenReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should()
            .Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(10));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Values/ConstantEquateBinaryValue.asm")]
    [FileDataPath("TokenizerTestData/Values/ConstantEquateBinaryValueUppercase.asm")]
    public void GivenConstantEquateWithBinaryValue_ThenReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(5));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/DefineCharacterValue.asm")]
    [FileDataPath("TokenizerTestData/DefineCharacterValueUppercase.asm")]
    public void GivenDefineWithCharacterValue_ThenReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should()
            .Equal(new DefineToken(), new NameConstantToken("VARIABLE"), new CharacterValueToken('a'));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Include.asm")]
    [FileDataPath("TokenizerTestData/IncludeUppercase.asm")]
    public void GivenInclude_ThenReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new IncludeToken(), new StringValueToken("file.inc"));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Org.asm")]
    [FileDataPath("TokenizerTestData/OrgUppercase.asm")]
    public void GivenOrg_ThenReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new OrgToken(), new NumberValueToken(0));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Config.asm")]
    [FileDataPath("TokenizerTestData/ConfigUppercase.asm")]
    public void GivenConfig_ThenReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new ConfigToken(), new NameConstantToken("_WDT_OFF"));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Label.asm")]
    [FileDataPath("TokenizerTestData/LabelUppercase.asm")]
    public void GivenLabel_ThenReturnListWith1Token(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new LabelToken("LABEL"));
    }


    [Theory]
    [FileDataPath("TokenizerTestData/MnemonicWithComma.asm")]
    [FileDataPath("TokenizerTestData/MnemonicWithCommaCompact.asm")]
    public void GivenMnemonicWithComma_ThenReturnListWith4Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("MOV"), new NameConstantToken("A"), new CommaToken(),
            new NameConstantToken("B"));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/LeftShiftExpression.asm")]
    [FileDataPath("TokenizerTestData/Operation/LeftShiftExpressionCompact.asm")]
    public void GivenLeftShiftExpression_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(1),
            new LeftShiftToken(), new NumberValueToken(3));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/RightShiftExpression.asm")]
    [FileDataPath("TokenizerTestData/Operation/RightShiftExpressionCompact.asm")]
    public void GivenRightShiftExpression_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(1),
            new RightShiftToken(), new NumberValueToken(3));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithParenthesis.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithParenthesisCompact.asm")]
    public void GivenConstantEquateWithParenthesis_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(),
            new OpenParenthesisToken(), new NumberValueToken(2), new ClosedParenthesisToken());
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithAddition.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithAdditionCompact.asm")]
    public void GivenConstantEquateWithAddition_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new PlusToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithAdditionWithOtherConstant.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithAdditionWithOtherConstantCompact.asm")]
    public void GivenConstantEquateWithAdditionWithOtherConstant_ThenReturnListWith10Tokens(string filePath)
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
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithSubtraction.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithSubtractionCompact.asm")]
    public void GivenConstantEquateWithSubtraction_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new MinusToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithAndBitwise.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithAndBitwiseCompact.asm")]
    public void GivenConstantEquateWithAndBitwise_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new AmpersandToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithOrBitwise.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithOrBitwiseCompact.asm")]
    public void GivenConstantEquateWithOrBitwise_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new BarToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithXorBitwise.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithXorBitwiseCompact.asm")]
    public void GivenConstantEquateWithXorBitwise_ThenReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new NumberValueToken(2),
            new XorToken(), new NumberValueToken(4));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithNegationBitwise.asm")]
    [FileDataPath("TokenizerTestData/Operation/ConstantEquateWithNegationBitwiseCompact.asm")]
    public void GivenConstantEquateWithXorBitwise_ThenReturnListWith4Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new TildaToken(),
            new NumberValueToken(5));
    }

    [Theory]
    [FileDataPath("TokenizerTestData/Operation/GotoWithNextAddress.asm")]
    [FileDataPath("TokenizerTestData/Operation/GotoWithNextAddressCompact.asm")]
    public void GivenGotoWithNextAddress_ThenReturnListWith4Tokens(string filePath)
    {
        var tokens = _fileTokenizerAdapter.Tokenize(filePath).ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Tokens.Should().Equal(new NameConstantToken("GOTO"), new DollarToken(), new PlusToken(),
            new NumberValueToken(1));
    }
}
