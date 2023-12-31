using TddSqlLite;
using Index = System.Index;

namespace Tests;

public class FakeDbFileHandler : IDbFileHandler
{
    private Dictionary<Index, Dictionary<int, string>> _message = new();

    public void WriteToDb(IEnumerable<string> contents)
    {
        var contentsDict = new Dictionary<int, string>();
        int idx = 0;
        contents.ToList().ForEach((x) => contentsDict.Add(idx++, x));
        _message.Add(_message.Count, contentsDict);
    }

    public string[] ReadFromDb()
    {
        return _message[_message.Keys.Last()].Values.ToArray();
    }

    public void InjectFilename(string filename)
    {
        _message = new Dictionary<Index, Dictionary<int, string>>();
    }

    public List<string> RetrieveMessage()
    {
        return _message[_message.Keys.Last()].Values.ToList();
    }
}