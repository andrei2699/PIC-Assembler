namespace PIC.Assembler.Application.Domain.Model.Instructions;

public record NoArgMnemonic(string Name, string OpCodePattern)
    : Mnemonic(Name, OpCodePattern);
