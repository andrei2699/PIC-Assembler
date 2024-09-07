using FluentAssertions;
using Moq;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Model;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Service;
using PIC.Assembler.Application.Port.In;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Adapter.Tests.Integration;

public class AssemblerIntegrationTests
{
    private readonly Mock<IConfigLoader> _configLoader = new();
    private readonly AssembleService _assembleService;

    public AssemblerIntegrationTests()
    {
        var fileTokenizerAdapter = new FileTokenizerAdapter();
        _assembleService = new AssembleService(_configLoader.Object, fileTokenizerAdapter,
            new ParserService(fileTokenizerAdapter, new ArithmeticExpressionParser()), new LinkerService(),
            new FileHexDebugWriter());
    }


    [Theory]
    [FileDataPath(filePaths: ["TestData/EmptyProgram.asm", "OutputTestData/EmptyProgram.txt"])]
    [FileDataPath(filePaths: ["TestData/OneInstruction.asm", "OutputTestData/OneInstruction.txt"])]
    [FileDataPath(filePaths: ["TestData/OneInstructionWithLabel.asm", "OutputTestData/OneInstructionWithLabel.txt"])]
    [FileDataPath(filePaths: ["TestData/OneInstructionWithOrg.asm", "OutputTestData/OneInstructionWithOrg.txt"])]
    [FileDataPath(filePaths: ["TestData/SimpleLoop.asm", "OutputTestData/SimpleLoop.txt"])]
    public void GivenInput_WhenAssemble_ThenOutputIsGeneratedAsDebug(string input, string output)
    {
        var instructionSet = GetTestInstructionSet();
        _configLoader.Setup(x => x.Load("config.json"))
            .Returns(new MicrocontrollerConfig(8, 12, instructionSet));

        var outputFilepath = $"{output}-actual.asm";
        _assembleService.Assemble(new AssembleCommand("config.json", input, outputFilepath));

        var expected = File.ReadAllText(output);
        var actual = File.ReadAllText(outputFilepath);

        actual.Should().Be(expected);
    }

    private static InstructionSet GetTestInstructionSet()
    {
        var instructionSet = new InstructionSet();
        instructionSet.AddDefinition(new InstructionDefinition("ADDWF", "000111dfffff", ["f", "d"]));
        instructionSet.AddDefinition(new InstructionDefinition("GOTO", "101kkkkkkkkk", ["k"]));
        return instructionSet;
    }
}
