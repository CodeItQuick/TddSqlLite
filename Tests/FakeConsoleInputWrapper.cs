namespace Tests;

public class FakeConsoleInputWrapper : IConsoleInputWrapper
{
    private readonly Stack<string> _commands;

    public FakeConsoleInputWrapper(Stack<string> commands)
    {
        _commands = commands;
    }

    public void WaitForInput()
    {
        _commands.Pop();
    }

    public Stack<string> RetrieveCommands()
    {
        return _commands;
    }
}