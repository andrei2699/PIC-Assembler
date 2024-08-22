using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Adapter.Out.File;

public class FileParserAdapter : IParser
{
    private const string CommentLiteral = ";";

    public IEnumerable<Token> Parse(string filepath)
    {
        return GetCleanedLines(filepath)
            .SelectMany(line => line.Split(" ").Select(ParseToken));
    }

    private static Token ParseToken(string token)
    {
        return token switch
        {
            "END" => new EndToken(),
            "EQU" => new EquateToken(),
            _ when int.TryParse(token, out var number) => new DecimalValueToken(number),
            _ when token.StartsWith("0X") => new HexadecimalValueToken(Convert.ToInt32(token, 16)),
            _ when token.EndsWith('B') && int.TryParse(token[..^1], out _) => new BinaryValueToken(Convert.ToInt32(
                token[..^1],
                2)),
            _ => new NameConstantToken(token)
        };
    }

    private static IEnumerable<string> GetCleanedLines(string filepath)
    {
        var readAllText = System.IO.File.ReadAllLines(filepath);

        return readAllText
            .Select(RemoveComment)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .Select(line => line.ToUpperInvariant());
    }

    private static string RemoveComment(string line)
    {
        var indexOf = line.IndexOf(CommentLiteral, StringComparison.Ordinal);

        return indexOf >= 0 ? line[..indexOf] : line;
    }
}
