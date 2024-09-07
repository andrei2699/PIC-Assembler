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
        var func = () => _linkerService.Link([], 0x00);

        func.Should().Throw<MissingEndInstructionException>();
    }

    [Fact]
    public void GivenNoEndInstruction_ThenThrowMissingEndInstructionException()
    {
        var func = () => _linkerService.Link([new OrgInstruction(1234)], 0x00);

        func.Should().Throw<MissingEndInstructionException>();
    }

    [Fact]
    public void GivenInvalidInstruction_ThenThrowInvalidInstructionException()
    {
        var func = () => _linkerService.Link([new InvalidInstruction(), new EndInstruction()], 0x00);

        func.Should().Throw<InvalidInstructionException>();
    }

    [Fact]
    public void GivenEndInstruction_ThenReturnEndOfFileInstruction()
    {
        var addressableInstructions = _linkerService.Link([
            new EndInstruction()
        ], 0x00);

        addressableInstructions.Should().BeEquivalentTo([new EndOfFileAddressableInstruction()]);
    }

    [Fact]
    public void GivenLabelInstruction_ThenReturnEndOfFileInstruction()
    {
        var addressableInstructions = _linkerService.Link([
            new LabelInstruction("LABEL"),
            new EndInstruction()
        ], 0x00);

        addressableInstructions.Should().BeEquivalentTo([new EndOfFileAddressableInstruction()]);
    }

    [Fact]
    public void GivenMnemonicInstruction_ThenReturnInstructionAtAddress0()
    {
        var addressableInstructions = _linkerService.Link([
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "1111", []), []),
            new EndInstruction()
        ], 0x00);

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
        ], 0x00);

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
        ], 0x00);

        addressableInstructions.Should().BeEquivalentTo([
            new AddressableInstruction(0x22, 0xF), new AddressableInstruction(0x33, 0x5),
            new EndOfFileAddressableInstruction()
        ]);
    }

    [Fact]
    public void GivenConfigInstruction_ThenReturnInstructionAtAddressSpecifiedByMicrocontrollerConfig()
    {
        var addressableInstructions = _linkerService.Link([
            new ConfigInstruction(0xFB),
            new EndInstruction()
        ], 0x11);

        addressableInstructions.Should()
            .BeEquivalentTo([new AddressableInstruction(0x11, 0xFB), new EndOfFileAddressableInstruction()]);
    }

    [Fact]
    public void
        GivenConfigInstructionAndMnemonicInstruction_ThenReturnInstructionAtAddressSpecifiedByMicrocontrollerConfig()
    {
        var addressableInstructions = _linkerService.Link([
            new ConfigInstruction(0x66),
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "1111", []), []),
            new EndInstruction()
        ], 0x22);

        addressableInstructions.Should()
            .BeEquivalentTo([
                new AddressableInstruction(0, 0xF), new AddressableInstruction(0x22, 0x66),
                new EndOfFileAddressableInstruction()
            ]);
    }

    [Fact]
    public void
        GivenMultipleOrgInstructionAndMnemonicInstruction_ThenReturnLastOccurrenceInstructionAtAddressSpecifiedMicrocontrollerConfig()
    {
        var addressableInstructions = _linkerService.Link([
            new ConfigInstruction(0x22),
            new Mnemonic(new InstructionDefinition("INSTRUCTION1", "1111", []), []),
            new ConfigInstruction(0x33),
            new Mnemonic(new InstructionDefinition("INSTRUCTION2", "0101", []), []),
            new EndInstruction()
        ], 0x11);

        addressableInstructions.Should().BeEquivalentTo([
            new AddressableInstruction(0x00, 0xF), new AddressableInstruction(0x01, 0x5),
            new AddressableInstruction(0x11, 0x33), new EndOfFileAddressableInstruction()
        ]);
    }
}
