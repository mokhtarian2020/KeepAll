using KeepAll.Core.Abstractions;
using KeepAll.Core.Models;
using KeepAll.Storage;
using KeepAll.Metadata;
using System.Text.Json;

namespace KeepAll.Demo;

class Program
{
    private static IItemRepository? _repo;
    private static IMetadataService? _metadata;

    static async Task Main(string[] args)
    {
        Console.WriteLine("🎯 KeepAll Demo - Interactive Testing");
        Console.WriteLine("=====================================");
        
        // Initialize services
        await InitializeAsync();
        
        // Show menu
        while (true)
        {
            ShowMenu();
            var choice = Console.ReadLine();
            
            try
            {
                switch (choice?.ToLower())
                {
                    case "1": await AddItemAsync(); break;
                    case "2": await ViewAllItemsAsync(); break;
                    case "3": await SearchItemsAsync(); break;
                    case "4": await FilterByTypeAsync(); break;
                    case "5": await MarkItemDoneAsync(); break;
                    case "6": await ExportDataAsync(); break;
                    case "7": await SimulateShareAsync(); break;
                    case "8": await ShowStatsAsync(); break;
                    case "0": 
                        Console.WriteLine("👋 Thanks for testing KeepAll!");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid option. Try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
    
    static async Task InitializeAsync()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), "keepall_demo.db3");
        Console.WriteLine($"📁 Database: {dbPath}");
        
        var sqliteRepo = new SqliteItemRepository(dbPath);
        await sqliteRepo.InitAsync();
        _repo = sqliteRepo;
        _metadata = new MetadataService();
        
        Console.WriteLine("✅ KeepAll services initialized!");
        Console.WriteLine();
    }
    
    static void ShowMenu()
    {
        Console.WriteLine("📱 KeepAll Demo Menu:");
        Console.WriteLine("1. ➕ Add Item");
        Console.WriteLine("2. 📋 View All Items");
        Console.WriteLine("3. 🔍 Search Items");
        Console.WriteLine("4. 🎭 Filter by Type (Books/Movies/Podcasts)");
        Console.WriteLine("5. ✅ Mark Item as Done");
        Console.WriteLine("6. 💾 Export Data (JSON)");
        Console.WriteLine("7. 📲 Simulate Android Share");
        Console.WriteLine("8. 📊 Show Statistics");
        Console.WriteLine("0. 🚪 Exit");
        Console.Write("\nChoose option: ");
    }
    
    static async Task AddItemAsync()
    {
        Console.Clear();
        Console.WriteLine("➕ Add New Item");
        Console.WriteLine("===============");
        
        Console.Write("Title: ");
        var title = Console.ReadLine() ?? "";
        
        Console.Write("URL (optional): ");
        var url = Console.ReadLine() ?? "";
        
        Console.WriteLine("\nItem Types:");
        Console.WriteLine("1. Link  2. Book  3. Movie  4. Podcast");
        Console.Write("Choose type (1-4): ");
        var typeChoice = Console.ReadLine();
        
        var type = typeChoice switch
        {
            "2" => ItemType.Book,
            "3" => ItemType.Movie,
            "4" => ItemType.Podcast,
            _ => ItemType.Link
        };
        
        Console.Write("Tags (comma-separated): ");
        var tagsInput = Console.ReadLine() ?? "";
        var tags = string.IsNullOrWhiteSpace(tagsInput) 
            ? Array.Empty<string>() 
            : tagsInput.Split(',').Select(t => t.Trim()).ToArray();
        
        Console.Write("Note (optional): ");
        var note = Console.ReadLine();
        
        // For media items, add some extra fields
        string? author = null;
        int? year = null;
        if (type != ItemType.Link)
        {
            Console.Write($"{(type == ItemType.Book ? "Author" : type == ItemType.Movie ? "Director" : "Host/Creator")} (optional): ");
            author = Console.ReadLine();
            
            Console.Write("Year (optional): ");
            if (int.TryParse(Console.ReadLine(), out var yearValue))
                year = yearValue;
        }
        
        var item = new Item
        {
            Title = title,
            Url = url,
            Type = type,
            Tags = tags,
            Note = string.IsNullOrWhiteSpace(note) ? null : note,
            AuthorOrCast = author,
            Year = year,
            SourceApp = "Demo"
        };
        
        // Enrich with metadata service (currently just passes through)
        item = await _metadata!.EnrichAsync(item);
        
        // Save to repository
        var savedItem = await _repo!.UpsertAsync(item);
        
        Console.WriteLine($"\n✅ Item saved with ID: {savedItem.Id}");
        Console.WriteLine($"📝 Title: {savedItem.Title}");
        Console.WriteLine($"🏷️  Type: {savedItem.Type}");
        if (savedItem.Tags.Length > 0)
            Console.WriteLine($"🏷️  Tags: {string.Join(", ", savedItem.Tags)}");
    }
    
    static async Task ViewAllItemsAsync()
    {
        Console.Clear();
        Console.WriteLine("📋 All Items");
        Console.WriteLine("============");
        
        var items = await _repo!.GetAllAsync();
        
        if (!items.Any())
        {
            Console.WriteLine("📭 No items found. Add some items first!");
            return;
        }
        
        foreach (var item in items)
        {
            PrintItem(item);
            Console.WriteLine();
        }
        
        Console.WriteLine($"📊 Total: {items.Count} items");
    }
    
    static async Task SearchItemsAsync()
    {
        Console.Clear();
        Console.WriteLine("🔍 Search Items");
        Console.WriteLine("===============");
        
        Console.Write("Enter search term: ");
        var query = Console.ReadLine() ?? "";
        
        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("❌ Search term cannot be empty");
            return;
        }
        
        var results = await _repo!.SearchAsync(query);
        
        Console.WriteLine($"\n🔍 Search results for '{query}':");
        Console.WriteLine("================================");
        
        if (!results.Any())
        {
            Console.WriteLine("📭 No items found matching your search.");
            return;
        }
        
        foreach (var item in results)
        {
            PrintItem(item);
            Console.WriteLine();
        }
        
        Console.WriteLine($"📊 Found: {results.Count} items");
    }
    
