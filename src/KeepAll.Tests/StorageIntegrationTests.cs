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

    [TestMethod]
    public async Task SqliteRepository_HandlesConcurrentInitialization()
    {
        // Arrange
        var dbPath = Path.GetTempFileName();
        var repo = new SqliteItemRepository(dbPath);

        // Act - Multiple concurrent initialization calls should be safe
        var initTasks = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            initTasks[i] = repo.InitAsync();
        }
        
        await Task.WhenAll(initTasks);

        // Verify the repository works correctly after concurrent initialization
        var item = new Item
        {
            Title = "Concurrent Test Item",
            Url = "https://example.com/concurrent",
            Type = ItemType.Link
        };

        var savedItem = await repo.UpsertAsync(item);
        var retrievedItem = await repo.GetAsync(savedItem.Id);

        // Assert
        Assert.IsNotNull(retrievedItem);
        Assert.AreEqual("Concurrent Test Item", retrievedItem.Title);
        Assert.AreEqual("https://example.com/concurrent", retrievedItem.Url);
        
        // Cleanup
        File.Delete(dbPath);
    }

    [TestMethod]
    public async Task SqliteRepository_HandlesInvalidPath()
    {
        // Arrange - Use an invalid path that should cause directory creation
        var invalidDir = Path.Combine(Path.GetTempPath(), "nonexistent", "subdir", "test.db");
        var repo = new SqliteItemRepository(invalidDir);

        // Act & Assert - Should handle directory creation gracefully
        await repo.InitAsync();
        
        var item = new Item { Title = "Test in new directory", Type = ItemType.Link };
        var savedItem = await repo.UpsertAsync(item);
        
        Assert.IsNotNull(savedItem);
        Assert.AreEqual("Test in new directory", savedItem.Title);
        
        // Cleanup
        var directory = Path.GetDirectoryName(invalidDir);
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }
}
