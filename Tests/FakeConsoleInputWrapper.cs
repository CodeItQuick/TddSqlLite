namespace Tests;

public class FakeConsoleInputWrapper : IConsoleInputWrapper
{
    private readonly Stack<string> _commands;
    private Stack<string> _runCommands = new();

    public FakeConsoleInputWrapper(Stack<string> commands)
    {
        _commands = commands;
    }

    public string? WaitForInput()
    {
        var tryPeek = _commands.TryPeek(out var command);
        if (tryPeek && command != "")
        {
            _commands.Pop();
        }

        if (tryPeek && !string.IsNullOrWhiteSpace(command))
        {
            _runCommands.Push(command);
        }

        return command;
    }

    public Stack<string> RetrieveRunCommands()
    {
        return _runCommands;
    }
}