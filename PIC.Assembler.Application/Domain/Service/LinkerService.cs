using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Application.Domain.Service;

public class LinkerService : ILinker
{
    public IEnumerable<AddressableInstruction> Link(IEnumerable<IInstruction> instructions, int configAddress)
    {
        var instructionList = instructions.ToList();
        if (instructionList.Find(instruction => instruction is EndInstruction) == null)
        {
            throw new MissingEndInstructionException();
        }

        var currentAddress = 0;
        var labelAddresses = new Dictionary<string, int>();
        var addressableInstructions = new List<Func<AddressableInstruction>>();
        AddressableInstruction? configAddressableInstruction = null;

        foreach (var instruction in instructionList.TakeWhile(instruction => instruction is not EndInstruction))
        {
            switch (instruction)
            {
                case LabelInstruction labelInstruction:
                    labelAddresses.Add(labelInstruction.Name, currentAddress);
                    break;
                case Mnemonic mnemonic:
                {
                    var address = currentAddress;
                    addressableInstructions.Add(() =>
                    {
                        var instructionValue = ComputeMnemonicInstructionValue(mnemonic, labelAddresses);
                        return new AddressableInstruction(address, instructionValue);
                    });
                    currentAddress++;
                    break;
                }
                case OrgInstruction orgInstruction:
                    currentAddress = orgInstruction.Value;
                    break;
                case ConfigInstruction configInstruction:
                    configAddressableInstruction = new AddressableInstruction(configAddress, configInstruction.Value);
                    break;
                default:
                    throw new InvalidInstructionException(nameof(instruction));
            }
        }

        var linkedInstructions = addressableInstructions.Select(func => func.Invoke());

        return configAddressableInstruction is not null
            ? linkedInstructions.Concat([configAddressableInstruction, new EndOfFileAddressableInstruction()])
            : linkedInstructions.Concat([new EndOfFileAddressableInstruction()]);
    }

    private static int ComputeMnemonicInstructionValue(Mnemonic mnemonic, Dictionary<string, int> labelAddresses)
    {
        var parameterValues = GetParameterValues(mnemonic, labelAddresses);
        var replacedOpcode = GetReplacedOpcode(mnemonic.Definition, parameterValues);

        return Convert.ToInt32(replacedOpcode, 2);
    }

    private static List<int> GetParameterValues(Mnemonic mnemonic, Dictionary<string, int> labelAddresses)
    {
        return mnemonic.Parameters.Select(parameter =>
        {
            return parameter switch
            {
                MnemonicNumericParameter mnemonicParameter => mnemonicParameter.Value,
                MnemonicLabelParameter mnemonicLabelParameter => labelAddresses[mnemonicLabelParameter.Label],
                _ => 0
            };
        }).ToList();
    }

    private static string GetReplacedOpcode(InstructionDefinition definition, List<int> parameterValues)
    {
        var opcode = definition.OpcodePattern;

        for (var i = 0; i < definition.ParameterNames.Count; i++)
        {
            var parameterName = definition.ParameterNames[i];
            var parameterValue = parameterValues[i];

            while (opcode.Contains(parameterName))
            {
                var valueToReplace = parameterValue & 1;

                var indexBeforeReplacement = opcode.LastIndexOf(parameterName, StringComparison.InvariantCulture);
                var indexAfterReplacement = indexBeforeReplacement + parameterName.Length;
                opcode = opcode[..indexBeforeReplacement] + valueToReplace + opcode[indexAfterReplacement..];

                parameterValue >>= 1;
            }
        }

        return opcode;
    }
}
