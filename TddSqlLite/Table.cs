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
    private const int MAX_ROWS_PER_PAGE = 14;
    private const int MAX_PAGES = 100;

    public void SerializeRow(Row row)
    {
        if (_pages.Length == MAX_PAGES)
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

    public Row DeserializeRow(Page page, Row row)
    {
        return _pages[page.PageNum].Rows.First(x => x.Id == row.Id);
    }

    public Row[] DeserializePage(Page page)
    {
        if (page.PageNum < _pages.Length)
        {
            var currentPage = _pages[page.PageNum];
            return currentPage.Rows.ToArray();
        }

        return Array.Empty<Row>();
    }
}