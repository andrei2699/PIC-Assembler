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
    [FileDataPath("TestData/OnlyEnd.asm")]
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
    public void GivenConstantEquateWithBinaryValue_ShouldReturnList0With3Tokens(string filePath)
    {
        var tokens = _fileParserAdapter.Parse(filePath);

        tokens.Should().Equal(new NameConstantToken("VARIABLE"), new EquateToken(), new BinaryValueToken(5));
    }
}
