using TddSqlLite.Database;

namespace Tests;

public class CursorTests
{
    [Fact]
    public void CreateCursorAtBeginningOfTable()
    {
        var table = new Table(new FakeDbFileHandler());
        table.CreateCursorStart();
        
        Assert.Null(table.SelectRow());
    }
    [Fact]
    public void CreateCursorAtBeginningOfTableWithDataInside()
    {
        var table = new Table(new FakeDbFileHandler());
        table.SerializeRow(new Row() { Id = 1, email = "test@user.com", username = "test-user"});

        var numRows = table.CreateCursorStart();

        Assert.Equivalent(
            new Row()
            {
                Id = 1, 
                username = "test-user", 
                email = "test@user.com"
            }, table.SelectRow());
        Assert.Equal(1, numRows);
    }
    [Fact]
    public void CreateCursorAtEndOfTableWithDataInside()
    {
        var table = new Table(new FakeDbFileHandler());
        table.SerializeRow(new Row() { Id = 1, email = "test@user.com", username = "test-user"});
        table.SerializeRow(new Row() { Id = 2, email = "test@user.com", username = "test-user"});
        
        table.CreateCursorEnd();
        
        Assert.Equivalent(
            new Row()
            {
                Id = 2, 
                email = "test@user.com", 
                username = "test-user"
            }, table.SelectRow());
    }
    [Fact]
    public void AccessCursorAtStartOfTableWithDataInside()
    {
        var table = new Table(new FakeDbFileHandler());
        table.SerializeRow(new Row() { Id = 1, email = "test@user.com", username = "test-user"});
        table.SerializeRow(new Row() { Id = 2, email = "test@user.com", username = "test-user"});
        table.CreateCursorStart();

        var selectRow = table.SelectRow();

        Assert.Equivalent(
            new Row()
            {
                Id = 1,
                email = "test@user.com",
                username = "test-user"
            }, selectRow);
    }
    [Fact]
    public void AdvanceCursorAtStartOfTableWithDataInside()
    {
        var table = new Table(new FakeDbFileHandler());
        table.SerializeRow(new Row() { Id = 1, email = "test@user.com", username = "test-user"});
        table.SerializeRow(new Row() { Id = 2, email = "test@user.com", username = "test-user"});
        table.CreateCursorStart();
        
        table.AdvanceCursor();

        var selectRow = table.SelectRow();
        Assert.Equivalent(
            new Row()
            {
                Id = 2,
                email = "test@user.com",
                username = "test-user"
            }, selectRow);
    }
}