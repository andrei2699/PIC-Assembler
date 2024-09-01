using PIC.Assembler.Common;

namespace PIC.Assembler.Application.Domain.Model.Tokens;

public record TokenList(List<Token> Tokens)
{
    public Option<T> GetTokenOption<T>(int index) where T : Token
    {
        if (index >= 0 && index < Tokens.Count && Tokens[index] is T t)
        {
            return Option<T>.Some(t);
        }

        return Option<T>.None();
    }

    public TokenList Slice(int startIndex)
    {
        return new TokenList(Tokens.Slice(startIndex, Tokens.Count - startIndex));
    }

    public List<TokenList> Split(Func<Token, bool> separatorCondition)
    {
        var result = new List<List<Token>>();
        var currentGroup = new List<Token>();

        foreach (var item in Tokens)
        {
            if (separatorCondition(item))
            {
                result.Add(currentGroup);
                currentGroup = [];
            }
            else
            {
                currentGroup.Add(item);
            }
        }

        if (currentGroup.Count > 0)
        {
            result.Add(currentGroup);
        }

        return result.Select(tokenList => new TokenList(tokenList)).ToList();
    }
}
