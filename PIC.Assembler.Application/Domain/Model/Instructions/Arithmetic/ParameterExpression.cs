namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record ParameterExpression(int Value) : IArithmeticExpression
{
    public int Evaluate() => Value;
}
