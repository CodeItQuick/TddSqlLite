using System.Text.Json.Serialization;

namespace TddSqlLite.Database;

public class Row
{
    [JsonIgnore]
    public Dictionary<string, string>? StringColumns { get; set; }
    [JsonIgnore]
    public Dictionary<string, int>? IntColumns { get; init; }
    private int _id;
    public int Id
    {
        get
        {
            int idColumn = 0;
            var idExists = IntColumns?.TryGetValue("Id", out idColumn);
            if (idExists is true)
            {
                return idColumn;
            }
            return _id;
        }
        set
        {
            _id = value;
            int idColumn = 0;
            var idExists = IntColumns?.TryGetValue("Id", out idColumn);
            if (idExists is true)
            {
                _id = idColumn;
            }
        }
    }

    private string _username;
    public string username 
    {
        get
        {
            string idColumn = "";
            var idExists = StringColumns?.TryGetValue("username", out idColumn);
            if (idExists is true)
            {
                return idColumn;
            }
            return _username;
        }
        set
        {
            _username = value;
            string idColumn = "";
            var idExists = StringColumns?.TryGetValue("username", out idColumn);
            if (idExists is true)
            {
                _username = idColumn;
            }
        }
    }

    private string _email;
    public string email 
    { 
        get
        {
            string idColumn = "";
            var idExists = StringColumns?.TryGetValue("email", out idColumn);
            if (idExists is true)
            {
                return idColumn;
            }
            return _email;
        }
        set
        {
            _email = value;
            string idColumn = "";
            var idExists = StringColumns?.TryGetValue("email", out idColumn);
            if (idExists is true)
            {
                _username = idColumn;
            }
        } 
    }
}