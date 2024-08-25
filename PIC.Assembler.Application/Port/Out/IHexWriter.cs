using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Application.Port.Out;

public interface IHexWriter
{
    void Write(IEnumerable<Instruction> instructions, string filepath);
}
