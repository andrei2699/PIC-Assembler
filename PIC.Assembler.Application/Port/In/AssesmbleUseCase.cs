namespace PIC.Assembler.Application.Port.In;

public interface IAssembleUseCase
{
    void Assemble(AssembleCommand command);
}
