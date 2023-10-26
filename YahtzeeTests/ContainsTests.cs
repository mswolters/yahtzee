using System.Collections.Immutable;

namespace Yahtzee.Tests;

[TestClass]
public class ContainsTests
{

    [TestMethod]
    public void ContainsAllWithOneElement()
    {
        var first = new List<int> { 1, 2, 3 };
        var second = new List<int> { 2 };
        Assert.IsTrue(first.ContainsAll(second));
    }
    
   
    [TestMethod]
    public void ContainsAllWithTwoElements()
    {
        var first = new List<int> { 1, 2, 3 };
        var second = new List<int> { 1, 2 };
        Assert.IsTrue(first.ContainsAll(second));
    }
    
    [TestMethod]
    public void ContainsAllWithAllElements()
    {
        var first = new List<int> { 1, 2, 3 };
        var second = new List<int> { 1, 2, 3 };
        Assert.IsTrue(first.ContainsAll(second));
    }
    
    [TestMethod]
    public void ContainsAllWithDupesInSecond()
    {
        var first = new List<int> { 1, 2, 3 };
        var second = new List<int> { 2, 2 };
        Assert.IsFalse(first.ContainsAll(second));
    }
    
    [TestMethod]
    public void ContainsAllWithDupesInFirstAndSecond()
    {
        var first = new List<int> { 1, 2, 2, 3 };
        var second = new List<int> { 2, 2 };
        Assert.IsTrue(first.ContainsAll(second));
    }
    
}