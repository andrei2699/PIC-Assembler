using PIC.Assembler.Application.Domain.Model.Tokens;

namespace PIC.Assembler.Application.Port.Out;

public interface ITokenizer
{
    IEnumerable<TokenList> Tokenize(string filepath);
}
