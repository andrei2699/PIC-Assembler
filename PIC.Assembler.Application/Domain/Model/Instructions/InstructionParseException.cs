using PIC.Assembler.Application.Domain.Model.Tokens;

namespace PIC.Assembler.Application.Domain.Model.Instructions;

public class InstructionParseException : Exception
{
    public InstructionParseException(string message) : base(message)
    {
    }

    public InstructionParseException(string message, FileInformation fileInformation) : base(
        $"{message} in file {fileInformation.FilePath} at line {fileInformation.Line} ")
    {
    }
}
