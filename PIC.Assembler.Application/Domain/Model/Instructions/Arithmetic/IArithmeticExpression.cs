namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public interface IArithmeticExpression
{
    int Priority();

    int Evaluate();
}
