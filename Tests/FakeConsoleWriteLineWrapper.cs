using TddSqlLite;

namespace Tests;

public class FakeConsoleWriteLineWrapper : IConsoleWriteLineWrapper
{
    private readonly List<string> _message = new();

    public void Print(string message)
    {
        _message.Add(message);
    }

    public List<string> RetrieveMessage()
    {
        return _message;
    }
}