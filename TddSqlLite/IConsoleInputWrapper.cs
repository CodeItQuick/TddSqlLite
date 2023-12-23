namespace Tests;

public interface IConsoleInputWrapper
{
    public void WaitForInput();
    Stack<string> RetrieveCommands();
}