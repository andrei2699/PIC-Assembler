namespace PIC.Assembler.Application.Domain.Model.Tokens;

public record LabelToken(string Name, FileInformation FileInformation) : Token(FileInformation);
