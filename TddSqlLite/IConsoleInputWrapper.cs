namespace Tests;

public interface IConsoleInputWrapper
{
    public string? WaitForInput();
    public Stack<string> RetrieveRunCommands();
}