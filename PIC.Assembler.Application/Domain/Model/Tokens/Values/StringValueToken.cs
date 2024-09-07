namespace PIC.Assembler.Application.Domain.Model.Tokens.Values;

public record StringValueToken(string Value, FileInformation FileInformation) : Token(FileInformation);
