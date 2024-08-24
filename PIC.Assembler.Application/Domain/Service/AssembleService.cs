using PIC.Assembler.Application.Domain.Model;
using PIC.Assembler.Application.Port.In;

namespace PIC.Assembler.Application.Domain.Service;

public class AssembleService : IAssembleUseCase
{
    public IEnumerable<HexRecord> Assemble(AssembleCommand command)
    {
        throw new NotImplementedException();
    }
}
