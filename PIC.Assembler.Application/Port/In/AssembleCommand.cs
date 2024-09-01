namespace PIC.Assembler.Application.Port.In;

public record AssembleCommand(string ConfigFilepath, string InputFilepath, string OutputFilepath);
