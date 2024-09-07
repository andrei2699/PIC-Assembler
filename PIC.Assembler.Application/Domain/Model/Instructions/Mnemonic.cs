namespace PIC.Assembler.Application.Domain.Model.Instructions;

public record Mnemonic(InstructionDefinition Definition, List<IMnemonicParameter> Parameters) : IInstruction;
