using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TddSqlLite;
using TddSqlLite.Table;

namespace Tests;

public class ReplTests
{
    private const string IntroText = "SQLite version 3.16.0 2016-11-04 19:09:39" +
                                     "Enter \".help\" for usage hints.\nConnected to a transient " +
                                     "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                                     "database.";

    [Fact]
    public void MainProducesTextOutput()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit");
        var fakeConsoleInputWrapper = new FakeConsoleInputWrapper(commands);
        var repl = new Repl(writeLineWrapper, fakeConsoleInputWrapper, new Table(new FakeDbFileHandler()));

        repl.Start();

        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", writeLineWrapper.RetrieveMessage());
    }

    [Fact]
    public void CanStartReplWithCREATECommands()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("CREATE table users (id int, username varchar(255), email varchar(255));");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(4, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Executed.",
            "sqlite> ", // enter create statement
            IntroText // intro text
        }, retrieveMessage.ToList());
    }

    [Fact]
    public void CanStartReplWithTwoStatementInsertCommandCreatesSyntaxError()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("INSERT 1");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(4, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Syntax error. Could not parse statement.\n",
            "sqlite> ", // enter insert statement
            IntroText // intro text
        }, retrieveMessage.ToList());
    }

    [Fact]
    public void CanStartReplWithTwoStatementInsertCommandCreatesTable()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("INSERT 1 cstack foo@bar.com");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(4, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Executed.",
            "sqlite> ", // enter insert statement
            IntroText // intro text
        }, retrieveMessage.ToList());
    }
    [Fact]
    public void CanStartReplWithTwoStatementInsertCommandInsertsRetrievableRows()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("SELECT * FROM cstack");
        commands.Push("INSERT 1 cstack foo@bar.com");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "1\tcstack\tfoo@bar.com",
            "Id\tusername\temail",
            "Executed.",
            "sqlite> ",
            "Executed.",
            "sqlite> ", // enter insert statement
            IntroText // intro text
        }, retrieveMessage.ToList());
        Assert.Equal(8, retrieveMessage.Count);
    }
    [Fact]
    public void InsertingTheTwoSameRecordsTwiceDoesNotCrashProgram()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("INSERT 1 error causes@error.com");
        commands.Push("INSERT 1 cstack foo@bar.com");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Failed to Insert Row. Already Exists.",
            "sqlite> ", // enter failed insert statement
            "Executed.",
            "sqlite> ", // enter insert statement
            IntroText // intro text
        }, retrieveMessage.ToList(), true);
    }
    [Fact]
    public void CanInsertBySpecifyingTableIntoDatabase()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("INSERT INTO database VALUES (1, error, causes@error.com)");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));
    
        repl.Start();
    
        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Executed.",
            "sqlite> ", // enter insert statement
            IntroText // intro text
        }, retrieveMessage.ToList(), true);
    }
    [Fact(Skip = "Unimplemented Acceptance Test #2")]
    public void CanInsertNewTableIntoDatabase()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("INSERT INTO NameOfTable VALUES (1, 'error', 'causes@error.com')");
        commands.Push("CREATE TABLE NameOfTable (" +
                          "Id int," +
                          "username VARCHAR," +
                          "email VARCHAR" +
                          ")");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));
    
        repl.Start();
    
        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Failed to Insert Row. Already Exists.",
            "sqlite> ", // enter failed insert statement
            "Executed.",
            "sqlite> ", // enter insert statement
            IntroText // intro text
        }, retrieveMessage.ToList(), true);
    }
    [Fact]
    public void CanRetrieveInsertedRowAfterClosingRepl()
    {
        var databaseTableFilename = @"databaseCanRetrieveAfterClosed.txt";
        string fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            databaseTableFilename);
        File.WriteAllText(fullPath, "", Encoding.UTF8);
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("INSERT 1 cstack foo@bar.com");
        var table = new Table(@"databaseCanRetrieveAfterClosed.txt");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), table);
        var writeLineWrapperRetrieval = new FakeConsoleWriteLineWrapper();
        var commandsRetrieval = new Stack<string>();
        commandsRetrieval.Push(".exit"); // exit
        commandsRetrieval.Push("SELECT * FROM cstack");
        var replRetrieve = new Repl(writeLineWrapperRetrieval, new FakeConsoleInputWrapper(commandsRetrieval), table);
        repl.Start();

        replRetrieve.Start();

        var retrieveMessage = writeLineWrapperRetrieval.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Executed.",
            "1\tcstack\tfoo@bar.com",
            "Id\tusername\temail",
            "sqlite> ", // enter select statement
            IntroText // intro text
        }, retrieveMessage.ToList());
        Assert.Equal(6, retrieveMessage.Count);
    }

    [Fact]
    public void CanInsertMultipleRowsAndSelectInsertedRows()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("SELECT * FROM USERS;");
        commands.Push("INSERT 3 dstack foo3@bar.com");
        commands.Push("INSERT 2 astack foo2@bar.com");
        commands.Push("INSERT 1 cstack foo1@bar.com");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "3\tdstack\tfoo3@bar.com",
            "2\tastack\tfoo2@bar.com",
            "1\tcstack\tfoo1@bar.com",
            "Id\tusername\temail",
            "Executed.",
            "sqlite> ", // enter create statement
            IntroText // intro text
        }, retrieveMessage.ToList());
        Assert.Equal(14, retrieveMessage.Count);
    }

    [Fact]
    public void CanRecognizeWhenKeywordInvalid()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("invalid command");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("Unrecognized keyword at start of 'invalid command'.", retrieveMessage);
        Assert.Equal(2, retrieveMessage.Count(x => x == "sqlite> ")); // two of these
        Assert.Equal(5, retrieveMessage.Count);
    }

    [Fact]
    public void CanRecognizeWhenInvalidMetaCommand()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push(".invalidcommand");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("Unrecognized command '.invalidcommand'.", retrieveMessage);
        Assert.Equal(2, retrieveMessage.Count(x => x == "sqlite> ")); // two of these
        Assert.Equal(4, retrieveMessage.Count);
    }

    [Fact]
    public void CanRecognizeWhenCommandInvalid()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("invalid command");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("Unrecognized keyword at start of 'invalid command'.", retrieveMessage);
        Assert.Equal(2, retrieveMessage.Count(x => x == "sqlite> ")); // two of these
        Assert.Equal(5, retrieveMessage.Count);
    }
    
    [Fact]
    public void InsertNonContiguousRowsDoesNotCrashOnSelect()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("SELECT * FROM USERS;");
        commands.Push("INSERT 3 dstack foo3@bar.com");
        commands.Push("INSERT 1 cstack foo1@bar.com");
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler()));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "3\tdstack\tfoo3@bar.com",
            "1\tcstack\tfoo1@bar.com",
            "Id\tusername\temail",
            "Executed.",
            "sqlite> ", // enter create statement
            IntroText // intro text
        }, retrieveMessage.ToList());
        Assert.Equal(11, retrieveMessage.Count);
    }
}