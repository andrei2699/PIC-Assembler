namespace PIC.Assembler.Application.Domain.Model.Instructions;

public record Mnemonic(string Name, string OpCodePattern, List<int> Parameters) : Instruction;
