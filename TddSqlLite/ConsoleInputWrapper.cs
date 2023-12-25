using Tests;

namespace TddSqlLite;

public class ConsoleInputWrapper : IConsoleInputWrapper
{
    private readonly Stack<string> _consoleInput = new();
    public string? WaitForInput()
    {
        var consoleInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(consoleInput) && consoleInput != ".exit")
        {
            _consoleInput.Push(consoleInput);
        }

        return consoleInput;
    }

    public Stack<string> RetrieveCommands()
    {
        var notEmpty = _consoleInput.TryPop(out var command);
        return _consoleInput;
    }
    public Stack<string> RetrieveRunCommands()
    {
        return _consoleInput;
    }
}