namespace TddSqlLite;

public class Table
{
    private Page[] _pages = Array.Empty<Page>();
    private const int MAX_PAGES = 100;

    public void SerializeRow(Row row, Page page)
    {
        if (page.PageNum > MAX_PAGES)
        {
            throw new Exception("Max page size of table exceeded");
        }

        if (page.Rows.Length > 1000)
        {
            throw new Exception("This page is full");
        }
        if (_pages.Length == 0)
        {
            _pages = new[]
            {
                new Page()
                {
                    PageNum = 0,
                    Rows = new []{ row }
                }
            };
        }
        _pages[page.PageNum].Rows = _pages[page.PageNum].Rows.Append(row).ToArray();
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