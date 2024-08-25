using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Application.Domain.Service;

public class ParserService(ITokenizer tokenizer) : IParser
{
    public IEnumerable<Instruction> Parse(IEnumerable<TokenList> tokenLists)
    {
        return tokenLists.SelectMany(Parse);
    }

    private IEnumerable<Instruction> Parse(TokenList tokenList)
    {
        for (var i = 0; i < tokenList.Tokens.Count; i++)
        {
            switch (tokenList.Tokens[i])
            {
                case EndToken:
                    yield return new EndInstruction();
                    break;
                case OrgToken:
                    yield return tokenList.GetTokenOption<NumberValueToken>(i + 1)
                        .Map(t => new OrgInstruction(t.Value))
                        .OrElseThrow(new InstructionParseException("org was missing number value"));
                    break;
                case IncludeToken:
                {
                    foreach (var instruction in ParseIncludeInstruction(tokenList, i + 1))
                    {
                        yield return instruction;
                    }

                    break;
                }
            }
        }
    }

    private IEnumerable<Instruction> ParseIncludeInstruction(TokenList tokenList, int index)
    {
        var option = tokenList.GetTokenOption<StringValueToken>(index)
            .Map(t => Parse(tokenizer.Tokenize(t.Value)));

        if (!option.HasValue())
        {
            throw new InstructionParseException("include was missing filepath");
        }

        foreach (var instruction in option.Get())
        {
            yield return instruction;
        }
    }
}
