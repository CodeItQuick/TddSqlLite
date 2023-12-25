using TddSqlLite;

namespace Tests;

public class FakeConsoleInputWrapper : ConsoleInputWrapper
{
    private readonly Stack<string> _commands;

    public FakeConsoleInputWrapper(Stack<string> commands)
    {
        _commands = commands;
    }

    public override string? WaitForInput()
    {
        var tryPop = _commands.TryPop(out var command);
        _currentCommand = command;  

        return _currentCommand;
    }
}