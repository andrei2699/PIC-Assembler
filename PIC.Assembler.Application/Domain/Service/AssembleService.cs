using PIC.Assembler.Application.Port.In;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Application.Domain.Service;

public class AssembleService(IConfigLoader configLoader, ITokenizer tokenizer, IParser parser, IHexWriter hexWriter)
    : IAssembleUseCase
{
    public void Assemble(AssembleCommand command)
    {
        var config = configLoader.Load(command.ConfigFilepath);

        var tokenLists = tokenizer.Tokenize(command.InputFilepath);
        var instructions = parser.Parse(tokenLists, config.InstructionSet);
        
        hexWriter.Write(instructions, command.OutputFilepath);
    }
}
