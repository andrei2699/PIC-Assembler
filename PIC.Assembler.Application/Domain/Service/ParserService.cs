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
            if (tokenList.Tokens[i] is EndToken)
            {
                yield return new EndInstruction();
            }
            else if (tokenList.Tokens[i] is OrgToken)
            {
                if (i + 1 < tokenList.Tokens.Count &&
                    tokenList.Tokens[i + 1] is NumberValueToken numberValueToken)
                {
                    yield return new OrgInstruction(numberValueToken.Value);
                }
                else
                {
                    throw new InstructionParseException("org was missing number value");
                }
            }
            else if (tokenList.Tokens[i] is IncludeToken)
            {
                if (i + 1 < tokenList.Tokens.Count &&
                    tokenList.Tokens[i + 1] is StringValueToken stringValueToken)
                {
                    foreach (var instruction in Parse(tokenizer.Tokenize(stringValueToken.Value)))
                    {
                        yield return instruction;
                    }
                }
                else
                {
                    throw new InstructionParseException("include was missing filepath");
                }
            }
        }
    }
}
