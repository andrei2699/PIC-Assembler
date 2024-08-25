namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record NumericNegationExpression(IArithmeticExpression Operand) : IArithmeticExpression
{
    public int Evaluate() => -Operand.Evaluate();
}
