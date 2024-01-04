using TddSqlLite;

namespace Tests;

public class FakeConsoleInputWrapper : ConsoleInputWrapper
{
    private List<string> _commands;


    public FakeConsoleInputWrapper(List<string> commands)
    {
        _commands = commands;
    }

    public override string? WaitForInput()
    {
        if (_commands.Any())
        {
            var command = _commands.First();
            _commands = _commands.Skip(1).ToList();
            _currentCommand = command;  
            return _currentCommand;
        }

        return "";

    }
}