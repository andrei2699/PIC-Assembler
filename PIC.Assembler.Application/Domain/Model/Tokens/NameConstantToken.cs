namespace PIC.Assembler.Application.Domain.Model.Tokens;

public record NameConstantToken(string Name, FileInformation FileInformation) : Token(FileInformation);
