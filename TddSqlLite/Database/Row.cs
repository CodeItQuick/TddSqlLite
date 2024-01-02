using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml;

namespace TddSqlLite.Database;

public class Row 
{

    [JsonExtensionData()] 
    public Dictionary<string, object>? DynamicColumns { get; set; } = new();
    public int Id
    {
        get
        {
            object idColumn = 0;
            var idExists = DynamicColumns?.TryGetValue("Id", out idColumn);
            if (idExists is true)
            {
                return idColumn is int column ? column : 0;
            }
            return default;
        }
        set
        {
            
            object idColumn = 0;
            var idExists = DynamicColumns?.TryGetValue("Id", out idColumn);
            if (idExists is false)
            {
                DynamicColumns?.TryAdd("Id", value);
            } 
        }
    }

    public string? username 
    {
        get
        {
            object idColumn = new string("");
            var idExists = DynamicColumns?.TryGetValue("username", out idColumn);
            if (idExists is true)
            {
                return idColumn as string ?? string.Empty;
            }
            return default;
        }
        set
        {
            if (value is { Length: > 255 })
            {
                throw new Exception("Varchar too long");
            }

            object idColumn = "";
            var idExists = DynamicColumns?.TryGetValue("username", out idColumn);
            if (idExists is false)
            {
                DynamicColumns?.TryAdd("username", value ?? string.Empty);
            }
        }
    }

    public string? email 
    { 
        get
        {
            object idColumn = new string("");
            var idExists = DynamicColumns?.TryGetValue("email", out idColumn);
            if (idExists is true)
            {
                return idColumn as string ?? string.Empty;
            }
            return default;
        }
        set
        {
            if (value is { Length: > 255 })
            {
                throw new Exception("Varchar too long");
            }
            if (value is { Length: > 255 })
            {
                throw new Exception("Varchar too long");
            }

            object idColumn = "";
            var idExists = DynamicColumns?.TryGetValue("email", out idColumn);
            if (idExists is false)
            {
                DynamicColumns?.TryAdd("email", value ?? string.Empty);
            }
        } 
    }
}

public class NewAgeRow
{
    [JsonExtensionData()] 
    public Dictionary<string, object>? DynamicColumns { get; set; } = new();
}