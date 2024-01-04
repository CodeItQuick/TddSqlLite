using System.Collections.Specialized;

namespace TddSqlLite.Database.Internals;

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
                    var success = sortedNodes.TryAdd(rowed.Key, rowed.Value);
                    if (!success)
                    {
                        throw new Exception("Row already exists in table");
                    }
                }
            }
        }

        RepopulateNodes(sortedNodes);
    }

    public void RepopulateNodes(SortedList<int, Row> sortedNodes)
    {
        _internalNodes = new SortedList<int, Dictionary<int, Row>>();
        // 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 # total nodes
        // 1, 1, 2, 2, 3, 3, 3, 3, 3, 4  # internal leaf nodes
        var stepSize = sortedNodes.Count < 4 ? 2 : 
            int.Parse("" + Math.Ceiling(Math.Sqrt(sortedNodes.Count)));
        for (var i = 0; i < sortedNodes.Count; i += stepSize)
        {
            var dictionary = new Dictionary<int, Row>();
            var maxNodeSize = 2;
            var lastIndex = sortedNodes
                .Where((x, idx) => idx < i + stepSize)
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
    public SortedList<int, Dictionary<int, NewAgeRow>> GetNodes()
    {
        var nodes = new SortedList<int, Dictionary<int, NewAgeRow>>();
        foreach (var node in _internalNodes)
        {
            var dictionaries = new Dictionary<int, NewAgeRow>();
            node.Value.Values.ToList().ForEach(row =>
            {
                dictionaries.Add( 
                    (int) row.DynamicColumns["Id"], 
                        new() { DynamicColumns = row.DynamicColumns  }
                );
            });
            nodes.Add(node.Key, dictionaries);
        }
        return nodes;
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

    public Row? GetNodeIdx(int rowIdx)
    {
        if (_internalNodes.Count == 0)
        {
            return null;
        }

        var flattenedNodes = _internalNodes
            .SelectMany(x => x.Value.Values)
            .ToList();

        return flattenedNodes[rowIdx];
    }
}
