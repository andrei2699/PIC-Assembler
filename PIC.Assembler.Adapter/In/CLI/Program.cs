using CommandLine;
using PIC.Assembler.Adapter.In.CLI;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Service;
using PIC.Assembler.Application.Port.In;
using PIC.Assembler.Application.Port.Out;

var result = Parser.Default.ParseArguments<CommandLineOptions>(args);

result.WithParsed(options =>
{
    IHexWriter fileHexDebugWriter = options.Debug ? new FileHexDebugWriter() : new FileHexWriterAdapter();

    var assembleService = GetAssembleService(fileHexDebugWriter);

    Console.WriteLine($"Reading from '{options.InputFilePath}'");
    Console.WriteLine($"Using config from '{options.ConfigFilePath}'");
    Console.WriteLine($"Writing to '{Path.GetFullPath(options.OutputFilePath)}'");

    assembleService.Assemble(new AssembleCommand(options.ConfigFilePath, options.InputFilePath,
        options.OutputFilePath));
});
return;

AssembleService GetAssembleService(IHexWriter hexWriter)
{
    var fileTokenizerAdapter = new FileTokenizerAdapter();

    return new AssembleService(new FileConfigLoaderAdapter(), fileTokenizerAdapter,
        new ParserService(fileTokenizerAdapter, new ArithmeticExpressionParser()), new LinkerService(), hexWriter);
}
