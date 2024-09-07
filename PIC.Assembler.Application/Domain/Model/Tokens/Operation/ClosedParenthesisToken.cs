namespace PIC.Assembler.Application.Domain.Model.Tokens.Operation;

public record ClosedParenthesisToken(FileInformation FileInformation) : Token(FileInformation);
