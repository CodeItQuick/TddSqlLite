using System.Text;
using System.Text.Json;
using TddSqlLite.Database;
using TddSqlLite.Database.Internals;

namespace Tests;

public class TableTests
{
    [Fact]
    public void CannotInsertRowWithTooLongUsername()
    {
        Assert.Throws<Exception>(() => new Row() { Id = 1, email = "test@user.com", username = string.Concat(Enumerable.Repeat("a", 256))});
    }
    [Fact]
    public void CannotInsertRowWithTooLongEmail()
    {
        Assert.Throws<Exception>(() => new Row()
        {
            Id = 1, 
            email = string.Concat(Enumerable.Repeat("a", 256)), 
            username = "testuser"
        });
    }
    [Fact]
    public void CannotInsertRowWithNegativeId()
    {
        var table = new Table(new FakeDbFileHandler(), "database");
        var row = new Row() { Id = -1, email = "test@user.com", username = "testuser"};
        var page = new Page() { PageNum = 0, Rows = Array.Empty<Row>()};
        Assert.Throws<Exception>(() => table.SerializeRow(row));
    }
    [Fact]
    public void CanInsertRowsIntoTable()
    {
        var table = new Table(new FakeDbFileHandler(), "database");
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
        var table = new Table(new FakeDbFileHandler(), "database");
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
        var table = new Table(new FakeDbFileHandler(), "database");
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
    public void AddSortedNodesToBeTree()
    {
        var sortedList = new SortedList<int, Row>()
        {
            [1] = new() { Id = 1, email = "test@user.com", username = "test-user" },
            [2] = new() { Id = 2, email = "test@user.com", username = "test-user" },
            [3] = new() { Id = 3, email = "test@user.com", username = "test-user" },
            [4] = new() { Id = 4, email = "test@user.com", username = "test-user" },
            [5] = new() { Id = 5, email = "test@user.com", username = "test-user" },
            [6] = new() { Id = 6, email = "test@user.com", username = "test-user" },
            [7] = new() { Id = 7, email = "test@user.com", username = "test-user" },
            [8] = new() { Id = 8, email = "test@user.com", username = "test-user" },
            [9] = new() { Id = 9, email = "test@user.com", username = "test-user" },
            [10] = new() { Id = 10, email = "test@user.com", username = "test-user" },
            [11] = new() { Id = 11, email = "test@user.com", username = "test-user" },
            [12] = new() { Id = 12, email = "test@user.com", username = "test-user" },
            [13] = new() { Id = 13, email = "test@user.com", username = "test-user" },
            [14] = new() { Id = 14, email = "test@user.com", username = "test-user" },
            [15] = new() { Id = 15, email = "test@user.com", username = "test-user" },
            [18] = new() { Id = 18, email = "test@user.com", username = "test-user" },
        };
        var bTree = new BTree();
        
        bTree.RepopulateNodes(sortedList);

        Assert.Equal(1, bTree.GetNode(1).Id);
        Assert.Equal(2, bTree.GetNode(2).Id);
        Assert.Equal(3, bTree.GetNode(3).Id);
        Assert.Equal(4, bTree.GetNode(4).Id);
        Assert.Equal(5, bTree.GetNode(5).Id);
        Assert.Equal(6, bTree.GetNode(6).Id);
        Assert.Equal(7, bTree.GetNode(7).Id);
        Assert.Equal(8, bTree.GetNode(8).Id);
        Assert.Equal(9, bTree.GetNode(9).Id);
        Assert.Equal(10, bTree.GetNode(10).Id);
        Assert.Equal(11, bTree.GetNode(11).Id);
        Assert.Equal(12, bTree.GetNode(12).Id);
        Assert.Equal(13, bTree.GetNode(13).Id);
        Assert.Equal(14, bTree.GetNode(14).Id);
        Assert.Equal(15, bTree.GetNode(15).Id);
        Assert.Equal(18, bTree.GetNode(18).Id);
    }
    [Fact]
    public void AddsToNextPageWhenFull()
    {
        var table = new Table(new FakeDbFileHandler(), "database");
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
        Assert.Equal(4, table.DeserializePage(page.PageNum).Length);
        Page page1 = new Page() { PageNum = 1 };
        Assert.Equal(4, table.DeserializePage(page1.PageNum).Length);
        Page page2 = new Page() { PageNum = 2 };
        Assert.Equal(4, table.DeserializePage(page2.PageNum).Length);
        Page page3 = new Page() { PageNum = 3 };
        Assert.Equal(3, table.DeserializePage(page3.PageNum).Length);
    }
    [Fact]
    public void CanFillTable()
    {
        var table = new Table(new FakeDbFileHandler(), "database");
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

        var numRows = table.CreateCursorStart();
        Assert.Equal(1400, numRows);
    }
    [Fact]
    public void FullTableThrowsError()
    {
        var table = new Table(new FakeDbFileHandler(), "database");
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
        var table = new Table(new FakeDbFileHandler(), "database");
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
        Assert.Equal(4, table.DeserializePage(page.PageNum).Length);
        Page page1 = new Page() { PageNum = 1 };
        Assert.Equal(4, table.DeserializePage(page1.PageNum).Length);
        Page page2 = new Page() { PageNum = 2 };
        Assert.Equal(4, table.DeserializePage(page2.PageNum).Length);
        Page page3 = new Page() { PageNum = 3 };
        Assert.Equal(4, table.DeserializePage(page3.PageNum).Length);
    }
    [Fact]
    public void CanSavePageToFile()
    {
        var fakeDbWriter = new FakeDbFileHandler();
        var table = new Table(fakeDbWriter, "database");
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

        Assert.Contains("[{\"Id\":1,\"email\":\"test@user.com\",\"username\":\"test_user\"}", fakeDbWriter.RetrieveMessage()[0]);
        Assert.Contains("[{\"Id\":5,\"email\":\"test@user.com\",\"username\":\"test_user\"}", fakeDbWriter.RetrieveMessage()[1]);
        Assert.Contains("[{\"Id\":9,\"email\":\"test@user.com\",\"username\":\"test_user\"}", fakeDbWriter.RetrieveMessage()[2]);
        Assert.Contains("[{\"Id\":13,\"email\":\"test@user.com\",\"username\":\"test_user\"}", fakeDbWriter.RetrieveMessage()[3]);
    }
    [Fact]
    public void CanRetrieveAlreadySavedFile()
    {
        var databaseTableFilename = @"canRetrieveSaveFile";
        string fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                databaseTableFilename + ".txt");
        var contents = 
            "[{\"Id\":1,\"username\":\"test_user\",\"email\":\"test@user.com\"}," +
            "{\"Id\":2,\"username\":\"test_user\",\"email\":\"test@user.com\"}]";
        File.WriteAllText(fullPath, contents, Encoding.UTF8);
        var table = new Table(databaseTableFilename);
        var rows = Enumerable
            .Range(3, 14)
            .Select(x => new Row()
            {
                Id = x, email = "test@user.com", username = "test_user"
            })
            .ToArray();
        foreach (var row in rows)
        {
            table.SerializeRow(row);
        }

        var readAllLines = File.ReadAllLines(fullPath);
        Assert.Contains("{\"Id\":1,\"username\":\"test_user\",\"email\":\"test@user.com\"}", 
            readAllLines.First());
        Assert.Contains("[{\"Id\":5,\"email\":\"test@user.com\",\"username\":\"test_user\"}", 
            readAllLines.Skip(1).First());
        Assert.Contains("[{\"Id\":9,\"email\":\"test@user.com\",\"username\":\"test_user\"}", 
            readAllLines.Skip(2).First());
        Assert.Contains("[{\"Id\":13,\"email\":\"test@user.com\",\"username\":\"test_user\"}", 
            readAllLines.Skip(3).First());
    }
    [Fact]
    public void CanRetrieveAlreadySavedFileWithOneRecord()
    {
        var databaseTableFilename = @"canRetrieveSaveFile";
        string fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                databaseTableFilename + ".txt");
        var contents = 
            "[{\"Id\":1,\"username\":\"test_user\",\"email\":\"test@user.com\"}]";
        File.WriteAllText(fullPath, contents, Encoding.UTF8);
        var table = new Table(databaseTableFilename);
        var rows = Enumerable
            .Range(2, 15)
            .Select(x => new Row()
            {
                Id = x, email = "test@user.com", username = "test_user"
            })
            .ToArray();
        foreach (var row in rows)
        {
            table.SerializeRow(row);
        }

