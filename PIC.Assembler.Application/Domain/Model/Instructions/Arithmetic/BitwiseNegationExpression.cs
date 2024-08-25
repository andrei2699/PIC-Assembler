namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record BitwiseNegationExpression(IArithmeticExpression Operand) : IArithmeticExpression
{
    public int Evaluate() => ~Operand.Evaluate();
}
