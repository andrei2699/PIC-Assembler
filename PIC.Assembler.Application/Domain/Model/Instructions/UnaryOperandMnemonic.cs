namespace PIC.Assembler.Application.Domain.Model.Instructions;

public record UnaryOperandMnemonic(
    string Name,
    string OpCodePattern,
    Parameter Parameter) : Mnemonic(Name, OpCodePattern);
