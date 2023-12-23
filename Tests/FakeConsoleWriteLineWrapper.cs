using TddSqlLite;

namespace Tests;

public class FakeConsoleWriteLineWrapper : IConsoleWriteLineWrapper
{
    private readonly Stack<string> _message = new();

    public void Print(string message)
    {
        _message.Push(message);
    }

    public Stack<string> RetrieveMessage()
    {
        return _message;
    }
}