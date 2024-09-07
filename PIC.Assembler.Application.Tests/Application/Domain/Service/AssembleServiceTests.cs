using Moq;
using PIC.Assembler.Application.Domain.Model;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Service;
using PIC.Assembler.Application.Port.In;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Application.Tests.Application.Domain.Service;

public class AssembleServiceTests
{
    private readonly Mock<IConfigLoader> _configLoader = new();
    private readonly Mock<ITokenizer> _tokenizerMock = new();
    private readonly Mock<IParser> _parserMock = new();
    private readonly Mock<ILinker> _linkerMock = new();
    private readonly Mock<IHexWriter> _hexWriter = new();
    private readonly AssembleService _assembleService;

    public AssembleServiceTests()
    {
        _assembleService = new AssembleService(_configLoader.Object, _tokenizerMock.Object, _parserMock.Object,
            _linkerMock.Object, _hexWriter.Object);
    }

    [Fact]
    public void ShouldWriteHexFile()
    {
        var fileInformation = new FileInformation("input.asm");
        var tokenLists = new List<TokenList> { new([new EndToken(fileInformation)]) };
        var instructions = new List<IInstruction> { new EndInstruction() };
        var addressableInstructions = new List<AddressableInstruction> { new(0, 0) };
        var instructionSet = new InstructionSet();
        _configLoader.Setup(x => x.Load("config.json"))
            .Returns(new MicrocontrollerConfig(8, 8, instructionSet));
        _tokenizerMock.Setup(x => x.Tokenize("input.asm"))
            .Returns(tokenLists);
        _parserMock.Setup(x => x.Parse(tokenLists, instructionSet))
            .Returns(instructions);
        _linkerMock.Setup(x => x.Link(instructions))
            .Returns(addressableInstructions);

        _assembleService.Assemble(new AssembleCommand("config.json", "input.asm", "output.hex"));

        _hexWriter.Verify(x => x.Write(addressableInstructions, "output.hex"), Times.Once);
    }
}
