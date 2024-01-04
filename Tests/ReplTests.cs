using System.Text;
using TddSqlLite;
using TddSqlLite.Database;

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
        var commands = new List<string>();
        commands.Add(".exit");
        var fakeConsoleInputWrapper = new FakeConsoleInputWrapper(commands);
        var repl = new Repl(writeLineWrapper, fakeConsoleInputWrapper, new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

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
        var commands = new List<string>
        {
            "CREATE TABLE users (id int, username varchar, email varchar)",
            ".exit" // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(4, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter create statement
            "Executed.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList());
    }
    [Fact]
    public void ThrowsErrorWhenTableMissing()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "SELECT * FROM MissingTable",
            ".exit" // exit
        };
        var repl = new Repl(
            writeLineWrapper, 
            new FakeConsoleInputWrapper(commands), 
            new Table(new FakeDbFileHandler(), "database"), 
            new FakeDbFileHandler());

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(4, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            "sqlite> ", // enter .exit
            "Failed to Select Table. Table does not exist.",
            "sqlite> ", // enter create statement
            IntroText // intro text
        }, retrieveMessage.ToList(), true);
    }

    [Fact]
    public void CanStartReplWithTwoStatementInsertCommandCreatesSyntaxError()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "INSERT 1",
            ".exit" // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equal(4, retrieveMessage.Count);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter insert statement
            "Syntax error. Could not parse statement.\n",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList());
    }

    [Fact]
    public void CanStartReplWithTwoStatementInsertCommandCreatesTable()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>();
        commands.Add("INSERT 1 cstack foo@bar.com");
        commands.Add(".exit"); // exit
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

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
        var commands = new List<string>();
        commands.Add("INSERT 1 cstack foo@bar.com");
        commands.Add("SELECT * FROM database");
        commands.Add(".exit"); // exit
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter insert statement
            "Executed.",
            "sqlite> ",
            "1\tcstack\tfoo@bar.com",
            "Id\tusername\temail",
            "Executed.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList());
        Assert.Equal(8, retrieveMessage.Count);
    }
    [Fact]
    public void InsertingTheTwoSameRecordsTwiceDoesNotCrashProgram()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "INSERT 1 cstack foo@bar.com",
            "INSERT 1 error causes@error.com",
            ".exit" // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter insert statement
            "Executed.",
            "sqlite> ", // enter failed insert statement
            "Failed to Insert Row. Already Exists.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList(), true);
    }
    [Fact]
    public void CanInsertBySpecifyingTableIntoDatabase()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "INSERT INTO database VALUES (1, user, user@test.com)",
            ".exit" // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());
    
        repl.Start();
    
        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter insert statement
            "Executed.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList(), true);
    }
    [Fact]
    public void CanInsertNewTableIntoDatabase()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>();
        commands.Add("CREATE TABLE NameOfTable (" +
                     "Id int," +
                     "username VARCHAR," +
                     "email VARCHAR" +
                     ")");
        commands.Add("INSERT INTO NameOfTable VALUES (3, testusername, test@user.com)");
        commands.Add(".exit"); // exit
        var fakeDbFileHandler = new FakeDbFileHandler();
        var repl = new Repl(
            writeLineWrapper, 
            new FakeConsoleInputWrapper(commands), 
            new Table(fakeDbFileHandler, "database"), 
            fakeDbFileHandler);
    
        repl.Start();
    
        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter create statement
            "Executed.",
            "sqlite> ", // enter insert statement
            "Executed.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList(), true);
    }

    [Fact]
    public void DoesNotCrashOnBadCreateInput()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var fakeDbFileHandler = new FakeDbFileHandler();
        var commands = new List<string>
        {
            "CREATE TABLE NameOfTable (" +
            "Id int," +
            "username VARCHAR," +
            "email VARCHAR" +
            "error",
            ".exit" // exit
        };
        var repl = new Repl(
            writeLineWrapper, 
            new FakeConsoleInputWrapper(commands), 
            new Table(fakeDbFileHandler, "database"), 
            fakeDbFileHandler);
    
        repl.Start();
    
        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter create statement
            "Parse Error - could not read statement.",
            "sqlite> ", // enter exit statement
        }, retrieveMessage.ToList(), true);
    }

    [Fact]
    public void CanSelectFromNewTableAfterInsert()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>();
        commands.Add("CREATE TABLE NameOfTable (" +
                     "Id int," +
                     "username VARCHAR," +
                     "email VARCHAR" +
                     ")");
        commands.Add("INSERT INTO NameOfTable VALUES (3, testusername, test@user.com)");
        commands.Add("SELECT * FROM NameOfTable");
        commands.Add(".exit"); // exit
        var fakeDbFileHandler = new FakeDbFileHandler();
        var repl = new Repl(
            writeLineWrapper, 
            new FakeConsoleInputWrapper(commands), 
            new Table(fakeDbFileHandler, "database"), 
            fakeDbFileHandler);
    
        repl.Start();
    
        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter create statement
            "Executed.",
            "sqlite> ", // enter insert statement
            "Executed.",
            "sqlite> ", // enter select statement
            "Id\tusername\temail",
            "3\ttestusername\ttest@user.com",
            "Executed.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList(), true);
    }

    [Fact]
    public void CanRetrieveInsertedRowAfterClosingRepl()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "INSERT 1 cstack test@user.com",
            ".exit", // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), "database.txt");
        var writeLineWrapperRetrieval = new FakeConsoleWriteLineWrapper();
        var commandsRetrieval = new List<string>
        {
            "SELECT * FROM database",
            ".exit" // exit
        };
        var replRetrieve = new Repl(writeLineWrapperRetrieval, new FakeConsoleInputWrapper(commandsRetrieval), "database.txt");
        repl.Start();

        replRetrieve.Start();

        var retrieveMessage = writeLineWrapperRetrieval.RetrieveMessage();
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter select statement
            "Id\tusername\temail",
            "1\tcstack\ttest@user.com",
            "Executed.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList()); // FIXME: fails on equal, this test isn't doing what you think its doing
    }

    [Fact]
    public void CanInsertMultipleRowsAndSelectInsertedRows()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "INSERT 1 cstack foo1@bar.com",
            "INSERT 2 astack foo2@bar.com",
            "INSERT 3 dstack foo3@bar.com",
            "SELECT * FROM database",
            ".exit", // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

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
        var commands = new List<string>
        {
            "invalid command",
            ".exit", // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

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
        var commands = new List<string>
        {
            ".invalidcommand",
            ".exit", // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

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
        var commands = new List<string>
        {
            "invalid command",
            ".exit", // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

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
    public void InsertNonContiguousRowsDoesNotCrashOnSelect()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "INSERT 1 cstack foo1@bar.com",
            "INSERT 3 dstack foo3@bar.com",
            "SELECT * FROM database",
            ".exit", // exit
        };
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), new Table(new FakeDbFileHandler(), "database"), new FakeDbFileHandler());

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter create statement
            "Executed.",
            "sqlite> ", // enter .exit
            "Id\tusername\temail",
            "1\tcstack\tfoo1@bar.com",
            "3\tdstack\tfoo3@bar.com",
        }, retrieveMessage.ToList());
        Assert.Equal(11, retrieveMessage.Count);
    }
    [Fact]
    public void CanCreateTableExitAndStillSelectTable()
    {
        
        var databaseTableFilename = @"tablesCanRetrieveAfterClosed";
        string fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            databaseTableFilename);
        File.WriteAllText(fullPath + ".txt", "", Encoding.UTF8);
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new List<string>
        {
            "CREATE TABLE RetrievableTable (id int, username varchar, email varchar)",
            ".exit", // exit
        };
        var table = new Table(databaseTableFilename);
        var repl = new Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands), table, new FakeDbFileHandler());
        var writeLineWrapperRetrieval = new FakeConsoleWriteLineWrapper();
        var commandsRetrieval = new List<string>
        {
            "SELECT * FROM RetrievableTable",
            ".exit" // exit
        };
        var replRetrieve = new Repl(writeLineWrapperRetrieval, new FakeConsoleInputWrapper(commandsRetrieval), table, new FakeDbFileHandler());
        repl.Start();

        replRetrieve.Start();

        var retrieveMessage = writeLineWrapperRetrieval.RetrieveMessage();
        Assert.Contains(IntroText, retrieveMessage);
        Assert.Equivalent(new List<string>()
        {
            IntroText, // intro text
            "sqlite> ", // enter select statement
            "Empty Table",
            "Executed.",
            "sqlite> ", // enter .exit
        }, retrieveMessage.ToList(), true);
    }
}