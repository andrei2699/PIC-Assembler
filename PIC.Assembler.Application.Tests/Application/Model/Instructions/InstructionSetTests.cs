using FluentAssertions;
using PIC.Assembler.Application.Domain.Model.Instructions;

namespace PIC.Assembler.Application.Tests.Application.Model.Instructions;

public class InstructionSetTests
{
    private readonly InstructionSet _instructionSet = new();

    [Fact]
    public void GivenNoInstruction_WhenGetDefinition_ThenThrowKeyNotFoundException()
    {
        var func = () => _instructionSet.GetDefinition("missing", 0);

        func.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void GivenOtherName_WhenGetDefinition_ThenThrowKeyNotFoundException()
    {
        _instructionSet.AddDefinition(new InstructionDefinition("instruction", "pattern", []));

        var func = () => _instructionSet.GetDefinition("other", 0);

        func.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void GivenOtherParameterCount_WhenGetDefinition_ThenThrowKeyNotFoundException()
    {
        _instructionSet.AddDefinition(new InstructionDefinition("instruction", "pattern", ["param-1", "param-2"]));

        var func = () => _instructionSet.GetDefinition("instruction", 1);

        func.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void GivenNameAndParameterCount_WhenGetDefinition_ThenReturnDefinition()
    {
        _instructionSet.AddDefinition(new InstructionDefinition("instruction", "pattern", ["param-1", "param-2"]));

        var instruction = _instructionSet.GetDefinition("instruction", 2);

        instruction.Should()
            .BeEquivalentTo(new InstructionDefinition("instruction", "pattern", ["param-1", "param-2"]));
    }
}
