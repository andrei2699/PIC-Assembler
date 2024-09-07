using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Application.Domain.Service;

public interface ILinker
{
    IEnumerable<AddressableInstruction> Link(IEnumerable<IInstruction> instructions, int configAddress);
}
