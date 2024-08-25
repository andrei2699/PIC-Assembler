namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record BitwiseOrExpression(IArithmeticExpression Left, IArithmeticExpression Right) : IArithmeticExpression
{
    public int Priority() => 2;

    public int Evaluate()
    {
        return Left.Evaluate() | Right.Evaluate();
    }
}
