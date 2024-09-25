using System.Text;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Operation;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Port.Out;
using PIC.Assembler.Common;

namespace PIC.Assembler.Adapter.Out.File;

public class FileTokenizerAdapter : ITokenizer
{
    private const string CommentLiteral = ";";

    public IEnumerable<TokenList> Tokenize(string filepath)
    {
        var fileInformation = new FileInformation(filepath);
        return GetCleanedLines(filepath)
            .Select(line =>
            {
                var tokens = line.Split(" ")
                    .SelectMany(token => ParseToken(token, fileInformation)).ToList();
                return new TokenList(tokens);
            });
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

    private static IEnumerable<Token> ParseToken(string token, FileInformation fileInformation)
    {
        var keywordToken = ParseKeywordToken(token, fileInformation);
        if (keywordToken.HasValue())
        {
            yield return keywordToken.Get();
            yield break;
        }

        var oneOreMultipleCharacterToken = ParseOneOreMultipleCharacterToken(token, fileInformation);
        if (oneOreMultipleCharacterToken.HasValue())
        {
            yield return oneOreMultipleCharacterToken.Get();
            yield break;
        }

        foreach (var individualCharacter in ParseIndividualCharacters(token, fileInformation))
        {
            yield return individualCharacter;
        }
    }

    private static Option<Token> ParseKeywordToken(string token, FileInformation fileInformation)
    {
        return token switch
        {
            "end" or "END" => Option<Token>.Some(new EndToken(fileInformation)),
            "equ" or "EQU" => Option<Token>.Some(new EquateToken(fileInformation)),
            "#include" or "#INCLUDE" => Option<Token>.Some(new IncludeToken(fileInformation)),
            "org" or "ORG" => Option<Token>.Some(new OrgToken(fileInformation)),
            "#define" or "#DEFINE" => Option<Token>.Some(new DefineToken(fileInformation)),
            "__config" or "__CONFIG" => Option<Token>.Some(new ConfigToken(fileInformation)),
            _ => Option<Token>.None()
        };
    }

    private static Option<Token> ParseOneOreMultipleCharacterToken(string parsedToken, FileInformation fileInformation)
    {
        return ParseOneCharacterToken(parsedToken, fileInformation)
            .OrElse(ParseMultipleCharacterToken(parsedToken, fileInformation))
            .OrElse(Option<Token>.None());
    }

    private static Option<Token> ParseOneCharacterToken(string token, FileInformation fileInformation)
    {
        return token switch
        {
            "(" => Option<Token>.Some(new OpenParenthesisToken(fileInformation)),
            ")" => Option<Token>.Some(new ClosedParenthesisToken(fileInformation)),
            "$" => Option<Token>.Some(new DollarToken(fileInformation)),
            "+" => Option<Token>.Some(new PlusToken(fileInformation)),
            "-" => Option<Token>.Some(new MinusToken(fileInformation)),
            "&" => Option<Token>.Some(new AmpersandToken(fileInformation)),
            "|" => Option<Token>.Some(new BarToken(fileInformation)),
            "^" => Option<Token>.Some(new XorToken(fileInformation)),
            "~" => Option<Token>.Some(new TildaToken(fileInformation)),
            "," => Option<Token>.Some(new CommaToken(fileInformation)),
            _ => Option<Token>.None()
        };
    }

    private static Option<Token> ParseMultipleCharacterToken(string token, FileInformation fileInformation)
    {
        return token switch
        {
            ">>" => Option<Token>.Some(new RightShiftToken(fileInformation)),
            "<<" => Option<Token>.Some(new LeftShiftToken(fileInformation)),
            _ when IsLabel(token) => Option<Token>.Some(new LabelToken(token.TrimEnd(':').ToUpperInvariant(),
                fileInformation)),
            _ when IsStringValueToken(token) => Option<Token>.Some(new StringValueToken(token.Trim('"'),
                fileInformation)),
            _ when IsCharacterValueToken(token) => Option<Token>.Some(new CharacterValueToken(token.Trim('\'')[0],
                fileInformation)),
            _ when IsDecimalValueToken(token, out var number) => Option<Token>.Some(
                new NumberValueToken(number, fileInformation)),
            _ when IsHexadecimalValueToken(token) => Option<Token>.Some(
                new NumberValueToken(Convert.ToInt32(token, 16), fileInformation)),
            _ when IsHexadecimalValueWithPrefixToken(token) => Option<Token>.Some(
                new NumberValueToken(Convert.ToInt32(token[2..^1], 16), fileInformation)),
            _ when IsBinaryValueToken(token) => Option<Token>.Some(
                new NumberValueToken(Convert.ToInt32(token[..^1], 2), fileInformation)),
            _ => Option<Token>.None()
        };
    }

    private static IEnumerable<Token> ParseIndividualCharacters(string token, FileInformation fileInformation)
    {
        var builder = new StringBuilder();

        foreach (var @char in token)
        {
            var individualCharacterTokenParsed = false;
            foreach (var individualCharacterToken in ParseIndividualCharacters(@char.ToString(), builder,
                         fileInformation))
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

            var multipleCharacterToken = ParseMultipleCharacterToken(builder.ToString(), fileInformation);
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
        yield return ParseOneOreMultipleCharacterToken(parsedToken, fileInformation)
            .OrElseGet(new NameConstantToken(parsedToken.ToUpperInvariant(), fileInformation));
    }

    private static IEnumerable<Token> ParseIndividualCharacters(string token, StringBuilder builder,
        FileInformation fileInformation)
    {
        var oneCharacterToken = ParseOneCharacterToken(token, fileInformation);
        if (!oneCharacterToken.HasValue())
        {
            yield break;
        }

        if (builder.Length > 0)
        {
            foreach (var other in ParseToken(builder.ToString(), fileInformation))
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

    private static bool IsHexadecimalValueWithPrefixToken(string token)
    {
        return (token.StartsWith("H'") || token.StartsWith("h'")) && token.EndsWith('\'');
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
