using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeepAll.Core.Models;
using KeepAll.Storage;
using System.IO;
using System.Threading.Tasks;

namespace KeepAll.Tests;

[TestClass]
public class StorageIntegrationTests
{
    [TestMethod]
    public async Task SqliteRepository_CanCreateAndRetrieveItems()
    {
        // Arrange
        var dbPath = Path.GetTempFileName();
        var repo = new SqliteItemRepository(dbPath);
        await repo.InitAsync();

        var item = new Item
        {
            Title = "Test Item",
            Url = "https://example.com",
            Type = ItemType.Link,
            Tags = new[] { "test", "example" }
        };

        // Act
        var savedItem = await repo.UpsertAsync(item);
        var retrievedItem = await repo.GetAsync(savedItem.Id);
        var allItems = await repo.GetAllAsync();

        // Assert
        Assert.IsNotNull(retrievedItem);
        Assert.AreEqual("Test Item", retrievedItem.Title);
        Assert.AreEqual("https://example.com", retrievedItem.Url);
        Assert.AreEqual(ItemType.Link, retrievedItem.Type);
        Assert.AreEqual(2, retrievedItem.Tags.Length);
        Assert.AreEqual("test", retrievedItem.Tags[0]);
        Assert.AreEqual("example", retrievedItem.Tags[1]);
        Assert.AreEqual(1, allItems.Count);

        // Cleanup
        File.Delete(dbPath);
    }

    [TestMethod]
    public async Task SqliteRepository_SearchWorks()
    {
        // Arrange
        var dbPath = Path.GetTempFileName();
        var repo = new SqliteItemRepository(dbPath);
        await repo.InitAsync();

        await repo.UpsertAsync(new Item { Title = "JavaScript Tutorial", Note = "Learn JS basics" });
        await repo.UpsertAsync(new Item { Title = "Python Guide", Note = "Advanced Python" });
        await repo.UpsertAsync(new Item { Title = "C# Reference", Note = "Microsoft documentation" });

        // Act
        var jsResults = await repo.SearchAsync("JavaScript");
        var pythonResults = await repo.SearchAsync("Python");
        var microsoftResults = await repo.SearchAsync("Microsoft");

        // Assert
        Assert.AreEqual(1, jsResults.Count);
        Assert.AreEqual("JavaScript Tutorial", jsResults[0].Title);
        
        Assert.AreEqual(1, pythonResults.Count);
        Assert.AreEqual("Python Guide", pythonResults[0].Title);
        
        Assert.AreEqual(1, microsoftResults.Count);
        Assert.AreEqual("C# Reference", microsoftResults[0].Title);

        // Cleanup
        File.Delete(dbPath);
    }
}
