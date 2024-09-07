namespace PIC.Assembler.Application.Domain.Model.Tokens.Operation;

public record OpenParenthesisToken(FileInformation FileInformation) : Token(FileInformation);
