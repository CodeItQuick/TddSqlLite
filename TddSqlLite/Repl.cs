namespace TddSqlLite;

public class Repl
{
    private readonly IConsoleWriteLineWrapper _writeLine;
    private readonly List<string> _commands;

    public Repl(IConsoleWriteLineWrapper writeLine)
    {
        _writeLine = writeLine;
    }

    public Repl(IConsoleWriteLineWrapper writeLine, List<string> commands)
    {
        _writeLine = writeLine;
        _commands = commands;
    }

    public void Start()
    {
        _writeLine.Print("SQLite version 3.16.0 2016-11-04 19:09:39" +
                         "Enter \".help\" for usage hints.\nConnected to a transient " +
                         "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                         "database.");
        foreach (var command in _commands)
        {
            _writeLine.Print("sqlite> " + command);
        }
    }
}