using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace PIC.Assembler.Adapter.Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class FileDataPathAttribute : DataAttribute
{
    private readonly string[] _filePaths;

    public FileDataPathAttribute(string filePath, [CallerFilePath] string testFilePath = "")
    {
        var testDataFolder =
            Path.GetRelativePath(Directory.GetCurrentDirectory(), Directory.GetParent(testFilePath)!.FullName)
                .Replace("..\\", "");

        _filePaths = [Path.Combine(Directory.GetCurrentDirectory(), testDataFolder, filePath)];
    }

    public FileDataPathAttribute([CallerFilePath] string testFilePath = "", params string[] filePaths)
    {
        var testDataFolder =
            Path.GetRelativePath(Directory.GetCurrentDirectory(), Directory.GetParent(testFilePath)!.FullName)
                .Replace("..\\", "");

        _filePaths = filePaths.Select(path => Path.Combine(Directory.GetCurrentDirectory(), testDataFolder, path))
            .ToArray();
    }


    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return _filePaths;
    }
}
