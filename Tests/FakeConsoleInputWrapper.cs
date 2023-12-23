namespace Tests;

public class FakeConsoleInputWrapper
{
    private readonly Stack<string> _commands;

    public FakeConsoleInputWrapper(Stack<string> commands)
    {
        _commands = commands;
    }

    public void WaitForInput()
    {
        
    }

    public Stack<string> RetrieveCommands()
    {
        return _commands;
    }
}