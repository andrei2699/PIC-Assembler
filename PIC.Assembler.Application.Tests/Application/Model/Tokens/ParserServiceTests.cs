using FluentAssertions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Common;

namespace PIC.Assembler.Application.Tests.Application.Model.Tokens;

public class TokenListTests
{
    private readonly FileInformation _fileInformation = new("file-path", 0);

    #region GetTokenOption

    [Fact]
    public void GivenEmptyList_WhenGetTokenOption_ThenReturnNone()
    {
        var tokenList = new TokenList([]);

        var tokenOption = tokenList.GetTokenOption<Token>(0);

        tokenOption.Should().Be(Option<Token>.None());
    }

    [Fact]
    public void GivenIndexSmallerThan0_WhenGetTokenOption_ThenReturnNone()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation)]);

        var tokenOption = tokenList.GetTokenOption<Token>(-20);

        tokenOption.Should().Be(Option<Token>.None());
    }

    [Fact]
    public void GivenIndexBiggerThanNumberOfElements_WhenGetTokenOption_ThenReturnNone()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation)]);

        var tokenOption = tokenList.GetTokenOption<Token>(40);

        tokenOption.Should().Be(Option<Token>.None());
    }

    [Fact]
    public void GivenElementOfDifferentTypeAtIndex_WhenGetTokenOption_ThenReturnNone()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation)]);

        var tokenOption = tokenList.GetTokenOption<BarToken>(0);

        tokenOption.Should().Be(Option<BarToken>.None());
    }

    [Fact]
    public void GivenElementOfTypeAtIndex_WhenGetTokenOption_ThenReturnSome()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation), new BarToken(_fileInformation)]);

        var tokenOption = tokenList.GetTokenOption<BarToken>(1);

        tokenOption.Should().Be(Option<BarToken>.Some(new BarToken(_fileInformation)));
    }

    #endregion

    #region Slice

    [Fact]
    public void GivenLessThan0_WhenSlice_ThenThrowArgumentOutOfRangeException()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation)]);

        var func = () => tokenList.Slice(-6);

        func.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GivenMoreThanCount_WhenSlice_ThenThrowArgumentException()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation)]);

        var func = () => tokenList.Slice(7);

        func.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Given0_WhenSlice_ThenReturnEntireList()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation), new BarToken(_fileInformation)]);

        var slice = tokenList.Slice(0);

        slice.Should()
            .BeEquivalentTo(new TokenList([new AmpersandToken(_fileInformation), new BarToken(_fileInformation)]));
    }

    [Fact]
    public void GivenOtherIndex_WhenSlice_ThenReturnListStartingWithProvidedIndex()
    {
        var tokenList = new TokenList([
            new AmpersandToken(_fileInformation), new BarToken(_fileInformation), new CommaToken(_fileInformation),
            new DefineToken(_fileInformation), new DollarToken(_fileInformation)
        ]);

        var slice = tokenList.Slice(2);

        slice.Should().BeEquivalentTo(new TokenList([
            new CommaToken(_fileInformation), new DefineToken(_fileInformation), new DollarToken(_fileInformation)
        ]));
    }

    #endregion

    #region Split

    [Fact]
    public void GivenEmptyList_WhenSlice_ThenReturnEmptyList()
    {
        var tokenList = new TokenList([]);

        var split = tokenList.Split(_ => true);

        split.Should().BeEmpty();
    }

    [Fact]
    public void GivenConditionIsAlwaysFalse_WhenSlice_ThenReturnListContainingEntireList()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation), new BarToken(_fileInformation)]);

        var split = tokenList.Split(_ => false);

        split.Should().BeEquivalentTo([
            new TokenList([new AmpersandToken(_fileInformation), new BarToken(_fileInformation)])
        ]);
    }

    [Fact]
    public void GivenConditionIsAlwaysTrue_WhenSlice_ThenReturnListWithEmptyListForEveryElement()
    {
        var tokenList = new TokenList([new AmpersandToken(_fileInformation), new BarToken(_fileInformation)]);

        var split = tokenList.Split(_ => true);

        split.Should().BeEquivalentTo([new TokenList([]), new TokenList([])]);
    }

    [Fact]
    public void GivenListWithCondition_WhenSlice_ThenReturnListWithTwoSubLists()
    {
        var tokenList = new TokenList([
            new AmpersandToken(_fileInformation), new BarToken(_fileInformation), new AmpersandToken(_fileInformation)
        ]);

        var split = tokenList.Split(token => token is BarToken);

        split.Should().BeEquivalentTo([
            new TokenList([new AmpersandToken(_fileInformation)]), new TokenList([new AmpersandToken(_fileInformation)])
        ]);
    }

    [Fact]
    public void GivenListWithCondition_WhenSlice_ThenReturnListOfListsThatMatchTheCondition()
    {
        var tokenList = new TokenList([
            new BarToken(_fileInformation), new AmpersandToken(_fileInformation), new AmpersandToken(_fileInformation),
            new BarToken(_fileInformation), new CommaToken(_fileInformation)
        ]);

        var split = tokenList.Split(token => token is BarToken);

        split.Should().BeEquivalentTo([
            new TokenList([]),
            new TokenList([new AmpersandToken(_fileInformation), new AmpersandToken(_fileInformation)]),
            new TokenList([new CommaToken(_fileInformation)])
        ]);
    }

    #endregion
}
