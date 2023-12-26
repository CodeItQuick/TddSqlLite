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
        page.Rows = page.Rows.Append(row).ToArray();
        _pages = _pages.Append(page).ToArray();
    }

    public Row DeserializeRow(Page page, Row row)
    {
        return _pages[page.PageNum].Rows.First(x => x.Id == row.Id);
    }
}