using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeepAll.Core.Models;

namespace KeepAll.Tests;

[TestClass]
public class CoreTests
{
    [TestMethod]
    public void Item_Defaults()
    {
        var it = new Item();
        Assert.AreEqual(ItemType.Link, it.Type);
        Assert.AreEqual(ItemStatus.Later, it.Status);
        Assert.IsFalse(string.IsNullOrWhiteSpace(it.Id));
    }
}
