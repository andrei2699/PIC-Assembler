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
        yield return [new CharacterValueToken('c')];
        yield return [new NumberValueToken(2)];
        yield return [new StringValueToken("text")];
        yield return [new AmpersandToken()];
        yield return [new BarToken()];
        yield return [new OpenParenthesisToken()];
        yield return [new ClosedParenthesisToken()];
        yield return [new DollarToken()];
        yield return [new LeftShiftToken()];
        yield return [new RightShiftToken()];
        yield return [new MinusToken()];
        yield return [new PlusToken()];
        yield return [new EquateToken()];
    }

    #endregion

    #region EndToken

    [Fact]
    public void GivenTokenListWithEndToken_ThenReturnEndInstruction()
    {
        var instructions = _parserService.Parse(new List<TokenList> { new([new EndToken()]) }, new InstructionSet());

        instructions.Should().Equal(new EndInstruction());
    }

    #endregion

    #region OrgToken

    [Fact]
    public void GivenTokenListWithOrgTokenWithoutValue_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(new List<TokenList> { new([new OrgToken()]) }, new InstructionSet()).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithOrgTokenWithOtherToken_ThenReturnOrgInstruction()
    {
        var func = () =>
            _parserService.Parse(new List<TokenList> { new([new OrgToken(), new StringValueToken("abc")]) },
                new InstructionSet()).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithOrgTokenWithNumberValue_ThenReturnOrgInstruction()
    {
        var instructions = _parserService.Parse(new List<TokenList> { new([new OrgToken(), new NumberValueToken(1)]) },
            new InstructionSet());

        instructions.Should().Equal(new OrgInstruction(1));
    }

    #endregion

    #region IncludeToken

    [Fact]
    public void GivenTokenListWithIncludeTokenWithoutStringValueToken_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList> { new([new IncludeToken()]) }, new InstructionSet())
            .ToList();

        func.Should().Throw<InstructionParseException>();
    }


    [Fact]
    public void GivenTokenListWithIncludeTokenWithOtherToken_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(new List<TokenList> { new([new IncludeToken(), new NumberValueToken(4)]) },
                new InstructionSet()).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithIncludeTokenWithStringValueToken_ThenReturnInstructionFromBothFiles()
    {
        _tokenizerMock.Setup(x => x.Tokenize("file.asm"))
            .Returns(new List<TokenList> { new([new OrgToken(), new NumberValueToken(4)]) });

        var instructions = _parserService.Parse(new List<TokenList>
                { new([new IncludeToken(), new StringValueToken("file.asm")]), new([new EndToken()]) },
            new InstructionSet());

        instructions.Should().Equal(new OrgInstruction(4), new EndInstruction());
    }

    [Fact]
    public void
        GivenTokenListWithMultipleIncludeTokenWithStringValueToken_ThenReturnInstructionFromAllFilesInOrderOfInclude()
    {
        _tokenizerMock.Setup(x => x.Tokenize("file1.asm"))
            .Returns(new List<TokenList> { new([new OrgToken(), new NumberValueToken(1)]) });
        _tokenizerMock.Setup(x => x.Tokenize("file2.asm"))
            .Returns(new List<TokenList> { new([new OrgToken(), new NumberValueToken(2)]) });
        _tokenizerMock.Setup(x => x.Tokenize("file3.asm"))
            .Returns(new List<TokenList> { new([new OrgToken(), new NumberValueToken(3)]) });

        var instructions = _parserService.Parse(new List<TokenList>
        {
            new([new IncludeToken(), new StringValueToken("file1.asm")]),
            new([new IncludeToken(), new StringValueToken("file2.asm")]),
            new([new IncludeToken(), new StringValueToken("file3.asm")]),
            new([new OrgToken(), new NumberValueToken(4)]),
            new([new EndToken()])
        }, new InstructionSet());

        instructions.Should().Equal(new OrgInstruction(1), new OrgInstruction(2), new OrgInstruction(3),
            new OrgInstruction(4), new EndInstruction());
    }

    [Fact]
    public void
        GivenTokenListWithIncludeTokenWithStringValueTokenInMultipleFiles_ThenReturnInstructionFromAllFilesInOrderOfInclude()
    {
        _tokenizerMock.Setup(x => x.Tokenize("file1.asm"))
            .Returns(new List<TokenList> { new([new OrgToken(), new NumberValueToken(1)]) });
        _tokenizerMock.Setup(x => x.Tokenize("file2.asm"))
            .Returns(new List<TokenList>
            {
                new([new IncludeToken(), new StringValueToken("file1.asm")]),
                new([new OrgToken(), new NumberValueToken(2)])
            });
        _tokenizerMock.Setup(x => x.Tokenize("file3.asm"))
            .Returns(new List<TokenList>
            {
                new([new IncludeToken(), new StringValueToken("file2.asm")]),
                new([new OrgToken(), new NumberValueToken(3)])
            });

        var instructions = _parserService.Parse(new List<TokenList>
        {
            new([new IncludeToken(), new StringValueToken("file3.asm")]),
            new([new OrgToken(), new NumberValueToken(4)]),
            new([new EndToken()])
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
            .Parse(new List<TokenList> { new([new LabelToken("Label"), new StringValueToken("abc")]) },
                new InstructionSet())
            .ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithLabelTokenWithoutOtherToken_ThenReturnLabelInstruction()
    {
        var instructions =
            _parserService.Parse(new List<TokenList> { new([new LabelToken("Label")]) }, new InstructionSet());

        instructions.Should().Equal(new LabelInstruction("Label"));
    }

    #endregion

    #region Mnemonics

    [Fact]
    public void GivenTokenListWithNameConstantThatIsNotInInstructionSet_ThenThrowException()
    {
        var instructions = _parserService.Parse(new List<TokenList>
                { new([new NameConstantToken("INSTRUCTION")]) },
            new InstructionSet());

        instructions.Should().BeEmpty();
    }

    [Fact]
    public void GivenTokenListWithNameConstant_ThenReturnMnemonicWithoutParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
                { new([new NameConstantToken("INSTRUCTION")]) },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [])]);
    }

    [Fact]
    public void GivenTokenListWithMultipleNameConstant_ThenReturnMnemonicListWithoutParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([new NameConstantToken("INSTRUCTION1")]),
                new([new NameConstantToken("INSTRUCTION2")]),
                new([new NameConstantToken("INSTRUCTION3")]),
            },
            new InstructionSet
            {
                { "INSTRUCTION1", "00" },
                { "INSTRUCTION2", "01" },
                { "INSTRUCTION3", "11" },
            }).ToList();

        instructions.Should().BeEquivalentTo([
            new Mnemonic("INSTRUCTION1", "00", []), new Mnemonic("INSTRUCTION2", "01", []),
            new Mnemonic("INSTRUCTION3", "11", []),
        ], options => options.RespectingRuntimeTypes());
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndNumberToken_ThenReturnMnemonicWithOneParameter()
    {
        var instructions = _parserService.Parse(new List<TokenList>
                { new([new NameConstantToken("INSTRUCTION"), new NumberValueToken(3)]) },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [3])]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndTwoNumberTokensSeparatedByComma_ThenReturnMnemonicWithTwoParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new NumberValueToken(3), new CommaToken(),
                    new NumberValueToken(5)
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [3, 5])]);
    }

    [Fact]
    public void
        GivenTokenListWithNameConstantAndMultipleNumberTokensSeparatedByComma_ThenReturnMnemonicWithMultipleParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new NumberValueToken(3), new CommaToken(),
                    new NumberValueToken(5), new CommaToken(), new NumberValueToken(6), new CommaToken(),
                    new NumberValueToken(10)
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [3, 5, 6, 10])]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndExpression_ThenReturnMnemonicWithOneParameter()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new NumberValueToken(1), new LeftShiftToken(),
                    new NumberValueToken(8)
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [1 << 8])]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndExpressionWithParenthesis_ThenReturnMnemonicWithOneParameter()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new OpenParenthesisToken(), new NumberValueToken(1),
                    new LeftShiftToken(), new NumberValueToken(8), new ClosedParenthesisToken()
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [1 << 8])]);
    }


    [Fact]
    public void GivenTokenListWithNameConstantAndTwoExpressionsSeparatedByComma_ThenReturnMnemonicWithTwoParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new NumberValueToken(6), new BarToken(),
                    new NumberValueToken(8), new CommaToken(),
                    new NumberValueToken(5), new RightShiftToken(), new NumberValueToken(1)
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [6 | 8, 5 >> 1])]);
    }

    [Fact]
    public void
        GivenTokenListWithNameConstantAndMultipleExpressionsSeparatedByComma_ThenReturnMnemonicWithMultipleParameters()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new OpenParenthesisToken(), new NumberValueToken(3),
                    new ClosedParenthesisToken(), new CommaToken(), new NumberValueToken(5), new AmpersandToken(),
                    new NumberValueToken(3), new CommaToken(), new NumberValueToken(6), new XorToken(),
                    new NumberValueToken(7), new CommaToken(), new NumberValueToken(10), new PlusToken(),
                    new NumberValueToken(9)
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        instructions.Should().BeEquivalentTo([new Mnemonic("INSTRUCTION", "0", [3, 5 & 3, 6 ^ 7, 10 + 9])]);
    }

    [Fact]
    public void GivenTokenListWithNameConstantAndTwoNumberTokensWithoutComma_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new NumberValueToken(3), new NumberValueToken(5)
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void
        GivenTokenListWithNameConstantAndMultipleNumberTokensWithoutComma_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList>
            {
                new([
                    new NameConstantToken("INSTRUCTION"), new NumberValueToken(3), new CommaToken(),
                    new NumberValueToken(5), new CommaToken(), new NumberValueToken(6), new NumberValueToken(10)
                ])
            },
            new InstructionSet
            {
                { "INSTRUCTION", "0" }
            }).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    #endregion
}
