namespace PIC.Assembler.Application.Domain.Model.Instructions;

public record InstructionDefinition(string Name, string OpCodePattern, List<string> ParameterNames);
