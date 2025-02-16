using System.Xml;

namespace Refactoring.Domain.Interfaces;

public interface IOutputWriter
{
    void Write(string text);
}