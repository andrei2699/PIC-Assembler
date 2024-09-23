namespace PIC.Assembler.Adapter.Out.File.Contracts;

public record FileMicrocontrollerConfig(
    int Bits,
    int Opcode,
    List<FileInstructionDefinition> Instructions,
    int ConfigAddress);
