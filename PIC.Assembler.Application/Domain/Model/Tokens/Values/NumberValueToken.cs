namespace PIC.Assembler.Application.Domain.Model.Tokens.Values;

public record NumberValueToken(int Value, FileInformation FileInformation) : Token(FileInformation);
