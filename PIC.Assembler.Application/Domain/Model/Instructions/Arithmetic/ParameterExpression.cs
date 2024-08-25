namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record ParameterExpression(int Value) : IArithmeticExpression
{
    public int Priority() => 0;

    public int Evaluate()
    {
        return Value;
    }
}
