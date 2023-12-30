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
        // 1, 2, 3, 4, 5, 6, 7 # nodes
        // 1, 1, 2, 2, 3, 3, 3, 3, # internal leaf nodes
        // 1, 1, 2, 3, 3, 3, 3, 3, # internal leaf nodes
        var stepSize = 2;
        for (var i = 0; i < sortedNodes.Count; i += stepSize)
        {
            var dictionary = new Dictionary<int, Row>();
            var maxNodeSize = 2;
            var lastIndex = sortedNodes
                .Where((x, idx) => idx < i + stepSize)
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

    public List<string> PrintBTree()
    {
        var printBTree = new List<string> { "db > Tree:" };
        printBTree.Add($"- internal (size {_internalNodes.Values.Count})");
        foreach (var leafNodes in _internalNodes.Values)
        {
            printBTree.Add($"\t- leaf (size {leafNodes.Values.Count})");
            foreach (var leafNodesValue in leafNodes.Values)
            {
                printBTree.Add($"\t\t- {leafNodesValue.Id}");
            }
        }
        return printBTree;
    }
}
