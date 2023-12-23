using Tests;

namespace TddSqlLite;

public class ConsoleInputWrapper : IConsoleInputWrapper
{
    public readonly Stack<string> ConsoleInput = new();
    public void WaitForInput()
    {
        var consoleInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(consoleInput) && consoleInput != ".exit")
        {
            ConsoleInput.Push(consoleInput);
        }
    }

    public Stack<string> RetrieveCommands()
    {
        var notEmpty = ConsoleInput.TryPop(out var command);
        return ConsoleInput;
    }
}