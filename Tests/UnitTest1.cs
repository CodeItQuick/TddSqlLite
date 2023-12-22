using TddSqlLite;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public void MainProducesTextOutput()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var repl = new Repl(writeLineWrapper, new List<string>());

        repl.Start();
        
        Assert.Equal("SQLite version 3.16.0 2016-11-04 19:09:39" +
                     "Enter \".help\" for usage hints.\nConnected to a transient " +
                     "in-memory database.\nUse \".open FILENAME\" to reopen on a persistent " +
                     "database.", writeLineWrapper.RetrieveMessage());
    }
    [Fact]
    public void CanStartReplWithCommands()
    {
        var writeLineWrapper = new FakeConsoleWriteLineWrapper();
        var repl = new Repl(writeLineWrapper, new List<string>()
        {
            "create table users (id int, username varchar(255), email varchar(255));"
        });

        repl.Start();
        
        Assert.Equal("sqlite> " + "create table users (id int, username varchar(255), email varchar(255));", writeLineWrapper.RetrieveMessage());
    }
}