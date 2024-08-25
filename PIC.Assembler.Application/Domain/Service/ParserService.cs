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
        if (tokenList.Tokens.Count == 0)
        {
            yield break;
        }

        switch (tokenList.Tokens[0])
        {
            case EndToken:
                yield return new EndInstruction();
                break;
            case OrgToken:
                yield return tokenList.GetTokenOption<NumberValueToken>(1)
                    .Map(t => new OrgInstruction(t.Value))
                    .OrElseThrow(new InstructionParseException("org was missing number value"));
                break;
            case LabelToken labelToken:
            {
                foreach (var instruction in ParseLabelInstruction(tokenList, 1, labelToken))
                {
                    yield return instruction;
                }

                break;
            }
            case IncludeToken:
            {
                foreach (var instruction in ParseIncludeInstruction(tokenList, 1))
                {
                    yield return instruction;
                }

                break;
            }
            default:
            {
                throw new InstructionParseException($"{tokenList.Tokens[0]} is not a valid instruction");
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

    private static IEnumerable<Instruction> ParseLabelInstruction(TokenList tokenList, int index, LabelToken labelToken)
    {
        var tokenOption = tokenList.GetTokenOption<Token>(index);
        if (tokenOption.HasValue())
        {
            throw new InstructionParseException("label instruction must be on separate line");
        }

        yield return new LabelInstruction(labelToken.Name);
    }
}
