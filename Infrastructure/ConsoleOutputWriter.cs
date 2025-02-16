using Refactoring.Domain.Interfaces;

namespace Refactoring.Infrastructure;

internal class ConsoleOutputWriter : IOutputWriter
{
    public void Write(string text)
    {
        Console.WriteLine(text);
    }
}