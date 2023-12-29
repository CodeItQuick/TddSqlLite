using System.Text.Json;
using Tests;

namespace TddSqlLite;

public class Pager
{
    private Page[] _pages =
    {
        new()
        {
            PageNum = 0,
            Rows = Array.Empty<Row>()
        }
    };
    private const int MAX_ROWS_PER_PAGE = 14;
    private const int MAX_PAGES = 100;

    public void AppendPage(Row row)
    {
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
    }

    public List<string> RetrieveAllRows()
    {   
        return _pages.Select(
                x =>
                    string.Concat(JsonSerializer.Serialize(x.Rows)))
            .ToList();
    }

    public int CountRows()
    {
        return _pages.Aggregate(0, (acc, curr) => acc + curr.Rows.Length);
    }

    public Row? SelectRow(Cursor cursorRow)
    {
        return _pages.FirstOrDefault(x => 
                x.Rows.Any(y => y.Id == cursorRow.RowNum + 1))
            ?.Rows
            .FirstOrDefault(x => x.Id == cursorRow.RowNum + 1);
    }
}