namespace PIC.Assembler.Application.Domain.Model.Tokens.Operation;

public record LeftShiftToken(FileInformation FileInformation) : Token(FileInformation);
