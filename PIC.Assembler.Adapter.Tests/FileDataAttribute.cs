using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace PIC.Assembler.Adapter.Tests;

public class FileDataAttribute : DataAttribute
{
    private readonly string _filePath;

    public FileDataAttribute(string filePath, [CallerFilePath] string testFilePath = "")
    {
        var testDataFolder =
            Path.GetRelativePath(Directory.GetCurrentDirectory(), Directory.GetParent(testFilePath)!.FullName)
                .Replace("..\\", "");
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), testDataFolder, filePath);
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return [File.ReadAllText(_filePath)];
    }
}
