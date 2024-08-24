using System.Text;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Port.Out;
using PIC.Assembler.Common;

namespace PIC.Assembler.Adapter.Out.File;

public class FileParserAdapter : IParser
{
    private const string CommentLiteral = ";";

    public IEnumerable<TokenList> Parse(string filepath)
    {
        return GetCleanedLines(filepath)
            .Select(line => new TokenList(line.Split(" ")
                .Select(ParseTokens)
                .SelectMany(tokenList => tokenList)));
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

    private static IEnumerable<Token> ParseTokens(string token)
    {
        var keywordToken = ParseKeywordToken(token);
        if (keywordToken.HasValue())
        {
            yield return keywordToken.Get();
            yield break;
        }

        var oneOreMultipleCharacterToken = ParseOneOreMultipleCharacterToken(token);
        if (oneOreMultipleCharacterToken.HasValue())
        {
            yield return oneOreMultipleCharacterToken.Get();
            yield break;
        }

        foreach (var individualCharacter in ParseIndividualCharacters(token))
        {
            yield return individualCharacter;
        }
    }

    private static Option<Token> ParseKeywordToken(string token)
    {
        return token switch
        {
            "end" or "END" => Option<Token>.Some(new EndToken()),
            "equ" or "EQU" => Option<Token>.Some(new EquateToken()),
            "#include" or "#INCLUDE" => Option<Token>.Some(new IncludeToken()),
            "org" or "ORG" => Option<Token>.Some(new OrgToken()),
            "#define" or "#DEFINE" => Option<Token>.Some(new DefineToken()),
            "__config" or "__CONFIG" => Option<Token>.Some(new ConfigToken()),
            _ => Option<Token>.None()
        };
    }

    private static Option<Token> ParseOneOreMultipleCharacterToken(string parsedToken)
    {
        return ParseOneCharacterToken(parsedToken)
            .OrElse(ParseMultipleCharacterToken(parsedToken))
            .OrElse(Option<Token>.None());
    }

    private static Option<Token> ParseOneCharacterToken(string token)
    {
        return token switch
        {
            "(" => Option<Token>.Some(new OpenParenthesisToken()),
            ")" => Option<Token>.Some(new ClosedParenthesisToken()),
            "$" => Option<Token>.Some(new DollarToken()),
            "+" => Option<Token>.Some(new PlusToken()),
            "-" => Option<Token>.Some(new MinusToken()),
            "&" => Option<Token>.Some(new AmpersandToken()),
            "|" => Option<Token>.Some(new BarToken()),
            _ => Option<Token>.None()
        };
    }

    private static Option<Token> ParseMultipleCharacterToken(string token)
    {
        return token switch
        {
            ">>" => Option<Token>.Some(new RightShiftToken()),
            "<<" => Option<Token>.Some(new LeftShiftToken()),
            _ when IsLabel(token) => Option<Token>.Some(new LabelToken(token.TrimEnd(':').ToUpperInvariant())),
            _ when IsStringValueToken(token) => Option<Token>.Some(new StringValueToken(token.Trim('"'))),
            _ when IsCharacterValueToken(token) => Option<Token>.Some(new CharacterValueToken(token.Trim('\'')[0])),
            _ when IsDecimalValueToken(token, out var number) => Option<Token>.Some(new DecimalValueToken(number)),
            _ when IsHexadecimalValueToken(token) => Option<Token>.Some(
                new HexadecimalValueToken(Convert.ToInt32(token, 16))),
            _ when IsBinaryValueToken(token) => Option<Token>.Some(
                new BinaryValueToken(Convert.ToInt32(token[..^1], 2))),
            _ => Option<Token>.None()
        };
    }

    private static IEnumerable<Token> ParseIndividualCharacters(string token)
    {
        var builder = new StringBuilder();

        foreach (var @char in token)
        {
            var individualCharacterTokenParsed = false;
            foreach (var individualCharacterToken in ParseIndividualCharacters(@char.ToString(), builder))
            {
                individualCharacterTokenParsed = true;
                yield return individualCharacterToken;
            }

            if (individualCharacterTokenParsed)
            {
                builder = builder.Clear();
                continue;
            }

            builder = builder.Append(@char);

            var multipleCharacterToken = ParseMultipleCharacterToken(builder.ToString());
            if (!multipleCharacterToken.HasValue())
            {
                continue;
            }

            builder = builder.Clear();
            yield return multipleCharacterToken.Get();
        }

        if (builder.Length <= 0)
        {
            yield break;
        }

        var parsedToken = builder.ToString();
        yield return ParseOneOreMultipleCharacterToken(parsedToken)
            .OrElseGet(new NameConstantToken(parsedToken.ToUpperInvariant()));
    }

    private static IEnumerable<Token> ParseIndividualCharacters(string token, StringBuilder builder)
    {
        var oneCharacterToken = ParseOneCharacterToken(token);
        if (!oneCharacterToken.HasValue())
        {
            yield break;
        }

        if (builder.Length > 0)
        {
            foreach (var other in ParseTokens(builder.ToString()))
            {
                yield return other;
            }
        }

        yield return oneCharacterToken.Get();
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
