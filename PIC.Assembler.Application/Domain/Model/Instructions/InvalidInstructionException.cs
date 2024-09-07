namespace PIC.Assembler.Application.Domain.Model.Instructions;

public class InvalidInstructionException(string name) : Exception(name);
