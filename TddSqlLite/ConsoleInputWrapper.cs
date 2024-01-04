using Tests;

namespace TddSqlLite;

public class ConsoleInputWrapper : IConsoleInputWrapper
{
    private readonly List<string> _consoleInput = new();
    protected string? _currentCommand;

    public virtual string? WaitForInput()
    {
        _currentCommand = Console.ReadLine();
        return _currentCommand;
    }


    public List<string> RetrieveRunCommands()
    {
        if (!string.IsNullOrEmpty(_currentCommand))
        {
            _consoleInput.Add(_currentCommand);
        }
        return _consoleInput;
    }
}