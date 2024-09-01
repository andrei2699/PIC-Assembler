using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Application.Port.Out;

public interface IHexWriter
{
    void Write(IEnumerable<AddressableInstruction> instructions, string filepath);
}
