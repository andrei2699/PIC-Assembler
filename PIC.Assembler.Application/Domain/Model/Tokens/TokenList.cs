using PIC.Assembler.Common;

namespace PIC.Assembler.Application.Domain.Model.Tokens;

public record TokenList(List<Token> Tokens)
{
    public Option<T> GetTokenOption<T>(int index) where T : Token
    {
        if (index < Tokens.Count && Tokens[index] is T t)
        {
            return Option<T>.Some(t);
        }

        return Option<T>.None();
    }
}
