using TddSqlLite;

namespace Tests;

public class FakeDbWriter : IDbWriter
{
    private readonly List<string> _message = new();

    public void WriteToDb(IEnumerable<string> contents)
    {
        contents.ToList().ForEach(content => _message.Add(content));
    }

    public List<string> RetrieveMessage()
    {
        return _message;
    }
}