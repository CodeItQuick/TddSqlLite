using Tests;

namespace TddSqlLite;

public class Repl
{
    private readonly IConsoleWriteLineWrapper _writeLine;
    private readonly IConsoleInputWrapper _consoleInputWrapper;
    private Stack<string> _commands = new();
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
    private enum EXECUTE
    {
        SUCCESS,
        TABLE_FULL
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
            _consoleInputWrapper.WaitForInput();
            var currentCommandStack = _consoleInputWrapper.RetrieveRunCommands();
            var peekSuccess = currentCommandStack.TryPeek(out var command);
            if (peekSuccess && command != null)
            {
                _commands.Push(command);
            }

            if (command != null && command.StartsWith("."))
            {
                var metaCommand = MetaCommands(command);
                switch (metaCommand)
                {
                    case META_COMMANDS.EXIT:
                        // pop the last command off the stack to exit the loop
                        _commands = new Stack<string>();
                        continue;
                    case META_COMMANDS.UNRECOGNIZED_COMMAND:
                    default:
                        _writeLine.Print($"Unrecognized command '{command}'.");
                        continue;
                }
            }

            var tryParseStatement = PrepareStatement(command, out var statement);
            switch (tryParseStatement)
            {
                case PREPARE_STATEMENTS.SUCCESS:
                    break;
                case PREPARE_STATEMENTS.SYNTAX_ERROR:
                    _writeLine.Print("Syntax error. Could not parse statement.\n");
                    continue;
                case PREPARE_STATEMENTS.UNRECOGNIZED_STATEMENT:
                    _writeLine.Print($"Unrecognized keyword at start of '{command}'.");
                    break;
            }

            switch (ExecuteStatement(statement))
            {
                case EXECUTE.SUCCESS:
                    break;
                case EXECUTE.TABLE_FULL:
                    break;
            }

        } while (_commands.Count > 0);
    }

    private EXECUTE ExecuteStatement(STATEMENTS statement)
    {
        switch (statement)
        {
            case STATEMENTS.CREATE:
                return EXECUTE.SUCCESS;
            case STATEMENTS.INSERT:
                return EXECUTE.SUCCESS;
            case STATEMENTS.SELECT:
                return EXECUTE.SUCCESS;
            default:
                return EXECUTE.TABLE_FULL;
        }
    }

    private static PREPARE_STATEMENTS? PrepareStatement(string? command, out STATEMENTS statement)
    {
        if (command == null)
        {
            statement = STATEMENTS.CREATE;
            return null;
        }
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

    private META_COMMANDS MetaCommands(string command)
    {
        switch (command)
        {
            case ".exit":
                return META_COMMANDS.EXIT;
            default:
                return META_COMMANDS.UNRECOGNIZED_COMMAND;
        }
    }
}