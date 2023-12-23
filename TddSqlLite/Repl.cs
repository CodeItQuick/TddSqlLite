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

    public Repl(IConsoleWriteLineWrapper writeLine, Stack<string> commands)
    {
        _writeLine = writeLine;
        _commands = commands;
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
        bool waitingCommand = true;
        do
        {
            var tryPeek = _commands.TryPeek(out var result);
            if (tryPeek)
            {
                var command = _commands.Pop();
                _writeLine.Print("sqlite> " + command);
            }
            else
            {
                _writeLine.Print("sqlite> ");
                waitingCommand = false;
            }
        } while (_commands.Count > 0 || waitingCommand);
    }
}