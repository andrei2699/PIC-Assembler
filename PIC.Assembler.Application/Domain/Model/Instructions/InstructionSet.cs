namespace PIC.Assembler.Application.Domain.Model.Instructions;

public class InstructionSet
{
    private readonly Dictionary<Tuple<string, int>, InstructionDefinition> _instructions = new();

    public void AddDefinition(InstructionDefinition definition)
    {
        _instructions.Add(new Tuple<string, int>(definition.Name, definition.ParameterNames.Count), definition);
    }

    public InstructionDefinition GetDefinition(string name, int parameterCount)
    {
        return _instructions[new Tuple<string, int>(name, parameterCount)];
    }
}
