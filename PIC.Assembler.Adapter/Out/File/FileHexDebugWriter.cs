using System.Text;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Adapter.Out.File;

public class FileHexDebugWriter : IHexWriter
{
    public void Write(IEnumerable<AddressableInstruction> instructions, string filepath)
    {
        var stringBuilder = new StringBuilder();

        foreach (var instruction in instructions)
        {
            if (instruction is EndOfFileAddressableInstruction)
            {
                break;
            }

            var address = instruction.Address.ToString("X4");
            var code = instruction.Instruction.ToString("X4");
            stringBuilder.AppendLine($"{address} {code}");
        }

        System.IO.File.WriteAllText(filepath, stringBuilder.ToString());
    }
}
