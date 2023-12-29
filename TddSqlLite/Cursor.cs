using TddSqlLite;

namespace Tests;

public class Cursor
{
    public int RowNum { get; set; }
    public int EndOfTable { get; set; }
    public Table Table { get; set; } = new(@"database.txt");
}