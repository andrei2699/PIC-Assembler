using PIC.Assembler.Application.Domain.Model;

namespace PIC.Assembler.Application.Port.Out;

public interface ILoadConfig
{
    MicrocontrollerConfig Load(string filepath);
}
