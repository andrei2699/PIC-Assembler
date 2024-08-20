namespace PIC.Assembler.Application.Domain.Model.Instructions;

public record BinaryOperandMnemonic(
    string Name,
    string OpCodePattern,
    Parameter FirstParameter,
    Parameter SecondParameter)
    : Mnemonic(Name, OpCodePattern);
