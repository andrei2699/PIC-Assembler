using FluentAssertions;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Model.Tokens;

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
    [FileDataPath("TestData/Empty.asm")]
    [FileDataPath("TestData/EmptyWithNewLines.asm")]
    [FileDataPath("TestData/EmptyWithNewLinesAndComments.asm")]
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

        tokens.Should().Equal([new EndToken()]);
    }
}
