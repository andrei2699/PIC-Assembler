using PIC.Assembler.Application.Domain.Model;

namespace PIC.Assembler.Application.Port.In;

public interface IAssembleUseCase
{
    List<BinaryCodeInstruction> Assemble(AssembleCommand command);
}
