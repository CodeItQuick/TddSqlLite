using TddSqlLite;

namespace Tests;

public class FakeConsoleWriteLineWrapper : IConsoleWriteLineWrapper
{
    private string _message;

    public void Print(string message)
    {
        _message = message;
    }

    public string RetrieveMessage()
    {
        return _message;
    }
}