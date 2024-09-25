# PIC Assembler

## Command line Options

- `InputFilePath` - Input path to assembly file
- `-c` - Path to config file
- `-o` - Output path to output hex file. Default value: `output.hex`
- `--debug`- Write to output file in the following format: `address opcode`

```shell
.\PicAssembler.exe file.asm -c PIC10F200.json -o output_file.asm
```

## Build Project

```shell
dotnet publish -r win-x64
```
