using System.Text;

namespace TddSqlLite;

public class DbWriter : IDbWriter
{
    public void WriteToDb(IEnumerable<string> contents)
    {
        // wipe file
        File.WriteAllTextAsync(
            "database.txt",
            "",
            Encoding.UTF8).GetAwaiter().GetResult();
        // print new page
        contents.ToList().ForEach(content => 
        File.WriteAllLinesAsync(
            "database.txt",
            new List<string>() { content },
            Encoding.UTF8).GetAwaiter().GetResult());
    }
}