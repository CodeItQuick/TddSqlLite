using TddSqlLite.Database;
using Tests;

namespace TddSqlLite;

public class Repl
{
    private readonly IConsoleWriteLineWrapper _writeLine;
    private readonly IConsoleInputWrapper _consoleInputWrapper;
    private Stack<string> _commands = new();
    private Table _table;
    private Table[] _tables;
    private IDbFileHandler _tableFileHandler;

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
        INSERT_ROW_FAIL,
        SELECT_MISSING_TABLE_FAIL
    }
    private enum STATEMENTS
    {
        CREATE,
        INSERT,
        SELECT,
        INSERT_INTO,
    };


    public Repl(IConsoleWriteLineWrapper writeLine, IConsoleInputWrapper consoleInputWrapper, string databaseFileName)
    {
        _writeLine = writeLine;
        _consoleInputWrapper = consoleInputWrapper;
        _tableFileHandler = new DbTableFileHandler(databaseFileName);
        _table = new Table("database"); // FIXME hardcoded
        _tables = new[] { _table };
    }
    public Repl(IConsoleWriteLineWrapper writeLine, IConsoleInputWrapper consoleInputWrapper, Table table, IDbFileHandler fakeDbFileHandler)
    {
        _writeLine = writeLine;
        _consoleInputWrapper = consoleInputWrapper;
        _table = table;
        _tables = new[] { _table };
        _tableFileHandler = fakeDbFileHandler;
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
                    continue;
            }

            try
            {
                var executeStatement = ExecuteStatement(statement, command);
                switch (executeStatement)
                {
                    case EXECUTE.SUCCESS:
                        _writeLine.Print("Executed.");
                        break;
                    case EXECUTE.INSERT_ROW_FAIL:
                        _writeLine.Print("Failed to Insert Row. Already Exists.");
                        break;
                    case EXECUTE.SELECT_MISSING_TABLE_FAIL:
                        _writeLine.Print("Failed to Select Table. Table does not exist.");
                        break;
                    case EXECUTE.TABLE_FULL:
                        _writeLine.Print("Table is Full.");
                        break;
                }
            }
            catch
            {
                _writeLine.Print("Parse Error - could not read statement.");
                continue;
            }

        } while (_commands.Count > 0);
    }

    private EXECUTE ExecuteStatement(STATEMENTS statement, string command)
    {
        switch (statement)
        {
            case STATEMENTS.CREATE:
                // finding parameters
                // FIXME: (irrelevant)
                var createStartBracket = command.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1;
                var createEndBracket = command.IndexOf(")", StringComparison.OrdinalIgnoreCase);
                var createValuesInsertLength = createEndBracket - createStartBracket;
                var createInBrackets = command.Substring(createStartBracket, createValuesInsertLength);
                var createCommands = createInBrackets.Split(",");
                // finding Table Name                    
                var createStartTableName = command.IndexOf("TABLE", StringComparison.OrdinalIgnoreCase) + 5;
                var createEndTableName = command.IndexOf("(", StringComparison.OrdinalIgnoreCase);
                var createTableStringLength = createEndTableName - createStartTableName;
                var createTableName = command
                    .Substring(createStartTableName, createTableStringLength)
                    .Trim();
                _tableFileHandler.InjectFilename(createTableName + ".txt");
                _tables = _tables
                    .Append(new Table(_tableFileHandler, createTableName))
                    .ToArray();
                return EXECUTE.SUCCESS;
            case STATEMENTS.INSERT_INTO or STATEMENTS.INSERT:
                string[] commands;
                var insertIntoTable = _table;
                if (statement == STATEMENTS.INSERT_INTO)
                {
                    // finding parameters
                    var startBracket = command.IndexOf("(", StringComparison.Ordinal) + 1;
                    var endBracket = command.IndexOf(")", StringComparison.Ordinal);
                    var valuesInsertLength = endBracket - startBracket;
                    var inBrackets = command.Substring(startBracket, valuesInsertLength);
                    commands = inBrackets.Split(",");
                    // finding Table Name                    
                    var startTableName = command.IndexOf("INTO", StringComparison.OrdinalIgnoreCase) + 4;
                    var endTableName = command.IndexOf("VALUES", StringComparison.OrdinalIgnoreCase);
                    var tableStringLength = endTableName - startTableName;
                    var tableName = command.Substring(startTableName, tableStringLength)
                                                .Trim();
                    insertIntoTable = _tables.First(table => table.IsTableName(tableName));
                }
                else
                {
                    commands = command.Split(" ")[1..];
                }
                
                var insertRow = new Row()
                {
                    Id = Int32.Parse(commands.Skip(0).First()),
                    username = commands.Skip(1).First().Trim(),
                    email = commands.Skip(2).First().Trim(),
                };
                try
                {
                    insertIntoTable.SerializeRow(insertRow);
                    return EXECUTE.SUCCESS;
                }
                catch
                {
                    return EXECUTE.INSERT_ROW_FAIL;
                }
            case STATEMENTS.SELECT:
                
                var selectedTable = command.Split(" ")[^1];
                try
                {
                    insertIntoTable = _tables.First(table => table.IsTableName(selectedTable));
                }
                catch
                {
                    return EXECUTE.SELECT_MISSING_TABLE_FAIL;
                }
                _writeLine.Print("Id\tusername\temail");
                var numRows = insertIntoTable.CreateCursorStart();
                for (var rowIdx = 0; rowIdx < numRows; rowIdx++)
                {
                    var row = insertIntoTable.SelectRow();
                    _writeLine.Print($"{row.Id}\t{row.username}\t{row.email}");
                    insertIntoTable.AdvanceCursor();
                }
                return EXECUTE.SUCCESS;
            default:
                return EXECUTE.TABLE_FULL;
        }
    }

    // FIXME: Move this to its own class
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