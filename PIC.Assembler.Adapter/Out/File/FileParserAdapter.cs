using System.Text;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Adapter.Out.File;

public class FileParserAdapter : IParser
{
    private const string CommentLiteral = ";";

    public IEnumerable<Token> Parse(string filepath)
    {
        return GetCleanedLines(filepath)
            .SelectMany(line => line.Split(" ").Select(ParseTokens))
            .SelectMany(tokenList => tokenList);
    }

    private static IEnumerable<Token> ParseTokens(string token)
    {
        switch (token)
        {
            case "end":
            case "END":
                yield return new EndToken();
                break;
            case "equ":
            case "EQU":
                yield return new EquateToken();
                break;
            case "#include":
            case "#INCLUDE":
                yield return new IncludeToken();
                break;
            case "org":
            case "ORG":
                yield return new OrgToken();
                break;
            case "#define":
            case "#DEFINE":
                yield return new DefineToken();
                break;
            case "__config":
            case "__CONFIG":
                yield return new ConfigToken();
                break;
            case "(":
                yield return new OpenParenthesisToken();
                break;
            case ")":
                yield return new ClosedParenthesisToken();
                break;
            case "$":
                yield return new DollarToken();
                break;
            case "+":
                yield return new PlusToken();
                break;
            case "-":
                yield return new MinusToken();
                break;
            case ">>":
                yield return new RightShiftToken();
                break;
            case "<<":
                yield return new LeftShiftToken();
                break;
            case "&":
                yield return new AmpersandToken();
                break;
            case "|":
                yield return new BarToken();
                break;
            case var _ when IsLabel(token):
                yield return new LabelToken(token.TrimEnd(':').ToUpperInvariant());
                break;
            case var _ when IsStringValueToken(token):
                yield return new StringValueToken(token.Trim('"'));
                break;
            case var _ when IsCharacterValueToken(token):
                yield return new CharacterValueToken(token.Trim('\'')[0]);
                break;
            case var _ when IsDecimalValueToken(token, out var number):
                yield return new DecimalValueToken(number);
                break;
            case var _ when IsHexadecimalValueToken(token):
                yield return new HexadecimalValueToken(Convert.ToInt32(token, 16));
                break;
            case var _ when IsBinaryValueToken(token):
                yield return new BinaryValueToken(Convert.ToInt32(token[..^1], 2));
                break;
            default:
                foreach (var individualCharacter in ParseIndividualCharacters(token))
                {
                    yield return individualCharacter;
                }

                break;
        }
    }

    private static IEnumerable<Token> ParseIndividualCharacters(string token)
    {
        var builder = new StringBuilder();

        foreach (var @char in token)
        {
            switch (@char)
            {
                case '(':
                case ')':
                case '$':
                case '+':
                case '-':
                case '&':
                case '|':
                    if (builder.Length > 0)
                    {
                        foreach (var other in ParseTokens(builder.ToString()))
                        {
                            yield return other;
                        }
                    }

                    foreach (var other in ParseTokens(@char.ToString()))
                    {
                        yield return other;
                    }

                    builder = builder.Clear();

                    break;

                default:
                    builder = builder.Append(@char);
                    var parsedToken = builder.ToString();

                    switch (parsedToken)
                    {
                        case ">>":
                            builder = builder.Clear();
                            yield return new RightShiftToken();
                            break;
                        case "<<":
                            builder = builder.Clear();
                            yield return new LeftShiftToken();
                            break;
                        case var _ when IsStringValueToken(parsedToken):
                            builder = builder.Clear();
                            yield return new StringValueToken(parsedToken.Trim('"'));
                            break;
                        case var _ when IsCharacterValueToken(parsedToken):
                            builder = builder.Clear();
                            yield return new CharacterValueToken(parsedToken.Trim('\'')[0]);
                            break;
                        case var _ when IsDecimalValueToken(parsedToken, out var number):
                            builder = builder.Clear();
                            yield return new DecimalValueToken(number);
                            break;
                        case var _ when IsHexadecimalValueToken(parsedToken):
                            builder = builder.Clear();
                            yield return new HexadecimalValueToken(Convert.ToInt32(parsedToken, 16));
                            break;
                        case var _ when IsBinaryValueToken(parsedToken):
                            builder = builder.Clear();
                            yield return new BinaryValueToken(Convert.ToInt32(parsedToken[..^1], 2));
                            break;
                    }

                    break;
            }
        }

        if (builder.Length > 0)
        {
            var parsedToken = builder.ToString();
            yield return parsedToken switch
            {
                _ when IsStringValueToken(parsedToken) => new StringValueToken(parsedToken.Trim('"')),
                _ when IsCharacterValueToken(parsedToken) => new CharacterValueToken(parsedToken.Trim('\'')[0]),
                _ when IsDecimalValueToken(parsedToken, out var number) => new DecimalValueToken(number),
                _ when IsHexadecimalValueToken(parsedToken) => new HexadecimalValueToken(Convert.ToInt32(parsedToken, 16)),
                _ when IsBinaryValueToken(parsedToken) => new BinaryValueToken(Convert.ToInt32(parsedToken[..^1], 2)),
                _ => new NameConstantToken(parsedToken.ToUpperInvariant())
            };
        }
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

    private static bool IsLabel(string token)
    {
        return token.EndsWith(':');
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
