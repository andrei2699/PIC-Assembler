using FluentAssertions;
using Moq;
using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Domain.Service;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Application.Tests.Application.Domain.Service;

public class ParserServiceTests
{
    private static readonly FileInformation FileInformation = new("file-path");
    private readonly Mock<ITokenizer> _tokenizerMock = new();
    private readonly ParserService _parserService;

    public ParserServiceTests()
    {
        _parserService = new ParserService(_tokenizerMock.Object, new ArithmeticExpressionParser());
    }

    #region NoTokens

    [Fact]
    public void GivenEmptyTokenList_ThenReturnNoElements()
    {
        var instructions = _parserService.Parse(new List<TokenList>(), new InstructionSet());

        instructions.Should().BeEmpty();
    }

    [Fact]
    public void GivenTokenListWithoutTokens_ThenReturnNoElements()
    {
        var instructions = _parserService.Parse(new List<TokenList> { new([]) }, new InstructionSet());

        instructions.Should().BeEmpty();
    }

    #endregion

    #region Invalid Token Combinations

    [Theory]
    [MemberData(nameof(GetInvalidTokenAtBeginning))]
    public void GivenInvalidTokenAtBeginningOfList_ThenThrowInstructionParseException(Token token)
    {
        var func = () => _parserService.Parse(new List<TokenList> { new([token]) }, new InstructionSet()).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    public static IEnumerable<object[]> GetInvalidTokenAtBeginning()
    {
        yield return [new CharacterValueToken('c', FileInformation)];
        yield return [new NumberValueToken(2, FileInformation)];
        yield return [new StringValueToken("text", FileInformation)];
        yield return [new AmpersandToken(FileInformation)];
        yield return [new BarToken(FileInformation)];
        yield return [new OpenParenthesisToken(FileInformation)];
        yield return [new ClosedParenthesisToken(FileInformation)];
        yield return [new DollarToken(FileInformation)];
        yield return [new LeftShiftToken(FileInformation)];
        yield return [new RightShiftToken(FileInformation)];
        yield return [new MinusToken(FileInformation)];
        yield return [new PlusToken(FileInformation)];
        yield return [new EquateToken(FileInformation)];
    }

    #endregion

    #region EndToken

    [Fact]
    public void GivenTokenListWithEndToken_ThenReturnEndInstruction()
    {
        var instructions = _parserService.Parse(new List<TokenList> { new([new EndToken(FileInformation)]) },
            new InstructionSet());

        instructions.Should().Equal(new EndInstruction());
    }

    #endregion

    #region OrgToken

    [Fact]
    public void GivenTokenListWithOrgTokenWithoutValue_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(new List<TokenList> { new([new OrgToken(FileInformation)]) }, new InstructionSet())
                .ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithOrgTokenWithOtherToken_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(
                new List<TokenList>
                    { new([new OrgToken(FileInformation), new StringValueToken("abc", FileInformation)]) },
                new InstructionSet()).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithOrgTokenWithNumberValue_ThenReturnOrgInstruction()
    {
        var instructions = _parserService.Parse(
            new List<TokenList> { new([new OrgToken(FileInformation), new NumberValueToken(1, FileInformation)]) },
            new InstructionSet());

        instructions.Should().Equal(new OrgInstruction(1));
    }

    #endregion

    #region ConfigToken

    [Fact]
    public void GivenTokenListWithConfigTokenWithoutValue_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(new List<TokenList> { new([new ConfigToken(FileInformation)]) }, new InstructionSet())
                .ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithConfigTokenWithOtherToken_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(
                new List<TokenList>
                    { new([new ConfigToken(FileInformation), new StringValueToken("abc", FileInformation)]) },
                new InstructionSet()).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithConfigTokenWithNumberValue_ThenReturnConfigInstruction()
    {
        var instructions = _parserService.Parse(
            new List<TokenList>
                { new([new ConfigToken(FileInformation), new NumberValueToken(0xFE, FileInformation)]) },
            new InstructionSet());

        instructions.Should().Equal(new ConfigInstruction(0xFE));
    }

    #endregion

    #region IncludeToken

    [Fact]
    public void GivenTokenListWithIncludeTokenWithoutStringValueToken_ThenThrowInstructionParseException()
    {
        var func = () => _parserService
            .Parse(new List<TokenList> { new([new IncludeToken(FileInformation)]) }, new InstructionSet())
            .ToList();

        func.Should().Throw<InstructionParseException>();
    }


    [Fact]
    public void GivenTokenListWithIncludeTokenWithOtherToken_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(
                new List<TokenList>
                    { new([new IncludeToken(FileInformation), new NumberValueToken(4, FileInformation)]) },
                new InstructionSet()).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithIncludeTokenWithStringValueToken_ThenReturnInstructionFromBothFiles()
    {
        var fileInformation = new FileInformation("file.asm");
        _tokenizerMock.Setup(x => x.Tokenize(It.Is<string>(s => s.EndsWith("file.asm"))))
            .Returns(new List<TokenList>
                { new([new OrgToken(fileInformation), new NumberValueToken(4, fileInformation)]) });

        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([new IncludeToken(FileInformation), new StringValueToken("file.asm", FileInformation)]),
                new([new EndToken(FileInformation)])
            },
            new InstructionSet());

