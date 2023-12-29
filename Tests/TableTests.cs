using System.Text.Json;
using TddSqlLite;

namespace Tests;

public class TableTests
{
    [Fact]
    public void CannotInsertRowWithTooLongUsername()
    {
        var table = new Table(@"database.txt");
        var row = new Row() { Id = 1, email = "test@user.com", username = string.Concat(Enumerable.Repeat("a", 256))};
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        Assert.Throws<Exception>(() => table.SerializeRow(row));
    }
    [Fact]
    public void CannotInsertRowWithTooLongEmail()
    {
        var table = new Table(@"database.txt");
        var row = new Row() { Id = 1, email = string.Concat(Enumerable.Repeat("a", 256)), username = "testuser"};
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        Assert.Throws<Exception>(() => table.SerializeRow(row));
    }
    [Fact]
    public void CannotInsertRowWithNegativeId()
    {
        var table = new Table(@"database.txt");
        var row = new Row() { Id = -1, email = "test@user.com", username = "testuser"};
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        Assert.Throws<Exception>(() => table.SerializeRow(row));
    }
    [Fact]
    public void CanInsertRowsIntoTable()
    {
        var table = new Table(new FakeDbFileHandler());
        var row = new Row() { Id = 1, email = "test@user.com", username = "test_user" };
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        table.SerializeRow(row);

        Assert.Equivalent(new Row()
            {
                Id = 1, email = "test@user.com", username = "test_user"
            },
            table.DeserializeRow(page.PageNum, row.Id));
    }
    [Fact]
    public void CanInsertRowsIntoExistingPage()
    {
        var table = new Table(@"database.txt");
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
            table.DeserializeRow(page.PageNum, row1.Id));
        Assert.Equivalent(
            new Row()
            {
                Id = 2, email = "test@user.com", username = "test_user"
            },
            table.DeserializeRow(page.PageNum, row2.Id));
    }
    [Fact]
    public void CanInsertRowsIntoExistingPageAndRetrieveAllAvailable()
    {
        var table = new Table(@"database.txt");
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
            table.DeserializePage(page.PageNum));
    }
    [Fact]
    public void AddsToNextPageWhenFull()
    {
        var table = new Table(@"database.txt");
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

        Page page = new Page() { PageNum = 0 };
        Assert.Equal(14, table.DeserializePage(page.PageNum).Length);
        Page page1 = new Page() { PageNum = 1 };
        Assert.Single(table.DeserializePage(page1.PageNum));
    }
    [Fact]
    public void CanFillTable()
    {
        var table = new Table(new FakeDbFileHandler());
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

        Page page = new Page() { PageNum = 99 };
        Assert.Equal(14, table.DeserializePage(page.PageNum).Length);
    }
    [Fact]
    public void FullTableThrowsError()
    {
        var table = new Table(new FakeDbFileHandler());
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
        var table = new Table(new FakeDbFileHandler());
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

        Page page = new Page() { PageNum = 0 };
        Assert.Equal(14, table.DeserializePage(page.PageNum).Length);
        Page page1 = new Page() { PageNum = 1 };
        Assert.Equal(2, table.DeserializePage(page1.PageNum).Length);
    }
    [Fact]
    public void CanSavePageToFile()
    {
        var fakeDbWriter = new FakeDbFileHandler();
        var table = new Table(fakeDbWriter);
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

        var initialResult = string.Concat(JsonSerializer.Serialize(rows[..14]));
        Assert.Equal(initialResult, fakeDbWriter.RetrieveMessage()[0]);
        var finalResult = string.Concat(
            JsonSerializer.Serialize(rows[14..16]));
        Assert.Equal(finalResult, fakeDbWriter.RetrieveMessage()[1]);
    }
    [Fact]
    public void InRealFileCanSavePageToFile()
    {
        var table = new Table(@"database.txt");
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

        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string fullPath = Path.Combine(path, @"database.txt");
        var readAllLines = File.ReadAllLines(fullPath);
        Assert.Equal("[{\"Id\":1,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":2,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":3,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":4,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":5,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":6,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":7,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":8,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":9,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":10,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":11,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":12,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":13,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":14,\"username\":\"test_user\",\"email\":\"test@user.com\"}]", 
            readAllLines.First());
        Assert.Equal("[{\"Id\":15,\"username\":\"test_user\",\"email\":\"test@user.com\"},{\"Id\":16,\"username\":\"test_user\",\"email\":\"test@user.com\"}]", 
            readAllLines.Skip(1).First());
    }
}