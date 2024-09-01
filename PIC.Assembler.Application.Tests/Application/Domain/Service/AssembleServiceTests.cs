using Moq;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Service;
using PIC.Assembler.Application.Port.In;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Application.Tests.Application.Domain.Service;

public class AssembleServiceTests
{
    private readonly Mock<ITokenizer> _tokenizerMock = new();
    private readonly Mock<IParser> _parserMock = new();
    private readonly Mock<IHexWriter> _hexWriter = new();
    private readonly AssembleService _assembleService;

    public AssembleServiceTests()
    {
        _assembleService = new AssembleService(_tokenizerMock.Object, _parserMock.Object, _hexWriter.Object);
    }

    [Fact]
    public void ShouldWriteHexFile()
    {
        var tokenLists = new List<TokenList> { new([new EndToken()]) };
        var instructions = new List<Instruction> { new EndInstruction() };
        _tokenizerMock.Setup(x => x.Tokenize("input.asm"))
            .Returns(tokenLists);
        _parserMock.Setup(x => x.Parse(tokenLists, new InstructionSet()))
            .Returns(instructions);

        _assembleService.Assemble(new AssembleCommand("input.asm", "output.hex"));

        _hexWriter.Verify(x => x.Write(instructions, "output.hex"), Times.Once);
    }
}
