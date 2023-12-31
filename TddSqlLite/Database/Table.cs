using System.Text.Json;
using TddSqlLite.Database.Internals;

namespace TddSqlLite.Database;

public class Table
{
    private readonly Pager _pager = new();
    private readonly IDbFileHandler _dbFileHandler;
    private Cursor _currentCursor;
    private readonly string _tableName;

    public Table(string tableName)
    {
        _tableName = tableName;
        _dbFileHandler = new DbTableFileHandler(tableName + ".txt");
        var existingData = _dbFileHandler.ReadFromDb();
        var rows = existingData.Select(x =>  
            JsonSerializer.Deserialize<Row[]>(x))
            .ToList();
        if (rows.Count > 0)
        {
            var insertRows = rows.SelectMany(x => x.ToList()).ToList();
            insertRows.ForEach(SerializeRow);
        }
    }

    public Table(IDbFileHandler dbFileHandler, string tableName)
    {
        _tableName = tableName;
        _dbFileHandler = dbFileHandler;
    }

    public void SerializeRow(Row row)
    {
        if (row.Id < 1)
        {
            throw new Exception("Id must be a positive number");
        }

        if (row.username.Length > 255)
        {
            throw new Exception("username string too many characters.");
        }

        if (row.email.Length > 255)
        {
            throw new Exception("username string too many characters.");
        }

        _pager.AppendPage(row);
        var contents = _pager.RetrieveAllRows();

        _dbFileHandler.WriteToDb(contents);
    }

    public Row DeserializeRow(int pageNum, int rowId)
    {
        string[] readFromDb = _dbFileHandler.ReadFromDb();
        var rows = readFromDb.ToList().Select(x => 
            JsonSerializer.Deserialize<Row[]>(x)).ToList();
        var deserializeRow = rows
            .Skip(pageNum)
            .FirstOrDefault()
            ?.First(x => x.Id == rowId);
        return deserializeRow ?? new Row();
    }

    public Row[] DeserializePage(int pagePageNum)
    {
        string[] readFromDb = _dbFileHandler.ReadFromDb();
        var rows = readFromDb.ToList().Select(x => 
            JsonSerializer.Deserialize<Row[]>(x)).ToList();
        return rows.Skip(pagePageNum).First() ?? Array.Empty<Row>();
    }

    public int CreateCursorStart()
    {
        _currentCursor = new Cursor()
        {
            RowNum = 0
        };
        return _pager.CountRows();
    }

    public void CreateCursorEnd()
    {
        var endOfTable = _pager.CountRows();
        _currentCursor = new Cursor()
        {
            RowNum = endOfTable - 1,
        };
    }

    public Row? SelectRow()
    {
        return _pager.SelectRow(_currentCursor);  
    }

    public void AdvanceCursor()
    {
        _currentCursor.RowNum++;
    }

    public bool IsTableName(string requestingTableName)
    {
        return _tableName == requestingTableName;
    }
}