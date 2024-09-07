using CommandLine;
using PIC.Assembler.Adapter.In.CLI;
using PIC.Assembler.Adapter.Out.File;
using PIC.Assembler.Application.Domain.Service;
using PIC.Assembler.Application.Port.In;

var result = Parser.Default.ParseArguments<CommandLineOptions>(args);

result.WithParsed(options =>
{
    // TODO: replace with real hex writer
    var fileHexDebugWriter = new FileHexDebugWriter();
    if (options.Debug)
    {
        fileHexDebugWriter = new FileHexDebugWriter();
    }

    var assembleService = GetAssembleService(fileHexDebugWriter);

    assembleService.Assemble(new AssembleCommand(options.ConfigFilePath, options.InputFilePath,
        options.OutputFilePath));
});
return;

AssembleService GetAssembleService(FileHexDebugWriter hexWriter)
{
    var fileTokenizerAdapter = new FileTokenizerAdapter();

    return new AssembleService(new FileConfigLoaderAdapter(), fileTokenizerAdapter,
        new ParserService(fileTokenizerAdapter, new ArithmeticExpressionParser()), new LinkerService(), hexWriter);
}
