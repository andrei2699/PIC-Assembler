namespace PIC.Assembler.Application.Domain.Model.Instructions.Arithmetic;

public record ConstantExpression(int Value) : IArithmeticExpression
{
    public int Evaluate() => Value;
}
