namespace PIC.Assembler.Application.Domain.Model.Tokens;

public record IncludeToken(FileInformation FileInformation) : Token(FileInformation);
