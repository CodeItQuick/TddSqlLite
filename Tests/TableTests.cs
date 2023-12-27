using TddSqlLite;

namespace Tests;

public class TableTests
{
    [Fact]
    public void CannotInsertRowWithTooLongUsername()
    {
        var table = new Table();
        var row = new Row() { Id = 1, email = "test@user.com", username = string.Concat(Enumerable.Repeat("a", 256))};
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        Assert.Throws<Exception>(() => table.SerializeRow(row));
    }
    [Fact]
    public void CannotInsertRowWithTooLongEmail()
    {
        var table = new Table();
        var row = new Row() { Id = 1, email = string.Concat(Enumerable.Repeat("a", 256)), username = "testuser"};
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        Assert.Throws<Exception>(() => table.SerializeRow(row));
    }
    [Fact]
    public void CannotInsertRowWithNegativeId()
    {
        var table = new Table();
        var row = new Row() { Id = -1, email = "test@user.com", username = "testuser"};
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        Assert.Throws<Exception>(() => table.SerializeRow(row));
    }
    [Fact]
    public void CanInsertRowsIntoTable()
    {
        var table = new Table();
        var row = new Row() { Id = 1, email = "test@user.com", username = "test_user" };
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        table.SerializeRow(row);

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
            Rows = Array.Empty<Row>()
        };
        table.SerializeRow(row1);
        table.SerializeRow(row2);

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
    public void CanInsertRowsIntoExistingPageAndRetrieveAllAvailable()
    {
        var table = new Table();
        var row1 = new Row()
        {
            Id = 1, email = "test@user.com", username = "test_user"
        };
        var row2 = new Row()
        {
            Id = 2, email = "test@user.com", username = "test_user"
        };
        var page = new Page() { 
            PageNum = 0, 
            Rows = Array.Empty<Row>()
        };
        table.SerializeRow(row1);
        table.SerializeRow(row2);

        Assert.Equivalent(
            new Row[] {
                new()
                {
                    Id = 1, email = "test@user.com", username = "test_user",
                },
                new()
                {
                    Id = 2, email = "test@user.com", username = "test_user",
                },
            },
            table.DeserializePage(page));
    }
    [Fact]
    public void AddsToNextPageWhenFull()
    {
        var table = new Table();
        var rows = Enumerable
            .Range(1, 15)
            .Select(x => new Row()
            {
                Id = x, email = "test@user.com", username = "test_user"
            })
            .ToArray();
        foreach (var row in rows)
        {
            table.SerializeRow(row);
        }
        
        Assert.Equal(14, table.DeserializePage(new Page() { PageNum = 0 }).Length);
        Assert.Single(table.DeserializePage(new Page() { PageNum = 1 }));
    }
    [Fact]
    public void CanFillTable()
    {
        var table = new Table();
        var rows = Enumerable
            .Range(1, 1400)
            .Select(x => new Row()
            {
                Id = x, email = "test@user.com", username = "test_user"
            })
            .ToArray();
        foreach (var row in rows)
        {
            table.SerializeRow(row);
        }
        
        Assert.Equal(14, table.DeserializePage(new Page() { PageNum = 99 }).Length);
    }
    [Fact]
    public void FullTableThrowsError()
    {
        var table = new Table();
        var rows = Enumerable
            .Range(1, 1400)
            .Select(x => new Row()
            {
                Id = x, email = "test@user.com", username = "test_user"
            })
            .ToArray();
        foreach (var row in rows)
        {
            table.SerializeRow(row);
        }
        
        Assert.Throws<Exception>(() => table.SerializeRow(new Row()
        {
            Id = 1401, email = "test@user.com", username = "test_user"
        }));
    }
    [Fact]
    public void AddsToNextPageTwiceWhenFull()
    {
        var table = new Table();
        var rows = Enumerable
            .Range(1, 16)
            .Select(x => new Row()
            {
                Id = x, email = "test@user.com", username = "test_user"
            })
            .ToArray();
        foreach (var row in rows)
        {
            table.SerializeRow(row);
        }
        
        Assert.Equal(14, table.DeserializePage(new Page() { PageNum = 0 }).Length);
        Assert.Equal(2, table.DeserializePage(new Page() { PageNum = 1 }).Length);
    }
}