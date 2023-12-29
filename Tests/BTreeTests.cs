using TddSqlLite;

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
        Assert.Equal(1, bTree.GetNode(1).Id);
        Assert.Equal(12, bTree.GetNode(12).Id);
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
    }
}