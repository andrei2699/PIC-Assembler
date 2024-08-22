using FluentAssertions;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Model.Tokens;
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
}
