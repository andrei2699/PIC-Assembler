using System.Text.Json;
using PIC.Assembler.Adapter.Out.File.Contracts;
using PIC.Assembler.Application.Domain.Model;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Adapter.Out.File;

public class FileConfigLoaderAdapter : IConfigLoader
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public MicrocontrollerConfig Load(string filepath)
    {
        var content = System.IO.File.ReadAllText(filepath);
        var config = JsonSerializer.Deserialize<FileMicrocontrollerConfig>(content, _jsonSerializerOptions)!;

        return new MicrocontrollerConfig(config.Bits, config.Opcode, CreateInstructionSet(config.Instructions),
            Convert.ToInt32(config.ConfigAddress, 16));
    }

    private static InstructionSet CreateInstructionSet(List<FileInstructionDefinition> instructions)
    {
        var instructionSet = new InstructionSet();
        foreach (var instruction in instructions)
        {
            if (string.IsNullOrWhiteSpace(instruction.Opcode))
            {
                throw new MissingInstructionOpcodeException();
            }

            instructionSet.AddDefinition(new InstructionDefinition(instruction.Name,
                instruction.Opcode.Replace(" ", ""),
                (instruction.Parameters ?? []).Select(p => p.Name).ToList()));
        }

        return instructionSet;
    }
}