    static async Task FilterByTypeAsync()
    {
        Console.Clear();
        Console.WriteLine("🎭 Filter by Type");
        Console.WriteLine("=================");
        
        Console.WriteLine("1. Books  2. Movies  3. Podcasts  4. Links");
        Console.Write("Choose type to filter (1-4): ");
        var choice = Console.ReadLine();
        
        var filterType = choice switch
        {
            "1" => ItemType.Book,
            "2" => ItemType.Movie,
            "3" => ItemType.Podcast,
            _ => ItemType.Link
        };
        
        var allItems = await _repo!.GetAllAsync();
        var filtered = allItems.Where(i => i.Type == filterType).ToList();
        
        Console.WriteLine($"\n📚 {filterType} items:");
        Console.WriteLine("================");
        
        if (!filtered.Any())
        {
            Console.WriteLine($"📭 No {filterType.ToString().ToLower()} items found.");
            return;
        }
        
        foreach (var item in filtered)
        {
            PrintItem(item);
            Console.WriteLine();
        }
        
        Console.WriteLine($"📊 Found: {filtered.Count} {filterType.ToString().ToLower()} items");
    }
    
    static async Task MarkItemDoneAsync()
    {
        Console.Clear();
        Console.WriteLine("✅ Mark Item as Done");
        Console.WriteLine("===================");
        
        var items = await _repo!.GetAllAsync();
        var pendingItems = items.Where(i => i.Status != ItemStatus.Done).ToList();
        
        if (!pendingItems.Any())
        {
            Console.WriteLine("📭 No pending items found!");
            return;
        }
        
        Console.WriteLine("Pending items:");
        for (int i = 0; i < pendingItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {pendingItems[i].Title} ({pendingItems[i].Type})");
        }
        
        Console.Write($"\nChoose item to mark as done (1-{pendingItems.Count}): ");
        if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= pendingItems.Count)
        {
            var item = pendingItems[index - 1];
            item.Status = ItemStatus.Done;
            item.CompletedAt = DateTime.UtcNow;
            
            await _repo!.UpsertAsync(item);
            
            Console.WriteLine($"\n✅ '{item.Title}' marked as done!");
        }
        else
        {
            Console.WriteLine("❌ Invalid selection.");
        }
    }
    
    static async Task ExportDataAsync()
    {
        Console.Clear();
        Console.WriteLine("💾 Export Data");
        Console.WriteLine("==============");
        
        var items = await _repo!.GetAllAsync();
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        
        var exportPath = Path.Combine(Path.GetTempPath(), $"keepall_export_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        await File.WriteAllTextAsync(exportPath, json);
        
        Console.WriteLine($"💾 Data exported to: {exportPath}");
        Console.WriteLine($"📊 Exported {items.Count} items");
        
        Console.WriteLine("\n📄 Preview (first 500 characters):");
        Console.WriteLine("=====================================");
        Console.WriteLine(json.Length > 500 ? json[..500] + "..." : json);
    }
    
    static async Task SimulateShareAsync()
    {
        Console.Clear();
        Console.WriteLine("📲 Simulate Android Share");
        Console.WriteLine("=========================");
        
        Console.WriteLine("This simulates sharing content from another app (like Chrome):");
        Console.Write("Enter URL or text to share: ");
        var sharedContent = Console.ReadLine() ?? "";
        
        if (string.IsNullOrWhiteSpace(sharedContent))
        {
            Console.WriteLine("❌ Nothing to share");
            return;
        }
        
        // Simulate the share receiver logic
        var isUrl = Uri.TryCreate(sharedContent, UriKind.Absolute, out var uri);
        var item = new Item
        {
            Title = isUrl ? $"Shared: {uri.Host}" : sharedContent.Length > 50 ? sharedContent[..50] + "..." : sharedContent,
            Url = isUrl ? sharedContent : "",
            Type = ItemType.Link,
            SourceApp = "Chrome (Simulated)",
            Note = isUrl ? null : sharedContent
        };
        
        var savedItem = await _repo!.UpsertAsync(item);
        
        Console.WriteLine($"\n📲 Shared content saved to KeepAll!");
        Console.WriteLine($"📝 Title: {savedItem.Title}");
        Console.WriteLine($"🌐 URL: {savedItem.Url}");
        Console.WriteLine($"📱 Source: {savedItem.SourceApp}");
    }
    
    static async Task ShowStatsAsync()
    {
        Console.Clear();
        Console.WriteLine("📊 KeepAll Statistics");
        Console.WriteLine("====================");
        
        var items = await _repo!.GetAllAsync();
        
        if (!items.Any())
        {
            Console.WriteLine("📭 No data available");
            return;
        }
        
        Console.WriteLine($"📊 Total Items: {items.Count}");
        Console.WriteLine();
        
        // By Type
        Console.WriteLine("By Type:");
        foreach (ItemType type in Enum.GetValues<ItemType>())
        {
            var count = items.Count(i => i.Type == type);
            Console.WriteLine($"  {type}: {count}");
        }
        Console.WriteLine();
        
        // By Status
        Console.WriteLine("By Status:");
        foreach (ItemStatus status in Enum.GetValues<ItemStatus>())
        {
            var count = items.Count(i => i.Status == status);
            Console.WriteLine($"  {status}: {count}");
        }
        Console.WriteLine();
        
        // Recent activity
        var recent = items.Where(i => i.CreatedAt > DateTime.UtcNow.AddDays(-7)).Count();
        Console.WriteLine($"📅 Added this week: {recent}");
        
        var completed = items.Where(i => i.CompletedAt.HasValue && i.CompletedAt > DateTime.UtcNow.AddDays(-7)).Count();
        Console.WriteLine($"✅ Completed this week: {completed}");
        
        // Top tags
        var allTags = items.SelectMany(i => i.Tags).Where(t => !string.IsNullOrWhiteSpace(t));
        var topTags = allTags.GroupBy(t => t.ToLower()).OrderByDescending(g => g.Count()).Take(5);
        
        if (topTags.Any())
        {
            Console.WriteLine("\n🏷️  Top Tags:");
            foreach (var tag in topTags)
            {
                Console.WriteLine($"  #{tag.Key}: {tag.Count()}");
            }
        }
    }
    
    static void PrintItem(Item item)
    {
        var statusIcon = item.Status switch
        {
            ItemStatus.Later => "⏳",
            ItemStatus.InProgress => "🔄",
            ItemStatus.Done => "✅",
            _ => "❓"
        };
        
        var typeIcon = item.Type switch
        {
            ItemType.Book => "📚",
            ItemType.Movie => "🎬",
            ItemType.Podcast => "🎧",
            _ => "🔗"
        };
        
        Console.WriteLine($"{statusIcon} {typeIcon} {item.Title}");
        
        if (!string.IsNullOrWhiteSpace(item.Url))
            Console.WriteLine($"   🌐 {item.Url}");
            
        if (!string.IsNullOrWhiteSpace(item.AuthorOrCast))
            Console.WriteLine($"   👤 {item.AuthorOrCast}");
            
        if (item.Year.HasValue)
            Console.WriteLine($"   📅 {item.Year}");
            
        if (item.Tags.Length > 0)
            Console.WriteLine($"   🏷️  {string.Join(", ", item.Tags)}");
            
        if (!string.IsNullOrWhiteSpace(item.Note))
            Console.WriteLine($"   📝 {item.Note}");
            
        if (!string.IsNullOrWhiteSpace(item.SourceApp))
            Console.WriteLine($"   📱 {item.SourceApp}");
            
        Console.WriteLine($"   🕒 {item.CreatedAt:yyyy-MM-dd HH:mm}");
        
        if (item.CompletedAt.HasValue)
            Console.WriteLine($"   ✅ Completed: {item.CompletedAt:yyyy-MM-dd HH:mm}");
    }
}
