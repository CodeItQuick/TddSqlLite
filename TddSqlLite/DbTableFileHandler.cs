using System.Text;

namespace TddSqlLite;

public class DbTableFileHandler : IDbFileHandler
{
    private string _fullPath;

    public DbTableFileHandler(string filename)
    {
        _fullPath = Path.Combine(Environment
            .GetFolderPath(Environment.SpecialFolder.ApplicationData), filename);
    }

    public void WriteToDb(IEnumerable<string> contents)
    {
        // wipe file
        File.WriteAllTextAsync(
            _fullPath,
            "",
            Encoding.UTF8).GetAwaiter().GetResult();
        // print new page
        contents.ToList().ForEach(content => 
        File.AppendAllLines(
            _fullPath,
            new List<string>() { content },
            Encoding.UTF8));
    }

    public string[] ReadFromDb()
    {
        return File.ReadAllLines(_fullPath);
    }
}