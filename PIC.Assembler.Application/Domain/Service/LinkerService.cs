using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Application.Domain.Service;

public class LinkerService : ILinker
{
    public IEnumerable<AddressableInstruction> Link(IEnumerable<Instruction> instructions)
    {
        throw new NotImplementedException();
    }
}
