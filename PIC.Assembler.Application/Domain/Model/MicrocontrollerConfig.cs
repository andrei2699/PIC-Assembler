using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Application.Domain.Model;

public record MicrocontrollerConfig(int Bits, int OpCodeBitSize, InstructionSet InstructionSet, int ConfigAddress);
