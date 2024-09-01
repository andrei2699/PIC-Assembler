namespace PIC.Assembler.Adapter.Out.File.Contracts;

public record FileInstructionDefinition(string Name, List<FileInstructionParameter>? Parameters, string Opcode);
