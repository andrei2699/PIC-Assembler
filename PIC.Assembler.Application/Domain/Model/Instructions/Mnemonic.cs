namespace PIC.Assembler.Application.Domain.Model.Instructions;

public abstract record Mnemonic(string Name, string OpCodePattern) : Instruction;
