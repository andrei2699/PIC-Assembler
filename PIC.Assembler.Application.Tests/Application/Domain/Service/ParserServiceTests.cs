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
        _parserService = new ParserService(_tokenizerMock.Object);
    }

    #region NoTokens

    [Fact]
    public void GivenEmptyTokenList_ThenReturnNoElements()
    {
        var instructions = _parserService.Parse(new List<TokenList>());

        instructions.Should().BeEmpty();
    }

    [Fact]
    public void GivenTokenListWithoutTokens_ThenReturnNoElements()
    {
        var instructions = _parserService.Parse(new List<TokenList> { new([]) });

        instructions.Should().BeEmpty();
    }

    #endregion

    #region Invalid Token Combinations

    [Theory]
    [MemberData(nameof(GetInvalidTokenAtBeginning))]
    public void GivenInvalidTokenAtBeginningOfList_ThenThrowInstructionParseException(Token token)
    {
        var func = () => _parserService.Parse(new List<TokenList> { new([token]) }).ToList();

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
        var instructions = _parserService.Parse(new List<TokenList> { new([new EndToken()]) });

        instructions.Should().Equal(new EndInstruction());
    }

    #endregion

    #region OrgToken

    [Fact]
    public void GivenTokenListWithOrgTokenWithoutValue_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList> { new([new OrgToken()]) }).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithOrgTokenWithOtherToken_ThenReturnOrgInstruction()
    {
        var func = () =>
            _parserService.Parse(new List<TokenList> { new([new OrgToken(), new StringValueToken("abc")]) }).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithOrgTokenWithNumberValue_ThenReturnOrgInstruction()
    {
        var instructions = _parserService.Parse(new List<TokenList>
            { new([new OrgToken(), new NumberValueToken(1)]) });

        instructions.Should().Equal(new OrgInstruction(1));
    }

    #endregion

    #region IncludeToken

    [Fact]
    public void GivenTokenListWithIncludeTokenWithoutStringValueToken_ThenThrowInstructionParseException()
    {
        var func = () => _parserService.Parse(new List<TokenList> { new([new IncludeToken()]) }).ToList();

        func.Should().Throw<InstructionParseException>();
    }


    [Fact]
    public void GivenTokenListWithIncludeTokenWithOtherToken_ThenThrowInstructionParseException()
    {
        var func = () =>
            _parserService.Parse(new List<TokenList> { new([new IncludeToken(), new NumberValueToken(4)]) }).ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithIncludeTokenWithStringValueToken_ThenReturnInstructionFromBothFiles()
    {
        _tokenizerMock.Setup(x => x.Tokenize("file.asm"))
            .Returns(new List<TokenList> { new([new OrgToken(), new NumberValueToken(4)]) });

        var instructions = _parserService.Parse(new List<TokenList>
            { new([new IncludeToken(), new StringValueToken("file.asm")]), new([new EndToken()]) });

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
        });

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
        });

        instructions.Should().Equal(new OrgInstruction(1), new OrgInstruction(2), new OrgInstruction(3),
            new OrgInstruction(4), new EndInstruction());
    }

    #endregion

    #region LabelToken

    [Fact]
    public void GivenTokenListWithLabelTokenWithOtherToken_ThenThrowInstructionParseException()
    {
        var func = () => _parserService
            .Parse(new List<TokenList> { new([new LabelToken("Label"), new StringValueToken("abc")]) })
            .ToList();

        func.Should().Throw<InstructionParseException>();
    }

    [Fact]
    public void GivenTokenListWithLabelTokenWithoutOtherToken_ThenReturnLabelInstruction()
    {
        var instructions = _parserService.Parse(new List<TokenList> { new([new LabelToken("Label")]) });

        instructions.Should().Equal(new LabelInstruction("Label"));
    }

    #endregion
}
