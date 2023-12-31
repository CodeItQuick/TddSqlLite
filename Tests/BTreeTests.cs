using TddSqlLite.Database;
using TddSqlLite.Database.Internals;

namespace Tests;

public class BTreeTests
{
    [Fact]
    public void CanInsertNode()
    {
        var bTree = new BTree();

        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        
        Assert.Equal(5, bTree.GetNode(5).Id);
    }
    [Fact]
    public void CanInsertTwoNodes()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });

        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        
        Assert.Equal(5, bTree.GetNode(5).Id);
        Assert.Equal(12, bTree.GetNode(12).Id);
    }
    [Fact]
    public void CanInsertThreeNodes()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });

        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        
        Assert.Equal(5, bTree.GetNode(5).Id);
        Assert.Equal(12, bTree.GetNode(12).Id);
        Assert.Equal(1, bTree.GetNode(1).Id);
    }
    [Fact]
    public void CanInsertThreeDifferentNodes()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });

        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        
        Assert.Equal(12, bTree.GetNode(12).Id);
        Assert.Equal(5, bTree.GetNode(5).Id);
        Assert.Equal(1, bTree.GetNode(1).Id);
    }
    [Fact]
    public void InsertingThreeNodesPrintsCorrectBTree()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });

        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });

        var printBTree = bTree.PrintBTree();
        Assert.Equal("db > Tree:", printBTree[0]);
        Assert.Equal("- internal (size 2)", printBTree[1]);
        Assert.Equal("\t- leaf (size 2)", printBTree[2]);
        Assert.Equal("\t\t- 1", printBTree[3]);
        Assert.Equal("\t\t- 5", printBTree[4]);
        Assert.Equal("\t- leaf (size 1)", printBTree[5]);
        Assert.Equal("\t\t- 12", printBTree[6]);
    }
    [Fact]
    public void InsertingFourNodesPrintsCorrectBTree()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 2, email = "test@user.com", username = "test-user" });

        var printBTree = bTree.PrintBTree();
        Assert.Equal("db > Tree:", printBTree[0]);
        Assert.Equal("- internal (size 2)", printBTree[1]);
        Assert.Equal("\t- leaf (size 2)", printBTree[2]);
        Assert.Equal("\t\t- 1", printBTree[3]);
        Assert.Equal("\t\t- 2", printBTree[4]);
        Assert.Equal("\t- leaf (size 2)", printBTree[5]);
        Assert.Equal("\t\t- 5", printBTree[6]);
        Assert.Equal("\t\t- 12", printBTree[7]);
    }
    [Fact]
    public void InsertingFiveNodesPrintsCorrectBTree()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 2, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 3, email = "test@user.com", username = "test-user" });

        var printBTree = bTree.PrintBTree();
        Assert.Equal("db > Tree:", printBTree[0]);
        Assert.Equal("- internal (size 2)", printBTree[1]);
        Assert.Equal("\t- leaf (size 3)", printBTree[2]);
        Assert.Equal("\t\t- 1", printBTree[3]);
        Assert.Equal("\t\t- 2", printBTree[4]);
        Assert.Equal("\t\t- 3", printBTree[5]);
        Assert.Equal("\t- leaf (size 2)", printBTree[6]);
        Assert.Equal("\t\t- 5", printBTree[7]);
        Assert.Equal("\t\t- 12", printBTree[8]);
    }
    [Fact]
    public void InsertingSixNodesPrintsCorrectBTree()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 2, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 3, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 4, email = "test@user.com", username = "test-user" });

        var printBTree = bTree.PrintBTree();
        Assert.Equal("db > Tree:", printBTree[0]);
        Assert.Equal("- internal (size 2)", printBTree[1]);
        Assert.Equal("\t- leaf (size 3)", printBTree[2]);
        Assert.Equal("\t\t- 1", printBTree[3]);
        Assert.Equal("\t\t- 2", printBTree[4]);
        Assert.Equal("\t\t- 3", printBTree[5]);
        Assert.Equal("\t- leaf (size 3)", printBTree[6]);
        Assert.Equal("\t\t- 4", printBTree[7]);
        Assert.Equal("\t\t- 5", printBTree[8]);
        Assert.Equal("\t\t- 12", printBTree[9]);
    }
    [Fact]
    public void InsertingSevenNodesPrintsCorrectBTree()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 2, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 3, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 4, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 11, email = "test@user.com", username = "test-user" });

        var printBTree = bTree.PrintBTree();
        Assert.Equal("db > Tree:", printBTree[0]);
        Assert.Equal("- internal (size 3)", printBTree[1]);
        Assert.Equal("\t- leaf (size 3)", printBTree[2]);
        Assert.Equal("\t\t- 1", printBTree[3]);
        Assert.Equal("\t\t- 2", printBTree[4]);
        Assert.Equal("\t\t- 3", printBTree[5]);
        Assert.Equal("\t- leaf (size 3)", printBTree[6]);
        Assert.Equal("\t\t- 4", printBTree[7]);
        Assert.Equal("\t\t- 5", printBTree[8]);
        Assert.Equal("\t\t- 11", printBTree[9]);
        Assert.Equal("\t- leaf (size 1)", printBTree[10]);
        Assert.Equal("\t\t- 12", printBTree[11]);
    }
    [Fact]
    public void InsertingElevenNodesPrintsCorrectBTree()
    {
        var bTree = new BTree();
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 2, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 3, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 4, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 8, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 13, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 17, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 21, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 23, email = "test@user.com", username = "test-user" });

        var printBTree = bTree.PrintBTree();
        Assert.Equal("db > Tree:", printBTree[0]);
        Assert.Equal("- internal (size 3)", printBTree[1]);
        Assert.Equal("\t- leaf (size 4)", printBTree[2]);
        Assert.Equal("\t\t- 1", printBTree[3]);
        Assert.Equal("\t\t- 2", printBTree[4]);
        Assert.Equal("\t\t- 3", printBTree[5]);
        Assert.Equal("\t\t- 4", printBTree[6]);
        Assert.Equal("\t- leaf (size 4)", printBTree[7]);
        Assert.Equal("\t\t- 5", printBTree[8]);
        Assert.Equal("\t\t- 8", printBTree[9]);
        Assert.Equal("\t\t- 12", printBTree[10]);
        Assert.Equal("\t\t- 13", printBTree[11]);
        Assert.Equal("\t- leaf (size 3)", printBTree[12]);
        Assert.Equal("\t\t- 17", printBTree[13]);
        Assert.Equal("\t\t- 21", printBTree[14]);
        Assert.Equal("\t\t- 23", printBTree[15]);
    }
    [Fact]
    public void CanInsertSevenDifferentNodes()
    {
        var bTree = new BTree();
        
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 11, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 8, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 3, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 14, email = "test@user.com", username = "test-user" });

        
        Assert.Equal(12, bTree.GetNode(12).Id);
        Assert.Equal(5, bTree.GetNode(5).Id);
        Assert.Equal(1, bTree.GetNode(1).Id);
        Assert.Equal(11, bTree.GetNode(11).Id);
        Assert.Equal(8, bTree.GetNode(8).Id);
        Assert.Equal(3, bTree.GetNode(3).Id);
        Assert.Equal(14, bTree.GetNode(14).Id);
    }[Fact]
    public void CanInsertSevenDifferentNodesGetNodeIdx()
    {
        var bTree = new BTree();
        
        bTree.AddNode(new Row() { Id = 12, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 5, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 1, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 11, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 8, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 3, email = "test@user.com", username = "test-user" });
        bTree.AddNode(new Row() { Id = 14, email = "test@user.com", username = "test-user" });

        
        Assert.Equal(1, bTree.GetNodeIdx(0).Id);
        Assert.Equal(3, bTree.GetNodeIdx(1).Id);
        Assert.Equal(5, bTree.GetNodeIdx(2).Id);
        Assert.Equal(8, bTree.GetNodeIdx(3).Id);
        Assert.Equal(11, bTree.GetNodeIdx(4).Id);
        Assert.Equal(12, bTree.GetNodeIdx(5).Id);
        Assert.Equal(14, bTree.GetNodeIdx(6).Id);
    }
}