using Tests;

namespace TddSqlLite;

public class Repl
{
    private readonly IConsoleWriteLineWrapper _writeLine;
    private readonly IConsoleInputWrapper _consoleInputWrapper;
    private readonly Stack<string> _commands = new();

    public Repl(IConsoleWriteLineWrapper writeLine)
    {
        _writeLine = writeLine;
    }

    public Repl(IConsoleWriteLineWrapper writeLine, IConsoleInputWrapper consoleInputWrapper)
    {
        _writeLine = writeLine;
        _consoleInputWrapper = consoleInputWrapper;
        _commands = consoleInputWrapper.RetrieveCommands();
    }

    public void Start()
    {
        _writeLine.Print("SQLite version 3.16.0 2016-11-04 19:09:39" +
                         "Enter \".help\" for usage hints.\nConnected to a transient " +
                         "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                         "database.");
        do
        {
            _writeLine.Print("sqlite> ");
            var currentCommandStack = _consoleInputWrapper.RetrieveCommands();
            var peek = currentCommandStack.TryPeek(out var command);
            var validCommands = new List<string>();
            if (command is "" or ".exit")
            {
                currentCommandStack.Pop();
            } else if (peek && !validCommands.Contains(command ?? ""))
            {
                _writeLine.Print($"Unrecognized command '{command}'.");
            }
            _consoleInputWrapper.WaitForInput();
        } while (_commands.Count > 0);
    }
}