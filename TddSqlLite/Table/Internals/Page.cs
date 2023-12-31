namespace TddSqlLite.Table.Internals;

public class Page
{
    public int PageNum { get; set; }
    public Row[] Rows { get; set; }
}