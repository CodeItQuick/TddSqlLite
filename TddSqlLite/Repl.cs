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

    private enum PREPARE_STATEMENTS
    {
        SUCCESS,
        SYNTAX_ERROR,
        UNRECOGNIZED_STATEMENT
    }
    private enum STATEMENTS
    {
        CREATE,
        INSERT,
        SELECT
    };


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
                var metaCommand = MetaCommands(command);
                ExecuteMetaCommand(metaCommand, currentCommandStack, command);
                continue;
            }

            var tryParseStatement = PrepareStatement(command, out var statement);
            if (tryParseStatement == PREPARE_STATEMENTS.SUCCESS)
            {
                ExecuteStatement(statement);
            }
            else if (tryParseStatement == PREPARE_STATEMENTS.SYNTAX_ERROR)
            {
                _writeLine.Print("Syntax error. Could not parse statement.\n");
            }
            else
            {
                _writeLine.Print($"Unrecognized keyword at start of '{command}'.");
            }

            _consoleInputWrapper.WaitForInput();
        } while (_commands.Count > 0);
    }

    private void ExecuteStatement(STATEMENTS statement)
    {
        switch (statement)
        {
            case STATEMENTS.CREATE:
                break;
            case STATEMENTS.INSERT:
                break;
            case STATEMENTS.SELECT:
                break;
        }
    }

    private static PREPARE_STATEMENTS PrepareStatement(string command, out STATEMENTS statement)
    {
        var commands = command.Split(" ");
        var firstCommand = commands.FirstOrDefault();
        var tryParseStatement = Enum.TryParse(firstCommand, out statement);
        if (statement == STATEMENTS.INSERT)
        {
            if (commands.Length < 3)
            {
                return PREPARE_STATEMENTS.SYNTAX_ERROR;
            }
        }
        return tryParseStatement ? PREPARE_STATEMENTS.SUCCESS : PREPARE_STATEMENTS.UNRECOGNIZED_STATEMENT;
    }

    private void ExecuteMetaCommand(META_COMMANDS metaCommand, Stack<string> currentCommandStack, string command)
    {
        switch (metaCommand)
        {
            case META_COMMANDS.EXIT:
                // pop the last command off the stack to exit the loop
                currentCommandStack.Pop();
                break;
            case META_COMMANDS.UNRECOGNIZED_COMMAND:
            default:
                _writeLine.Print($"Unrecognized command '{command}'.");
                break;
        }
    }

    private META_COMMANDS MetaCommands(string command)
    {
        if (command is ".exit")
        {
            return META_COMMANDS.EXIT;
        }

        return META_COMMANDS.UNRECOGNIZED_COMMAND;
    }
}