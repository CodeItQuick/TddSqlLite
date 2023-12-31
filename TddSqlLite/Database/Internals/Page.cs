namespace TddSqlLite.Database.Internals;

public class Page
{
    public int PageNum { get; set; }
    public Row[] Rows { get; set; }
}