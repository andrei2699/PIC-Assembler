using FluentAssertions;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;

namespace PIC.Assembler.Adapter.Tests.Out.File;

public class FileParserAdapterTests
{
    private readonly FileParserAdapter _fileParserAdapter = new();

    [Fact]
    public void GivenMissingFile_ShouldThrowException()
    {
        var func = () => _fileParserAdapter.Parse("missing-file.asm");

        func.Should().Throw<FileNotFoundException>();
    }

    [Theory]
    [FileDataPath("TestData/EmptyLines/Empty.asm")]
    [FileDataPath("TestData/EmptyLines/EmptyWithNewLines.asm")]
    [FileDataPath("TestData/EmptyLines/EmptyWithNewLinesAndComments.asm")]
    public void GivenEmptyFile_ShouldReturnEmptyList(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().BeEmpty();
    }

    [Theory]
    [FileDataPath("TestData/End.asm")]
    [FileDataPath("TestData/EndUppercase.asm")]
    public void GivenEnd_ShouldReturnListWithEndToken(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new EndToken());
    }

    [Theory]
    [FileDataPath("TestData/Values/ConstantEquateDecimalValue.asm")]
    [FileDataPath("TestData/Values/ConstantEquateDecimalValueUppercase.asm")]
    public void GivenConstantEquateWithDecimalValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new DecimalValueToken(2));
    }

    [Theory]
    [FileDataPath("TestData/Values/ConstantEquateHexadecimalValue.asm")]
    [FileDataPath("TestData/Values/ConstantEquateHexadecimalValueUppercase.asm")]
    public void GivenConstantEquateWithHexadecimalValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new HexadecimalValueToken(10));
    }

    [Theory]
    [FileDataPath("TestData/Values/ConstantEquateBinaryValue.asm")]
    [FileDataPath("TestData/Values/ConstantEquateBinaryValueUppercase.asm")]
    public void GivenConstantEquateWithBinaryValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new BinaryValueToken(5));
    }

    [Theory]
    [FileDataPath("TestData/DefineCharacterValue.asm")]
    [FileDataPath("TestData/DefineCharacterValueUppercase.asm")]
    public void GivenDefineWithCharacterValue_ShouldReturnListWith3Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new DefineToken(), new NameConstantToken("VARIABLE"), new CharacterValueToken('a'));
    }

    [Theory]
    [FileDataPath("TestData/Include.asm")]
    [FileDataPath("TestData/IncludeUppercase.asm")]
    public void GivenInclude_ShouldReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new IncludeToken(), new StringValueToken("file.inc"));
    }

    [Theory]
    [FileDataPath("TestData/Org.asm")]
    [FileDataPath("TestData/OrgUppercase.asm")]
    public void GivenOrg_ShouldReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new OrgToken(), new HexadecimalValueToken(0));
    }

    [Theory]
    [FileDataPath("TestData/Config.asm")]
    [FileDataPath("TestData/ConfigUppercase.asm")]
    public void GivenConfig_ShouldReturnListWith2Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new ConfigToken(), new NameConstantToken("_WDT_OFF"));
    }

    [Theory]
    [FileDataPath("TestData/Label.asm")]
    [FileDataPath("TestData/LabelUppercase.asm")]
    public void GivenLabel_ShouldReturnListWith1Token(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal( new LabelToken("LABEL"));
    }

    [Theory]
    [FileDataPath("TestData/Operation/LeftShiftExpression.asm")]
    [FileDataPath("TestData/Operation/LeftShiftExpressionCompact.asm")]
    public void GivenLeftShiftExpression_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new DecimalValueToken(1),
            new LeftShiftToken(), new DecimalValueToken(3));
    }

    [Theory]
    [FileDataPath("TestData/Operation/RightShiftExpression.asm")]
    [FileDataPath("TestData/Operation/RightShiftExpressionCompact.asm")]
    public void GivenRightShiftExpression_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new DecimalValueToken(1),
            new RightShiftToken(), new DecimalValueToken(3));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithParenthesis.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithParenthesisCompact.asm")]
    public void GivenConstantEquateWithParenthesis_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new OpenParenthesisToken(),
            new DecimalValueToken(2), new ClosedParenthesisToken());
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithAddition.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithAdditionCompact.asm")]
    public void GivenConstantEquateWithAddition_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new DecimalValueToken(2),
            new PlusToken(), new DecimalValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithSubtraction.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithSubtractionCompact.asm")]
    public void GivenConstantEquateWithSubtraction_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new DecimalValueToken(2),
            new MinusToken(), new DecimalValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithAndBitwise.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithAndBitwiseCompact.asm")]
    public void GivenConstantEquateWithAndBitwise_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new DecimalValueToken(2),
            new AmpersandToken(), new DecimalValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/ConstantEquateWithOrBitwise.asm")]
    [FileDataPath("TestData/Operation/ConstantEquateWithOrBitwiseCompact.asm")]
    public void GivenConstantEquateWithOrBitwise_ShouldReturnListWith5Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new DecimalValueToken(2),
            new BarToken(), new DecimalValueToken(4));
    }

    [Theory]
    [FileDataPath("TestData/Operation/GotoWithNextAddress.asm")]
    [FileDataPath("TestData/Operation/GotoWithNextAddressCompact.asm")]
    public void GivenGotoWithNextAddress_ShouldReturnListWith4Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("GOTO"), new DollarToken(), new PlusToken(),
            new DecimalValueToken(1));
    }
}
