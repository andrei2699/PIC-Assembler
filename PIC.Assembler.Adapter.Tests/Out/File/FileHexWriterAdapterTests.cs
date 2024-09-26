using FluentAssertions;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Adapter.Tests.Out.File;

public class FileHexWriterAdapterTests
{
    private readonly FileHexWriterAdapter _fileHexWriterAdapter = new();

    [Theory]
    [FileDataPath("HexWriterTestData/EndInstruction.hex")]
    public void WriteEndInstruction(string expectedFilePath)
    {
        var outputFilepath = $"{expectedFilePath}-actual.hex";

        _fileHexWriterAdapter.Write([new EndOfFileAddressableInstruction()], outputFilepath);

        var expected = System.IO.File.ReadAllText(expectedFilePath);
        var actual = System.IO.File.ReadAllText(outputFilepath);
        actual.Should().Be(expected);
    }

    [Theory]
    [FileDataPath("HexWriterTestData/AddressableInstruction.hex")]
    public void WriteAddressableInstruction(string expectedFilePath)
    {
        var outputFilepath = $"{expectedFilePath}-actual.hex";

        _fileHexWriterAdapter.Write([new AddressableInstruction(0xFE, 0xA5)], outputFilepath);

        var expected = System.IO.File.ReadAllText(expectedFilePath);
        var actual = System.IO.File.ReadAllText(outputFilepath);
        actual.Should().Be(expected);
    }

    [Theory]
    [FileDataPath("HexWriterTestData/AddressableInstructionList.hex")]
    public void WriteAddressableInstructionList(string expectedFilePath)
    {
        var outputFilepath = $"{expectedFilePath}-actual.hex";

        _fileHexWriterAdapter.Write([new AddressableInstruction(0x24, 0xBE), new EndOfFileAddressableInstruction()],
            outputFilepath);

        var expected = System.IO.File.ReadAllText(expectedFilePath);
        var actual = System.IO.File.ReadAllText(outputFilepath);
        actual.Should().Be(expected);
    }
    
    
    [Theory]
    [FileDataPath("HexWriterTestData/AddressableInstructionWith3NibblesInData.hex")]
    public void WriteAddressableInstructionWith3NibblesInData(string expectedFilePath)
    {
        var outputFilepath = $"{expectedFilePath}-actual.hex";

        _fileHexWriterAdapter.Write([new AddressableInstruction(0x00, 0xCFD)],
            outputFilepath);

        var expected = System.IO.File.ReadAllText(expectedFilePath);
        var actual = System.IO.File.ReadAllText(outputFilepath);
        actual.Should().Be(expected);
    }
}
