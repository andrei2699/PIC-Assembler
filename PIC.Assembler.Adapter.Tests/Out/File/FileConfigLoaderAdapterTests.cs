using FluentAssertions;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Model;
using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Adapter.Tests.Out.File;

public class FileConfigLoaderAdapterTests
{
    private readonly FileConfigLoaderAdapter _fileConfigLoaderAdapter = new();

    [Fact]
    public void GivenMissingFile_ThenThrowException()
    {
        var func = () => _fileConfigLoaderAdapter.Load("missing-file.json");

        func.Should().Throw<FileNotFoundException>();
    }

    [Theory]
    [FileDataPath("ConfigLoaderTestData/NoOpcode.json")]
    public void GivenInstructionWithoutOpcode_ThenThrowMissingInstructionOpcodeException(string filePath)
    {
        var func = () => _fileConfigLoaderAdapter.Load(filePath);

        func.Should().Throw<MissingInstructionOpcodeException>();
    }

    [Theory]
    [FileDataPath("ConfigLoaderTestData/NoInstructions.json")]
    public void GivenNoInstructions_ThenReturnConfigWithoutInstructions(string filePath)
    {
        var config = _fileConfigLoaderAdapter.Load(filePath);

        config.Should().BeEquivalentTo(CreateConfig([]));
    }

    [Theory]
    [FileDataPath("ConfigLoaderTestData/EmptyParameters.json")]
    [FileDataPath("ConfigLoaderTestData/MissingParameters.json")]
    public void GivenNoParameters_ThenReturnConfigWithInstructionWithoutParameters(string filePath)
    {
        var config = _fileConfigLoaderAdapter.Load(filePath);

        config.Should().BeEquivalentTo(CreateConfig([new InstructionDefinition("NO-PARAMETERS", "000011110000", [])]));
    }

    [Theory]
    [FileDataPath("ConfigLoaderTestData/OneParameter.json")]
    public void GivenOneParameter_ThenReturnConfigWithInstructionWithOneParameter(string filePath)
    {
        var config = _fileConfigLoaderAdapter.Load(filePath);

        config.Should()
            .BeEquivalentTo(CreateConfig([new InstructionDefinition("ONE-PARAMETER", "00001111ffff", ["f"])]));
    }

    [Theory]
    [FileDataPath("ConfigLoaderTestData/TwoParameters.json")]
    public void GivenTwoParameters_ThenReturnConfigWithInstructionWithTwoParameters(string filePath)
    {
        var config = _fileConfigLoaderAdapter.Load(filePath);

        config.Should()
            .BeEquivalentTo(CreateConfig([new InstructionDefinition("TWO-PARAMETERS", "0000ffffssss", ["f", "s"])]));
    }

    [Theory]
    [FileDataPath("ConfigLoaderTestData/MultipleParameters.json")]
    public void GivenMultipleParameters_ThenReturnConfigWithInstructionWithMultipleParameters(string filePath)
    {
        var config = _fileConfigLoaderAdapter.Load(filePath);

        config.Should().BeEquivalentTo(CreateConfig([
            new InstructionDefinition("MULTIPLE-PARAMETERS", "abcdefef0011", ["a", "b", "c", "d", "e", "f"])
        ]));
    }

    [Theory]
    [FileDataPath("ConfigLoaderTestData/MultipleInstructions.json")]
    public void GivenMultipleInstructions_ThenReturnConfigWithMultipleInstructions(string filePath)
    {
        var config = _fileConfigLoaderAdapter.Load(filePath);

        config.Should().BeEquivalentTo(CreateConfig([
            new InstructionDefinition("INSTRUCTION1", "000011110000", []),
            new InstructionDefinition("INSTRUCTION2", "000000000000", [])
        ]));
    }

    private static MicrocontrollerConfig CreateConfig(List<InstructionDefinition> instructionDefinitions)
    {
        var instructionSet = new InstructionSet();

        foreach (var instructionDefinition in instructionDefinitions)
        {
            instructionSet.AddDefinition(instructionDefinition);
        }

        return new MicrocontrollerConfig(8, 12, instructionSet, 4095);
    }
}
