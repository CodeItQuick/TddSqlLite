using TddSqlLite;

namespace Tests;

public class Repl
{
    [Fact]
    public void MainProducesTextOutput()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var repl = new TddSqlLite.Repl(writeLineWrapper, new Stack<string>());

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
        commands.Push("create table users (id int, username varchar(255), email varchar(255));");
        var repl = new TddSqlLite.Repl(writeLineWrapper, commands);

        repl.Start();

        var retrieveMessage = writeLineWrapper.RetrieveMessage();
        Assert.Contains("SQLite version 3.16.0 2016-11-04 19:09:39" +
                        "Enter \".help\" for usage hints.\nConnected to a transient " +
                        "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                        "database.", retrieveMessage);
        Assert.Contains("sqlite> " + "create table users (id int, username varchar(255), email varchar(255));", retrieveMessage);
        Assert.Equal(3, retrieveMessage.Count);
    }
}