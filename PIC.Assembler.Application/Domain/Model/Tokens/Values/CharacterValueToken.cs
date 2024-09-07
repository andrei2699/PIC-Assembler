namespace PIC.Assembler.Application.Domain.Model.Tokens.Values;

public record CharacterValueToken(char Value, FileInformation FileInformation) : Token(FileInformation);
