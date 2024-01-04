namespace Tests;

public interface IConsoleInputWrapper
{
    public string? WaitForInput();
    public List<string> RetrieveRunCommands();
}