using PIC.Assembler.Application.Domain.Model;

namespace PIC.Assembler.Application.Port.In;

public interface IAssembleUseCase
{
    List<HexRecord> Assemble(AssembleCommand command);
}
