using System.Text;
using System.Text.Json;

namespace TddSqlLite;

public class Table
{
    private Page[] _pages  = {
        new()
        {
            PageNum = 0,
            Rows = Array.Empty<Row>()
        }
    };

    private IDbWriter _dbWriter = new DbWriter();
    private const int MAX_ROWS_PER_PAGE = 14;
    private const int MAX_PAGES = 100;

    public Table()
    {
    }

    public Table(IDbWriter dbWriter)
    {
        _dbWriter = dbWriter;
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
        if (_pages.Length == MAX_PAGES && 
            _pages[^1].Rows.Length == MAX_ROWS_PER_PAGE)
        {
            throw new Exception("Table Full");
        }
        if (_pages[^1].Rows.Length == MAX_ROWS_PER_PAGE)
        {
            _pages = _pages.Append(
                new Page()
            {
                PageNum = _pages.Length - 1, 
                Rows = Array.Empty<Row>()
            }).ToArray();
        }
        _pages[^1].Rows = _pages[^1].Rows.Append(row).ToArray();
        var contents = _pages.Select(
            x =>  
            string.Concat(JsonSerializer.Serialize(x.Rows)))
            .ToList();
        _dbWriter.WriteToDb(contents);
    }

    public Row DeserializeRow(Page page, Row row)
    {
        return _pages[page.PageNum].Rows.First(x => x.Id == row.Id);
    }

    public Row[] DeserializePage(Page page)
    {
        if (page.PageNum >= _pages.Length)
        {
            return Array.Empty<Row>();
        };
        var currentPage = _pages[page.PageNum];
        return currentPage.Rows.ToArray();
    }
}