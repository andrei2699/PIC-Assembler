using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Adapter.Out.File;

public class FileParserAdapter : IParser
{
    private const string CommentLiteral = ";";

    public IEnumerable<Token> Parse(string filepath)
    {
        return GetCleanedLines(filepath)
            .Select(line => new EndToken());
    }

    private static IEnumerable<string> GetCleanedLines(string filepath)
    {
        var readAllText = System.IO.File.ReadAllLines(filepath);

        return readAllText
            .Select(RemoveComment)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0);
    }

    private static string RemoveComment(string line)
    {
        var indexOf = line.IndexOf(CommentLiteral, StringComparison.Ordinal);

        return indexOf >= 0 ? line[..indexOf] : line;
    }
}
