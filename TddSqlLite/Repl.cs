using TddSqlLite.Table;
using Tests;

namespace TddSqlLite;

public class Repl
{
    private readonly IConsoleWriteLineWrapper _writeLine;
    private readonly IConsoleInputWrapper _consoleInputWrapper;
    private Stack<string> _commands = new();
    private Table.Table _table;

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
        TABLE_FULL,
        INSERT_ROW_FAIL
    }
    private enum STATEMENTS
    {
        CREATE,
        INSERT,
        SELECT,
        INSERT_INTO
    };


    public Repl(IConsoleWriteLineWrapper writeLine, IConsoleInputWrapper consoleInputWrapper, string databaseFileName)
    {
        _writeLine = writeLine;
        _consoleInputWrapper = consoleInputWrapper;
        _table = new Table.Table(databaseFileName);
    }
    public Repl(IConsoleWriteLineWrapper writeLine, IConsoleInputWrapper consoleInputWrapper, Table.Table table)
    {
        _writeLine = writeLine;
        _consoleInputWrapper = consoleInputWrapper;
        _table = table;
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
            var command = currentCommandStack.Peek();
            _commands.Push(command);

            if (command.StartsWith("."))
            {
                var metaCommand = MetaCommands(command);
                switch (metaCommand)
                {
                    case META_COMMANDS.EXIT:
                        // wipe all commands to exit program
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

            switch (ExecuteStatement(statement, command))
            {
                case EXECUTE.SUCCESS:
                    _writeLine.Print("Executed.");
                    break;
                case EXECUTE.INSERT_ROW_FAIL:
                    _writeLine.Print("Failed to Insert Row. Already Exists.");
                    break;
                case EXECUTE.TABLE_FULL:
                    _writeLine.Print("Table is Full.");
                    break;
            }

        } while (_commands.Count > 0);
    }

    private EXECUTE ExecuteStatement(STATEMENTS statement, string command)
    {
        switch (statement)
        {
            case STATEMENTS.CREATE:
                return EXECUTE.SUCCESS;
            case STATEMENTS.INSERT_INTO or STATEMENTS.INSERT:
                string[] commands;
                if (statement == STATEMENTS.INSERT_INTO)
                {
                    var startBracket = command.IndexOf("(", StringComparison.Ordinal) + 1;
                    var endBracket = command.IndexOf(")", StringComparison.Ordinal);
                    var valuesInsertLength = endBracket - startBracket;
                    var inBrackets = command.Substring(startBracket, valuesInsertLength);
                    commands = inBrackets.Split(",");
                }
                else
                {
                    commands = command.Split(" ")[1..];
                }
                
                var insertRow = new Row()
                {
                    Id = Int32.Parse(commands.Skip(0).First()),
                    username = commands.Skip(1).First(),
                    email = commands.Skip(2).First(),
                };
                try
                {
                    _table.SerializeRow(insertRow);
                    return EXECUTE.SUCCESS;
                }
                catch
                {
                    return EXECUTE.INSERT_ROW_FAIL;
                }
            case STATEMENTS.SELECT:
                
                _writeLine.Print("Id\tusername\temail");
                var numRows = _table.CreateCursorStart();
                for (var rowIdx = 0; rowIdx < numRows; rowIdx++)
                {
                    var row = _table.SelectRow();
                    _writeLine.Print($"{row.Id}\t{row.username}\t{row.email}");
                    _table.AdvanceCursor();
                }
                return EXECUTE.SUCCESS;
            default:
                return EXECUTE.TABLE_FULL;
        }
    }

    private static PREPARE_STATEMENTS PrepareStatement(string command, out STATEMENTS statement)
    {
        var commands = command.Split(" ");
        var firstCommand = commands.FirstOrDefault();
        var isStatement = Enum.TryParse(firstCommand, out statement);
        switch (isStatement)
        {
            case false:
                return PREPARE_STATEMENTS.UNRECOGNIZED_STATEMENT;
            case true when statement == STATEMENTS.INSERT && commands.Contains("INTO") && commands.Contains("VALUES"):
                statement = STATEMENTS.INSERT_INTO;
                return PREPARE_STATEMENTS.SUCCESS;
            case true when statement == STATEMENTS.INSERT && commands.Length < 3:
                return PREPARE_STATEMENTS.SYNTAX_ERROR;
            default:
                return PREPARE_STATEMENTS.SUCCESS;
        }
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