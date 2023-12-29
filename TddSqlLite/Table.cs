using System.Text;
using System.Text.Json;
using Tests;

namespace TddSqlLite;

public class Table
{
    private readonly Pager _pager = new();
    private readonly IDbFileHandler _dbFileHandler;
    private Cursor _currentCursor;

    public Table(string databaseTableFilename)
    {
        _dbFileHandler = new DbTableFileHandler(databaseTableFilename);
    }

    public Table(IDbFileHandler dbFileHandler)
    {
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

    public void CreateCursorStart()
    {
        _currentCursor = new Cursor()
        {
            RowNum = 0, 
            Table = this, 
            EndOfTable = _pager.CountRows() - 1
        };
    }

    public void CreateCursorEnd()
    {
        var endOfTable = _pager.CountRows();
        _currentCursor = new Cursor()
        {
            RowNum = endOfTable - 1, 
            Table = this, 
            EndOfTable = endOfTable - 1
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
}