namespace TddSqlLite;

public interface IDbFileHandler
{
    public void WriteToDb(IEnumerable<string> contents);
    public string[] ReadFromDb();
}