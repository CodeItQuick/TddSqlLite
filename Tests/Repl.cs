using TddSqlLite;

namespace Tests;

public class Repl
{
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
    public void CanStartReplWithCommands()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(""); // exit
        commands.Push("create table users (id int, username varchar(255), email varchar(255));");
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("sqlite> ", retrieveMessage);
        Assert.Equal(3, retrieveMessage.Count);
    }
    [Fact]
    public void CanRecognizeWhenCommandInvalid()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var commands = new Stack<string>();
        commands.Push(""); // exit
        commands.Push("invalid command");
        var repl = new TddSqlLite.Repl(writeLineWrapper, new FakeConsoleInputWrapper(commands));

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("Unrecognized command 'invalid command'.", retrieveMessage);
        Assert.Equal(2, retrieveMessage.Count(x => x == "sqlite> ")); // two of these
        Assert.Equal(4, retrieveMessage.Count);
    }
}