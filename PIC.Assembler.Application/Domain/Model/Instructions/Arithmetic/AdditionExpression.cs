namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record AdditionExpression(IArithmeticExpression Left, IArithmeticExpression Right) : IArithmeticExpression
{
    public int Priority() => 1;

    public int Evaluate()
    {
        return Left.Evaluate() + Right.Evaluate();
    }
}
