using TddSqlLite;

namespace Tests;

public class CursorTests
{
    [Fact]
    public void CreateCursorAtBeginningOfTable()
    {
        var table = new Table(@"database.txt");
        table.CreateCursorStart();
        
        Assert.Null(table.SelectRow());
    }
    [Fact]
    public void CreateCursorAtBeginningOfTableWithDataInside()
    {
        var table = new Table(@"databaseAtStart.txt");
        table.SerializeRow(new Row() { Id = 1, email = "test@user.com", username = "test-user"});
        
        table.CreateCursorStart();
        
        Assert.Equivalent(
            new Row()
            {
                Id = 1, 
                username = "test-user", 
                email = "test@user.com"
            }, table.SelectRow());
    }
    [Fact]
    public void CreateCursorAtEndOfTableWithDataInside()
    {
        var cursor = new Cursor();

        var table = new Table(@"cursorAtEnd.txt");
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
        var table = new Table(@"cursorAtEnd.txt");
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
        var table = new Table(@"cursorAtEnd.txt");
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