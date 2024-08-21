using PIC.Assembler.Application.Domain.Model.Tokens;

namespace PIC.Assembler.Application.Port.Out;

public interface IParser
{
    IEnumerable<Token> Parse(string filepath);
}
