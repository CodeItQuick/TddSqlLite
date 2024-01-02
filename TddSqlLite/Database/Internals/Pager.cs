using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;

namespace TddSqlLite.Database.Internals;

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
    private BTree _bTree = new();

    public void AppendPage(Row row)
    {
        _bTree.AddNode(row);
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
        var allNodes = _bTree.GetNodes();
        // return allNodes.Select(x => JsonSerializer.Serialize(x)).ToList();
        return allNodes.Values.Select(
                x =>
                    string.Concat(JsonSerializer.Serialize(x.Values)))
            .ToList();
    }

    public int CountRows()
    {
        return _bTree.GetAllNodes().Count;
    }

    public Row? SelectRow(Cursor cursorRow)
    {
        return _bTree.GetNodeIdx(cursorRow.RowNum);
    }
}