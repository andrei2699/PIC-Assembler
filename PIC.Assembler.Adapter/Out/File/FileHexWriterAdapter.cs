using System.Text;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Adapter.Out.File;

public class FileHexWriterAdapter : IHexWriter
{
    private const string DataRecordType = "00";
    private const string ByteCount = "02";

    public void Write(IEnumerable<AddressableInstruction> instructions, string filepath)
    {
        var stringBuilder = new StringBuilder();

        foreach (var instruction in instructions)
        {
            if (instruction is EndOfFileAddressableInstruction)
            {
                stringBuilder.AppendLine(":00000001FF");
                break;
            }

            var address = instruction.Address.ToString("X4");
            var data = instruction.Data.ToString("X2");
            var checksum = CalculateCheckSum(instruction.Address, instruction.Data).ToString("X2");
            stringBuilder.AppendLine($":{ByteCount}{address}{DataRecordType}{data}{checksum}");
        }

        System.IO.File.WriteAllText(filepath, stringBuilder.ToString());
    }

    private static byte CalculateCheckSum(int address, int data)
    {
        var sum = 2 + address + 0 + data;
        var lsb = sum & 0xFF;

        return (byte)(~lsb + 1);
    }
}
