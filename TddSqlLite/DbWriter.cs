using System.Text;

namespace TddSqlLite;

public class DbWriter : IDbWriter
{
    public void WriteToDb(IEnumerable<string> contents)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string fullPath = Path.Combine(path, @"database.txt");
        // wipe file
        File.WriteAllTextAsync(
            fullPath,
            "",
            Encoding.UTF8).GetAwaiter().GetResult();
        // print new page
        contents.ToList().ForEach(content => 
        File.AppendAllLines(
            fullPath,
            new List<string>() { content },
            Encoding.UTF8));
    }
}