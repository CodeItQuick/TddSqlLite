using Tests;

namespace TddSqlLite;

public class ConsoleInputWrapper : IConsoleInputWrapper
{
    private readonly Stack<string> _consoleInput = new();
    protected string? _currentCommand;

    public virtual string? WaitForInput()
    {
        _currentCommand = Console.ReadLine();
        return _currentCommand;
    }


    public Stack<string> RetrieveRunCommands()
    {
        if (!string.IsNullOrEmpty(_currentCommand))
        {
            _consoleInput.Push(_currentCommand);
        }
        return _consoleInput;
    }
}