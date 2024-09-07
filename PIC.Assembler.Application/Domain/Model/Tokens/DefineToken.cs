namespace PIC.Assembler.Application.Domain.Model.Tokens;

public record DefineToken(FileInformation FileInformation) : Token(FileInformation);
