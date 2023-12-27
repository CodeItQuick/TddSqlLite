namespace TddSqlLite;

public interface IDbWriter
{
    public void WriteToDb(IEnumerable<string> contents);
}