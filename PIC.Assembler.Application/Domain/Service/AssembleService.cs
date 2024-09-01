using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Port.In;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Application.Domain.Service;

public class AssembleService(ITokenizer tokenizer, IParser parser, IHexWriter hexWriter) : IAssembleUseCase
{
    public void Assemble(AssembleCommand command)
    {
        // 1. parse input file
        // 2. preprocessor - if #include is detected, stop parsing main file and parse the #include and then continue with main file (do recursive if needed)
        // 3. create instructions with addresses based on Org and other tokens
        // 4. parse expressions if needed
        // 5. replace constants with values
        // 6. write to output file in hex format

        hexWriter.Write(parser.Parse(tokenizer.Tokenize(command.InputFilepath), new InstructionSet()),
            command.OutputFilepath);
    }
}
