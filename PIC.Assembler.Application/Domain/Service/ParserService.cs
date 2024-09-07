using PIC.Assembler.Application.Domain.Model.Instructions;
using PIC.Assembler.Application.Domain.Model.Tokens;
using PIC.Assembler.Application.Domain.Model.Tokens.Values;
using PIC.Assembler.Application.Port.Out;

namespace PIC.Assembler.Application.Domain.Service;

public class ParserService(ITokenizer tokenizer, ArithmeticExpressionParser arithmeticExpressionParser) : IParser
{
    public IEnumerable<IInstruction> Parse(IEnumerable<TokenList> tokenLists, InstructionSet instructionSet)
    {
        var variables = new Dictionary<string, int>();

        return tokenLists.Where(l => l.Tokens.Count > 0).SelectMany(list => Parse(list, instructionSet, variables));
    }

    private IEnumerable<IInstruction> Parse(TokenList tokenList, InstructionSet instructionSet,
        Dictionary<string, int> variables)
    {
        switch (tokenList.Tokens[0])
        {
            case EndToken:
                yield return new EndInstruction();
                break;
            case OrgToken:
                yield return tokenList.GetTokenOption<NumberValueToken>(1)
                    .Map(t => new OrgInstruction(t.Value))
                    .OrElseThrow(new InstructionParseException("org was missing number value"));
                break;
            case LabelToken labelToken:
            {
                foreach (var instruction in ParseLabelInstruction(tokenList, labelToken))
                {
                    yield return instruction;
                }

                break;
            }
            case IncludeToken:
            {
                foreach (var instruction in ParseIncludeInstruction(tokenList, instructionSet))
                {
                    yield return instruction;
                }

                break;
            }
            case NameConstantToken nameConstantToken:
            {
                foreach (var instruction in ParseNameConstant(nameConstantToken, tokenList, instructionSet, variables))
                {
                    yield return instruction;
                }

                break;
            }
            default:
                throw new InstructionParseException($"{tokenList.Tokens[0]} is not a valid instruction");
        }
    }

    private IEnumerable<IInstruction> ParseIncludeInstruction(TokenList tokenList, InstructionSet instructionSet)
    {
        var option = tokenList.GetTokenOption<StringValueToken>(1)
            .Map(t => Parse(tokenizer.Tokenize(t.Value), instructionSet));

        if (!option.HasValue())
        {
            throw new InstructionParseException("include was missing filepath");
        }

        foreach (var instruction in option.Get())
        {
            yield return instruction;
        }
    }

    private static IEnumerable<IInstruction> ParseLabelInstruction(TokenList tokenList, LabelToken labelToken)
    {
        var tokenOption = tokenList.GetTokenOption<Token>(1);
        if (tokenOption.HasValue())
        {
            throw new InstructionParseException("label instruction must be on separate line");
        }

        yield return new LabelInstruction(labelToken.Name);
    }

    private IEnumerable<IInstruction> ParseNameConstant(NameConstantToken nameConstantToken, TokenList tokenList,
        InstructionSet instructionSet, Dictionary<string, int> variables)
    {
        if (tokenList.GetTokenOption<EquateToken>(1).HasValue())
        {
            var tokenListValue = new TokenList(tokenList.Slice(2).Tokens
                .Select(token => ReplaceNameConstantWithVariableValue(token, variables))
                .ToList());
            var expression = arithmeticExpressionParser.Parse(tokenListValue);

            variables[nameConstantToken.Name] = expression.Evaluate();

            yield break;
        }

        var parameterTokenLists = tokenList.Slice(1).Split(t => t is CommaToken)
            .Select(list =>
                new TokenList(list.Tokens.Select(token => ReplaceNameConstantWithVariableValue(token, variables))
                    .ToList())).ToList();
        var mnemonicParameters = GetMnemonicParameters(parameterTokenLists);

        var instructionDefinition = instructionSet.GetDefinition(nameConstantToken.Name, mnemonicParameters.Count);

        yield return new Mnemonic(instructionDefinition, mnemonicParameters);
    }

    private List<IMnemonicParameter> GetMnemonicParameters(List<TokenList> parameterTokenLists)
    {
        return parameterTokenLists.Select<TokenList, IMnemonicParameter>(list =>
        {
            var nameConstant = list.GetTokenOption<NameConstantToken>(0);
            if (nameConstant.HasValue())
            {
                return new MnemonicLabelParameter(nameConstant.Get().Name);
            }

            return new MnemonicNumericParameter(arithmeticExpressionParser.Parse(list).Evaluate());
        }).ToList();
    }

    private static Token ReplaceNameConstantWithVariableValue(Token token, Dictionary<string, int> variables)
    {
        if (token is NameConstantToken nameConstantToken &&
            variables.TryGetValue(nameConstantToken.Name, out var value))
        {
            return new NumberValueToken(value, nameConstantToken.FileInformation);
        }

        return token;
    }
}