        var readAllLines = File.ReadAllLines(fullPath);
        Assert.Contains("{\"Id\":1,\"username\":\"test_user\",\"email\":\"test@user.com\"}", 
            readAllLines.First());
        Assert.Contains("[{\"Id\":5,\"email\":\"test@user.com\",\"username\":\"test_user\"}", 
            readAllLines.Skip(1).First());
        Assert.Contains("[{\"Id\":9,\"email\":\"test@user.com\",\"username\":\"test_user\"}", 
            readAllLines.Skip(2).First());
        Assert.Contains("[{\"Id\":13,\"email\":\"test@user.com\",\"username\":\"test_user\"}", 
            readAllLines.Skip(3).First());
    }
    [Fact]
    public void InRealFileCanSavePageToFile()
    {
        var databaseTableFilename = @"databaseCanSavePageToFile";
        string fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            databaseTableFilename + ".txt");
        File.WriteAllText(fullPath, "", Encoding.UTF8);
        var table = new Table(databaseTableFilename);
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

        var readAllLines = File.ReadAllLines(fullPath);
        Assert.Contains("[{\"Id\":1,\"email\":\"test@user.com\",\"username\":\"test_user\"}", readAllLines.First());
        Assert.Contains("[{\"Id\":5,\"email\":\"test@user.com\",\"username\":\"test_user\"}", readAllLines.Skip(1).First());
        Assert.Contains("[{\"Id\":9,\"email\":\"test@user.com\",\"username\":\"test_user\"}", readAllLines.Skip(2).First());
        Assert.Contains("[{\"Id\":13,\"email\":\"test@user.com\",\"username\":\"test_user\"}", readAllLines.Skip(3).First());
    }
    [Fact]
    public void CanConstructRowWithId()
    {
        var table = new Table(new FakeDbFileHandler(), "user");
        var row = new Row()
            {
                DynamicColumns = new Dictionary<string, object>()
                {
                    ["Id"] = 1
                }, 
                email = "test@user.com", 
                username = "test_user"
            };
        table.SerializeRow(row);

        var readAllLines = table.DeserializePage(0).ToList().First();
        Assert.Equivalent(new Row()
            {
                Id = 1, 
                email = "test@user.com", 
                username = "test_user"
            }, 
            readAllLines, true);
    }
    [Fact]
    public void CanConstructRowFromString()
    {
        var storedValues = "[{\"Id\": 1, \"username\": \"test_user\", \"email\": \"test@user.com\"}]";
        var deserialize = JsonSerializer.Deserialize<Row[]>(storedValues);
        Assert.Equivalent(
            new List<Row>() { new() { Id = 1, username = "test_user", email = "test@user.com" } }, 
            deserialize, 
            true);
    }
    [Fact]
    public void CanConstructRowWithUsername()
    {
        var table = new Table(new FakeDbFileHandler(), "user");
        var row = new Row()
            {
                DynamicColumns = new Dictionary<string, object>()
                {
                    ["username"] = "test_user"
                }, 
                email = "test@user.com", 
                Id = 1
            };
        table.SerializeRow(row);

        var readAllLines = table.DeserializePage(0).ToList().First();
        Assert.Equivalent(new Row()
            {
                Id = 1, 
                email = "test@user.com", 
                username = "test_user"
            }, 
            readAllLines, true);
    }
    [Fact]
    public void CanConstructRowWithEmail()
    {
        var table = new Table(new FakeDbFileHandler(), "user");
        var row = new Row()
            {
                DynamicColumns = new System.Collections.Generic.Dictionary<string, object>()
                {
                    ["email"] = "test@user.com"
                }, 
                username = "test_user", 
                Id = 1
            };
        table.SerializeRow(row);

        var readAllLines = table.DeserializePage(0).ToList().First();
        Assert.Equivalent(new Row()
            {
                Id = 1, 
                email = "test@user.com", 
                username = "test_user"
            }, 
            readAllLines, true);
    }
    [Fact]
    public void CanConstructRowWithCustomStringField()
    {
        var fakeDbFileHandler = new FakeDbFileHandler();
        var table = new Table(fakeDbFileHandler, "user");
        var row = new Row()
            {
                DynamicColumns = new Dictionary<string, object>()
                {
                    ["CustomProperty"] = "test_property"
                }, 
                Id = 1
            };
        table.SerializeRow(row);

        var readAllLines = string.Concat(fakeDbFileHandler.ReadFromDb());
        Assert.Equivalent(
            "[{\"CustomProperty\":\"test_property\",\"Id\":1}]", readAllLines, true);
    }
    [Fact]
    public void CanConstructSystemTable()
    {
        var fakeDbFileHandler = new FakeDbFileHandler();
        var table = new Table(fakeDbFileHandler, "system");
        var row = new Row()
        {
            DynamicColumns = new Dictionary<string, object>()
            {
                ["TableName"] = "enteredtable"
            }, 
            Id = 1
        };
        
        table.SerializeRow(row);

        var readAllLines = string.Concat(fakeDbFileHandler.ReadFromDb());
        Assert.Equivalent(
            "[{\"TableName\":\"enteredtable\",\"Id\":1}]", readAllLines, true);
    }
}