        instructions.Should().Equal(new OrgInstruction(4), new EndInstruction());
    }

    [Fact]
    public void
        GivenTokenListWithMultipleIncludeTokenWithStringValueToken_ThenReturnInstructionFromAllFilesInOrderOfInclude()
    {
        var fileInformation1 = new FileInformation("file1.asm");
        var fileInformation2 = new FileInformation("file2.asm");
        var fileInformation3 = new FileInformation("file3.asm");
        _tokenizerMock.Setup(x => x.Tokenize(It.Is<string>(s => s.EndsWith("file1.asm"))))
            .Returns(new List<TokenList>
                { new([new OrgToken(fileInformation1), new NumberValueToken(1, fileInformation1)]) });
        _tokenizerMock.Setup(x => x.Tokenize(It.Is<string>(s => s.EndsWith("file2.asm"))))
            .Returns(new List<TokenList>
                { new([new OrgToken(fileInformation2), new NumberValueToken(2, fileInformation2)]) });
        _tokenizerMock.Setup(x => x.Tokenize(It.Is<string>(s => s.EndsWith("file3.asm"))))
            .Returns(new List<TokenList>
                { new([new OrgToken(fileInformation3), new NumberValueToken(3, fileInformation3)]) });

        var instructions = _parserService.Parse(new List<TokenList>
        {
            new([new IncludeToken(FileInformation), new StringValueToken("file1.asm", FileInformation)]),
            new([new IncludeToken(FileInformation), new StringValueToken("file2.asm", FileInformation)]),
            new([new IncludeToken(FileInformation), new StringValueToken("file3.asm", FileInformation)]),
            new([new OrgToken(FileInformation), new NumberValueToken(4, FileInformation)]),
            new([new EndToken(FileInformation)])
        }, new InstructionSet());

        instructions.Should().Equal(new OrgInstruction(1), new OrgInstruction(2), new OrgInstruction(3),
            new OrgInstruction(4), new EndInstruction());
    }

    [Fact]
    public void
        GivenTokenListWithIncludeTokenWithStringValueTokenInMultipleFiles_ThenReturnInstructionFromAllFilesInOrderOfInclude()
    {
        var fileInformation1 = new FileInformation("file1.asm");
        var fileInformation2 = new FileInformation("file2.asm");
        var fileInformation3 = new FileInformation("file3.asm");
        _tokenizerMock.Setup(x => x.Tokenize(It.Is<string>(s => s.EndsWith("file1.asm"))))
            .Returns(new List<TokenList>
                { new([new OrgToken(fileInformation1), new NumberValueToken(1, fileInformation1)]) });
        _tokenizerMock.Setup(x => x.Tokenize(It.Is<string>(s => s.EndsWith("file2.asm"))))
            .Returns(new List<TokenList>
            {
                new([new IncludeToken(fileInformation2), new StringValueToken("file1.asm", fileInformation2)]),
                new([new OrgToken(fileInformation2), new NumberValueToken(2, fileInformation2)])
            });
        _tokenizerMock.Setup(x => x.Tokenize(It.Is<string>(s => s.EndsWith("file3.asm"))))
            .Returns(new List<TokenList>
            {
                new([new IncludeToken(fileInformation3), new StringValueToken("file2.asm", fileInformation3)]),
                new([new OrgToken(fileInformation3), new NumberValueToken(3, fileInformation3)])
            });

        var instructions = _parserService.Parse(new List<TokenList>
        {
            new([new IncludeToken(FileInformation), new StringValueToken("file3.asm", FileInformation)]),
            new([new OrgToken(FileInformation), new NumberValueToken(4, FileInformation)]),
            new([new EndToken(FileInformation)])
        }, new InstructionSet());

        instructions.Should().Equal(new OrgInstruction(1), new OrgInstruction(2), new OrgInstruction(3),
            new OrgInstruction(4), new EndInstruction());
    }

    #endregion

    #region LabelToken

    [Fact]
    public void GivenTokenListWithLabelTokenWithOtherToken_ThenThrowInstructionParseException()
    {
        var func = () => _parserService
            .Parse(
                new List<TokenList>
                    { new([new LabelToken("Label", FileInformation), new StringValueToken("abc", FileInformation)]) },
                new InstructionSet())
            .ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithLabelTokenWithoutOtherToken_ThenReturnLabelInstruction()
    {
        var instructions =
            _parserService.Parse(new List<TokenList> { new([new LabelToken("Label", FileInformation)]) },
                new InstructionSet());

        instructions.Should().Equal(new LabelInstruction("Label"));
    }

    #endregion

    #region Mnemonics

    [Fact]
    public void GivenTokenListWithNameConstantThatIsNotInInstructionSet_ThenThrowKeyNotFoundException()
    {
        var func = () => _parserService.Parse(new List<TokenList>
                { new([new NameConstantToken("INSTRUCTION", FileInformation)]) },
            new InstructionSet()).ToList();

        func.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void GivenTokenListWithNameConstant_ThenReturnMnemonicWithoutParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
                { new([new NameConstantToken("INSTRUCTION", FileInformation)]) },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 0 } })).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", []), [])]);
    }

    [Fact]
    public void GivenTokenListWithMultipleNameConstant_ThenReturnMnemonicListWithoutParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([new NameConstantToken("INSTRUCTION1", FileInformation)]),
                new([new NameConstantToken("INSTRUCTION2", FileInformation)]),
                new([new NameConstantToken("INSTRUCTION3", FileInformation)]),
            },
            CreateInstructionSet(new Dictionary<string, int>
            {
                { "INSTRUCTION1", 0 },
                { "INSTRUCTION2", 0 },
                { "INSTRUCTION3", 0 },
            })).ToList();

        instructions.Should().BeEquivalentTo([
            new Mnemonic(new InstructionDefinition("INSTRUCTION1", "0", []), []),
            new Mnemonic(new InstructionDefinition("INSTRUCTION2", "0", []), []),
            new Mnemonic(new InstructionDefinition("INSTRUCTION3", "0", []), []),
        ], options => options.RespectingRuntimeTypes());
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndNumberToken_ThenReturnMnemonicWithOneParameter()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([new NameConstantToken("INSTRUCTION", FileInformation), new NumberValueToken(3, FileInformation)])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        instructions.Should()
            .BeEquivalentTo([
                new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0"]), [new MnemonicNumericParameter(3)])
            ]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndNameConstantToken_ThenReturnMnemonicWithOneParameterWithLabel()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation),
                    new NameConstantToken("LABEL", FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        instructions.Should()
            .BeEquivalentTo([
                new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0"]),
                    [new MnemonicLabelParameter("LABEL")])
            ]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndTwoNumberTokensSeparatedByComma_ThenReturnMnemonicWithTwoParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new NumberValueToken(3, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(5, FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 2 } })).ToList();

        instructions.Should().BeEquivalentTo([
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0", "p-1"]),
                [new MnemonicNumericParameter(3), new MnemonicNumericParameter(5)])
        ]);
    }

    [Fact]
    public void
        GivenTokenListWithNameConstantAndMultipleNumberTokensSeparatedByComma_ThenReturnMnemonicWithMultipleParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new NumberValueToken(3, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(5, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(6, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(10, FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 4 } })).ToList();

        instructions.Should().BeEquivalentTo([
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0", "p-1", "p-2", "p-3"]),
            [
                new MnemonicNumericParameter(3), new MnemonicNumericParameter(5), new MnemonicNumericParameter(6),
                new MnemonicNumericParameter(10)
            ])
        ]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndExpression_ThenReturnMnemonicWithOneParameter()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new NumberValueToken(1, FileInformation),
                    new LeftShiftToken(FileInformation), new NumberValueToken(8, FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        instructions.Should()
            .BeEquivalentTo([
                new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0"]),
                    [new MnemonicNumericParameter(1 << 8)])
            ]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndExpressionWithParenthesis_ThenReturnMnemonicWithOneParameter()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new OpenParenthesisToken(FileInformation),
                    new NumberValueToken(1, FileInformation), new LeftShiftToken(FileInformation),
                    new NumberValueToken(8, FileInformation), new ClosedParenthesisToken(FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        instructions.Should()
            .BeEquivalentTo([
                new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0"]),
                    [new MnemonicNumericParameter(1 << 8)])
            ]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndTwoExpressionsSeparatedByComma_ThenReturnMnemonicWithTwoParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new NumberValueToken(6, FileInformation),
                    new BarToken(FileInformation), new NumberValueToken(8, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(5, FileInformation),
                    new RightShiftToken(FileInformation), new NumberValueToken(1, FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 2 } })).ToList();

        instructions.Should().BeEquivalentTo([
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0", "p-1"]),
                [new MnemonicNumericParameter(6 | 8), new MnemonicNumericParameter(5 >> 1)])
        ]);
    }

    [Fact]
    public void
        GivenTokenListWithNameConstantAndMultipleExpressionsSeparatedByComma_ThenReturnMnemonicWithMultipleParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new OpenParenthesisToken(FileInformation),
                    new NumberValueToken(3, FileInformation), new ClosedParenthesisToken(FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(5, FileInformation),
                    new AmpersandToken(FileInformation), new NumberValueToken(3, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(6, FileInformation),
                    new XorToken(FileInformation), new NumberValueToken(7, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(10, FileInformation),
                    new PlusToken(FileInformation), new NumberValueToken(9, FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 4 } })).ToList();

        instructions.Should().BeEquivalentTo([
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0", "p-1", "p-2", "p-3"]),
            [
                new MnemonicNumericParameter(3), new MnemonicNumericParameter(5 & 3),
                new MnemonicNumericParameter(6 ^ 7), new MnemonicNumericParameter(10 + 9)
            ])
        ]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndTwoNumberTokensWithoutComma_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new NumberValueToken(3, FileInformation),
                    new NumberValueToken(5, FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 0 } })).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void
        GivenTokenListWithNameConstantAndMultipleNumberTokensWithoutComma_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION", FileInformation), new NumberValueToken(3, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(5, FileInformation),
                    new CommaToken(FileInformation), new NumberValueToken(6, FileInformation),
                    new NumberValueToken(10, FileInformation)
                ])
            },
            CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    #endregion

    # region Equate Replacements

    [Fact]
    public void GivenTokenListWithOneEquateWithNothingToReplace_ThenReturnEmptyList()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("VARIABLE", FileInformation), new EquateToken(FileInformation),
                    new NumberValueToken(3, FileInformation)
                ])
            },
            new InstructionSet());

        instructions.Should().BeEmpty();
    }

    [Fact]
    public void GivenTokenListWithOneEquate_ThenReturnInstructionListWithReplacedValues()
    {
        var instructions = _parserService.Parse(new List<TokenList>
        {
            new([
                new NameConstantToken("VARIABLE", FileInformation), new EquateToken(FileInformation),
                new NumberValueToken(3, FileInformation)
            ]),
            new([
                new NameConstantToken("INSTRUCTION", FileInformation),
                new NameConstantToken("VARIABLE", FileInformation)
            ])
        }, CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        instructions.Should()
            .BeEquivalentTo([
                new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0"]), [new MnemonicNumericParameter(3)])
            ]);
    }

    [Fact]
    public void GivenTokenListWithOneEquateWithExpression_ThenReturnInstructionListWithReplacedValues()
    {
        var instructions = _parserService.Parse(new List<TokenList>
        {
            new([
                new NameConstantToken("VARIABLE", FileInformation), new EquateToken(FileInformation),
                new OpenParenthesisToken(FileInformation), new NumberValueToken(3, FileInformation),
                new LeftShiftToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]),
            new([
                new NameConstantToken("INSTRUCTION", FileInformation),
                new NameConstantToken("VARIABLE", FileInformation)
            ])
        }, CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        instructions.Should()
            .BeEquivalentTo([
                new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0"]),
                    [new MnemonicNumericParameter(3 << 1)])
            ]);
    }

    [Fact]
    public void GivenTokenListWithEquateWithExpressionsWithUnknownVariable_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList>
        {
            new([
                new NameConstantToken("VAR", FileInformation), new EquateToken(FileInformation),
                new OpenParenthesisToken(FileInformation), new NameConstantToken("UNKNOWN", FileInformation),
                new LeftShiftToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]),
            new([
                new NameConstantToken("INSTRUCTION", FileInformation), new NameConstantToken("VAR", FileInformation)
            ])
        }, CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 1 } })).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithMultipleEquateWithExpressions_ThenReturnInstructionListWithReplacedValues()
    {
        var instructions = _parserService.Parse(new List<TokenList>
        {
            new([
                new NameConstantToken("VAR1", FileInformation), new EquateToken(FileInformation),
                new NumberValueToken(2, FileInformation)
            ]),
            new([
                new NameConstantToken("VAR2", FileInformation), new EquateToken(FileInformation),
                new OpenParenthesisToken(FileInformation), new NameConstantToken("VAR1", FileInformation),
                new LeftShiftToken(FileInformation), new NumberValueToken(1, FileInformation),
                new ClosedParenthesisToken(FileInformation)
            ]),
            new([
                new NameConstantToken("VAR3", FileInformation), new EquateToken(FileInformation),
                new NameConstantToken("VAR1", FileInformation), new PlusToken(FileInformation),
                new NameConstantToken("VAR2", FileInformation)
            ]),
            new([
                new NameConstantToken("INSTRUCTION", FileInformation), new NameConstantToken("VAR1", FileInformation),
                new CommaToken(FileInformation), new NameConstantToken("VAR2", FileInformation),
                new CommaToken(FileInformation), new NameConstantToken("VAR3", FileInformation)
            ])
        }, CreateInstructionSet(new Dictionary<string, int> { { "INSTRUCTION", 3 } })).ToList();

        instructions.Should().BeEquivalentTo([
            new Mnemonic(new InstructionDefinition("INSTRUCTION", "0", ["p-0", "p-1", "p-2"]),
            [
                new MnemonicNumericParameter(2), new MnemonicNumericParameter(2 << 1),
                new MnemonicNumericParameter(2 + (2 << 1))
            ])
        ]);
    }

    #endregion

    private static InstructionSet CreateInstructionSet(Dictionary<string, int> instructions)
    {
        var instructionSet = new InstructionSet();
        foreach (var instruction in instructions)
        {
            var parameterNames = Enumerable.Range(0, instruction.Value).Select(i => $"p-{i}").ToList();
            instructionSet.AddDefinition(new InstructionDefinition(instruction.Key, "0", parameterNames));
        }

        return instructionSet;
    }
}
