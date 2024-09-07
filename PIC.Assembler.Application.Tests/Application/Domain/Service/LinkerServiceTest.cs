using FluentAssertions;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Service;

namespace PIC.Assembler.Application.Tests.Application.Domain.Service;

file record InvalidInstruction : IInstruction;

public class LinkerServiceTest
{
    private readonly LinkerService _linkerService = new();

    [Fact]
    public void GivenNoInstructions_ThenThrowMissingEndInstructionException()
    {
        var func = () => _linkerService.Link([]);

        func.Should().Throw<MissingEndInstructionException>();
    }

    [Fact]
    public void GivenNoEndInstruction_ThenThrowMissingEndInstructionException()
    {
        var func = () => _linkerService.Link([new OrgInstruction(1234)]);

        func.Should().Throw<MissingEndInstructionException>();
    }

    [Fact]
    public void GivenInvalidInstruction_ThenThrowInvalidInstructionException()
    {
        var func = () => _linkerService.Link([new InvalidInstruction(), new EndInstruction()]);

        func.Should().Throw<InvalidInstructionException>();
    }

    [Fact]
    public void GivenEndInstruction_ThenReturnEndOfFileInstruction()
    {
        var addressableInstructions = _linkerService.Link([
            new EndInstruction()
        ]);

        addressableInstructions.Should().BeEquivalentTo([new EndOfFileAddressableInstruction()]);
    }

    [Fact]
    public void GivenLabelInstruction_ThenReturnEndOfFileInstruction()
    {
        var addressableInstructions = _linkerService.Link([
            new LabelInstruction("LABEL"),
            new EndInstruction()
        ]);

        addressableInstructions.Should().BeEquivalentTo([new EndOfFileAddressableInstruction()]);
    }

    [Fact]
    public void GivenMnemonicInstruction_ThenReturnInstructionAtAddress0()
    {
        var addressableInstructions = _linkerService.Link([
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "1111", []), []),
            new EndInstruction()
        ]);

        addressableInstructions.Should().BeEquivalentTo([
            new AddressableInstruction(0, 0xF),
            new EndOfFileAddressableInstruction()
        ]);
    }

    [Fact]
    public void GivenOrgInstructionAndMnemonicInstruction_ThenReturnInstructionAtAddressSpecifiedByOrg()
    {
        var addressableInstructions = _linkerService.Link([
            new OrgInstruction(0x66),
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "1111", []), []),
            new EndInstruction()
        ]);

        addressableInstructions.Should()
            .BeEquivalentTo([new AddressableInstruction(0x66, 0xF), new EndOfFileAddressableInstruction()]);
    }

    [Fact]
    public void GivenMultipleOrgInstructionAndMnemonicInstruction_ThenReturnInstructionAtAddressSpecifiedByOrg()
    {
        var addressableInstructions = _linkerService.Link([
            new OrgInstruction(0x22),
            new Mnemonic(new InstructionDefinition("INSTRUCTION1", "1111", []), []),
            new OrgInstruction(0x33),
            new Mnemonic(new InstructionDefinition("INSTRUCTION2", "0101", []), []),
            new EndInstruction()
        ]);

        addressableInstructions.Should().BeEquivalentTo([
            new AddressableInstruction(0x22, 0xF), new AddressableInstruction(0x33, 0x5),
            new EndOfFileAddressableInstruction()
        ]);
    }
}
