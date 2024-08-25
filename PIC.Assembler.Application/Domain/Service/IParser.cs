using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;

namespace PIC.Assembler.Application.Domain.Service;

public interface IParser
{
    IEnumerable<Instruction> Parse(IEnumerable<TokenList> tokenLists);
}
