using PIC.Assembler.Application.Domain.Model;

namespace PIC.Assembler.Application.Port.Out;

public interface IConfigLoader
{
    MicrocontrollerConfig Load(string filepath);
}
