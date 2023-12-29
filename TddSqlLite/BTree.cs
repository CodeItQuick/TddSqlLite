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
        var internalNodeList = _internalNodes.Count == 0 ? new Dictionary<int, Row>() : _internalNodes.FirstOrDefault().Value;
        foreach (var node in internalNodeList)
        {
            sortedNodes.Add(node.Key, node.Value);
        }
        if (internalNodeList.Count > 0)
        {
            _internalNodes.Remove(_internalNodes.First().Key);
        }
        for (var i = 0; i < sortedNodes.Count; i += 2)
        {
            _internalNodes.Add(sortedNodes.Keys[i], new Dictionary<int, Row>()
            {
                [sortedNodes[sortedNodes.Keys[i]].Id] = sortedNodes[sortedNodes.Keys[i]], 
            });
            if (i + 1 < sortedNodes.Count)
            {
                _internalNodes.Remove(sortedNodes.Keys[i]);
                _internalNodes.Add(sortedNodes.Keys[i + 1], new Dictionary<int, Row>
                {
                    [sortedNodes[sortedNodes.Keys[i]].Id] = sortedNodes[sortedNodes.Keys[i]], 
                    [sortedNodes[sortedNodes.Keys[i + 1]].Id] = sortedNodes[sortedNodes.Keys[i + 1]], 
                });
            }
        }
    }

    public Row GetNode(int nodeId)
    {
        var correctNode = _internalNodes
            .First(x => x.Key >= nodeId);
        return correctNode.Value[nodeId];
    }
}
