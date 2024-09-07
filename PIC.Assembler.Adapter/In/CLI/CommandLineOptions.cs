using CommandLine;

namespace PIC.Assembler.Adapter.In.CLI;

public class CommandLineOptions
{
    [Value(0, Required = true, HelpText = "Input path to assembly file")]
    public string InputFilePath { get; set; } = "";

    [Option('c', "config", Required = true, HelpText = "Path to config file")]
    public string ConfigFilePath { get; set; } = "";

    [Option('o', "output", Required = false, Default = "output.hex", HelpText = "Output path to output hex file")]
    public string OutputFilePath { get; set; } = "";

    [Option("debug", Required = false, Default = false,
        HelpText = "Write to output file in the following format: address opcode")]
    public bool Debug { get; set; } = false;
}
