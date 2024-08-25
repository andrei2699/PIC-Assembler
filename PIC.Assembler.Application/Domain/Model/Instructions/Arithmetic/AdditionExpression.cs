namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record AdditionExpression(IArithmeticExpression Left, IArithmeticExpression Right) : IArithmeticExpression
{
    public int Evaluate() => Left.Evaluate() + Right.Evaluate();
}
