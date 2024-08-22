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
            "end" => new EndToken(),
            "END" => new EndToken(),
            "equ" => new EquateToken(),
            "EQU" => new EquateToken(),
            "#include" => new IncludeToken(),
            "#INCLUDE" => new IncludeToken(),
            "org" => new OrgToken(),
            "ORG" => new OrgToken(),
            "#define" => new DefineToken(),
            "#DEFINE" => new DefineToken(),
            "__config" => new ConfigToken(),
            "__CONFIG" => new ConfigToken(),
            _ when IsStringValueToken(token) => new StringValueToken(token.Trim('"')),
            _ when IsCharacterValueToken(token) => new CharacterValueToken(token.Trim('\'')[0]),
            _ when IsDecimalValueToken(token, out var number) => new DecimalValueToken(number),
            _ when IsHexadecimalValueToken(token) => new HexadecimalValueToken(Convert.ToInt32(token, 16)),
            _ when IsBinaryValueToken(token) => new BinaryValueToken(Convert.ToInt32(token[..^1], 2)),
            _ => new NameConstantToken(token.ToUpperInvariant())
        };
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

    private static bool IsBinaryValueToken(string token)
    {
        return (token.EndsWith('b') || token.EndsWith('B')) && int.TryParse(token[..^1], out _);
    }

    private static bool IsHexadecimalValueToken(string token)
    {
        return token.StartsWith("0x");
    }

    private static bool IsDecimalValueToken(string token, out int number)
    {
        return int.TryParse(token, out number);
    }

    private static bool IsStringValueToken(string token)
    {
        return token.StartsWith('"') && token.EndsWith('"');
    }

    private static bool IsCharacterValueToken(string token)
    {
        return token.StartsWith('\'') && token.EndsWith('\'');
    }
}
