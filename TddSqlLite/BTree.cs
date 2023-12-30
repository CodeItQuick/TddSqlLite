using System.Collections;
using System.Collections.Specialized;
using TddSqlLite;

namespace Tests;

public class BTree
{
    private SortedList<int, Dictionary<int, Row>> _internalNodes= new();
    public void AddNode(Row row)
    {
        var sortedNodes = new SortedList<int, Row>()
        {
            [row.Id] = row
        };
        if (_internalNodes.Any())
        {
            foreach (var rowNode in _internalNodes.ToList())
            {
                foreach (var rowed in rowNode.Value)
                {
                    sortedNodes.Add(rowed.Key, rowed.Value);
                }
                
            }
        }

        RepopulateNodes(sortedNodes);
    }

    public void RepopulateNodes(SortedList<int, Row> sortedNodes)
    {
        _internalNodes = new SortedList<int, Dictionary<int, Row>>();
        for (var i = 0; i < sortedNodes.Count; i += 14)
        {
            var dictionary = new Dictionary<int, Row>();
            var lastIndex = sortedNodes
                .Where((x, idx) => idx < i + 14)
                .Select((x, idx) => x)
                .Count();
            var range = Enumerable.Range(i, lastIndex - i).ToList();
            range.ForEach(
                (curr) =>
                {
                    var index = curr;
                    var sortedNodesKey = sortedNodes.Keys[index];
                    var node = sortedNodes[sortedNodesKey];
                    var nodeId = node.Id;
                    dictionary.Add(nodeId, node);
                });
            _internalNodes.Add(dictionary.Keys.Last(), dictionary);
        }
    }

    public Row? GetNode(int nodeId)
    {
        if (_internalNodes.Count == 0)
        {
            return null;
        }

        Dictionary<int, Row>? correctNode = _internalNodes
            .FirstOrDefault(x => x.Key >= nodeId)
            .Value;
        return correctNode.FirstOrDefault(x => x.Key == nodeId).Value;
    }

    public List<Row> GetAllNodes()
    {
        return _internalNodes
            .SelectMany(x => x.Value.Values)
            .ToList();
    }
    public SortedList<int, Dictionary<int, Row>> GetNodes()
    {
        return _internalNodes;
    }
}
