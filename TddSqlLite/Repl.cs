using Tests;

namespace TddSqlLite;

public class Repl
{
    private readonly IConsoleWriteLineWrapper _writeLine;
    private readonly IConsoleInputWrapper _consoleInputWrapper;
    private readonly Stack<string> _commands = new();
    private enum META_COMMANDS
    {
        EXIT,
        UNRECOGNIZED_COMMAND
    }

    private enum STATEMENTS
    {
        CREATE,
        INSERT
    };


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
            if (command.StartsWith("."))
            {
                var metaCommand = MetaCommands(command, currentCommandStack);
                if (metaCommand == META_COMMANDS.EXIT)
                {
                    currentCommandStack.Pop();
                    continue;
                }

                if (metaCommand == META_COMMANDS.UNRECOGNIZED_COMMAND)
                {
                    _writeLine.Print($"Unrecognized command '{command}'.");
                    continue;
                }
            }

            var firstCommand = command.Split(" ").FirstOrDefault();
            var tryParseStatement = Enum.TryParse<STATEMENTS>(firstCommand, out var statement);
            if (tryParseStatement)
            {
                if (statement == STATEMENTS.CREATE)
                {
                    
                }

                if (statement == STATEMENTS.INSERT)
                {
                    
                }
            }
            else
            {
                _writeLine.Print($"Unrecognized command '{command}'.");
            }
            _consoleInputWrapper.WaitForInput();
        } while (_commands.Count > 0);
    }

    private META_COMMANDS MetaCommands(string command, Stack<string> currentCommandStack)
    {
        if (command is ".exit")
        {
            return META_COMMANDS.EXIT;
        }

        return META_COMMANDS.UNRECOGNIZED_COMMAND;
    }
}