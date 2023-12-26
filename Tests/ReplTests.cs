using System.ComponentModel.DataAnnotations.Schema;
using TddSqlLite;

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
        var repl = new TddSqlLite.Repl(writeLineWrapper, fakeConsoleInputWrapper);

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
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(3, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
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
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

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
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(3, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "sqlite> ", // enter insert statement
            IntroText // intro text
        }, retrieveMessage.ToList());
    }

    [Fact]
    public void CanStartReplWithSelectCommands()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("SELECT * FROM USERS;");
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(3, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "sqlite> ", // enter create statement
            IntroText // intro text
        }, retrieveMessage.ToList());
    }

    [Fact]
    public void CanRecognizeWhenKeywordInvalid()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push("invalid command");
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("Unrecognized keyword at start of 'invalid command'.", retrieveMessage);
        Assert.Equal(2, retrieveMessage.Count(x => x == "sqlite> ")); // two of these
        Assert.Equal(4, retrieveMessage.Count);
    }

    [Fact]
    public void CanRecognizeWhenInvalidMetaCommand()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(".exit"); // exit
        commands.Push(".invalidcommand");
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

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
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("Unrecognized keyword at start of 'invalid command'.", retrieveMessage);
        Assert.Equal(2, retrieveMessage.Count(x => x == "sqlite> ")); // two of these
        Assert.Equal(4, retrieveMessage.Count);
    }

    [Fact]
    public void CanInsertRowsIntoTable()
    {
        var table = new Table();
        var row = new Row() { Id = 1, email = "test@user.com", username = "test_user" };
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        table.SerializeRow(row, page);

        Assert.Equivalent(new Row()
            {
                Id = 1, email = "test@user.com", username = "test_user"
            },
            table.DeserializeRow(page, row));
    }
    [Fact]
    public void CanInsertRowsIntoExistingPage()
    {
        var table = new Table();
        var row1 = new Row()
        {
            Id = 1, email = "test@user.com", username = "test_user"
        };
        var row2 = new Row() { Id = 2, email = "test@user.com", username = "test_user" };
        var page = new Page() { 
            PageNum = 0, 
            Rows = new []
            { 
                row1 
            }
        };
        table.SerializeRow(row2, page);

        Assert.Equivalent(
            new Row()
            {
                Id = 1, email = "test@user.com", username = "test_user"
            },
            table.DeserializeRow(page, row1));
        Assert.Equivalent(
            new Row()
            {
                Id = 2, email = "test@user.com", username = "test_user"
            },
            table.DeserializeRow(page, row2));
    }
    [Fact]
    public void KnowsWhenPageFull()
    {
        var table = new Table();
        var row = new Row() { Id = 1, email = "test@user.com", username = "test_user" };
        var rows = Enumerable
            .Range(0, 1001)
            .Select(x => new Row()
            {
                Id = x, email = "test@user.com", username = "test_user"
            })
            .ToArray();
        var page = new Page() { PageNum = 0, Rows = rows};
        Assert.Throws<Exception>(() => table.SerializeRow(row, page));
    }
    [Fact]
    public void TableHasMaxPageNumber()
    {
        var table = new Table();
        var row = new Row() { Id = 1, email = "test@user.com", username = "test_user" };
        var page = new Page() { PageNum = 101 };

        Assert.Throws<Exception>(() => table.SerializeRow(row, page));
    }
}