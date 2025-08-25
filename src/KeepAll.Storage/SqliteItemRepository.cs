using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using KeepAll.Core.Abstractions;
using KeepAll.Core.Models;

namespace KeepAll.Storage
{
    public sealed class SqliteItemRepository : IItemRepository
    {
        private readonly string _dbPath;
        private SQLiteAsyncConnection? _db;
        private readonly SemaphoreSlim _initSemaphore = new(1, 1);
        private bool _isInitialized;

        public SqliteItemRepository(string dbPath)
        {
            _dbPath = dbPath;
        }

        public async Task InitAsync()
        {
            if (_isInitialized) return;

            await _initSemaphore.WaitAsync();
            try
            {
                if (_isInitialized) return;

                // Ensure directory exists
                var directory = Path.GetDirectoryName(_dbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Initialize SQLite with retry logic
                await InitializeDatabaseWithRetry();
                _isInitialized = true;
            }
            finally
            {
                _initSemaphore.Release();
            }
        }

        private async Task InitializeDatabaseWithRetry(int maxRetries = 3)
        {
            Exception? lastException = null;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Initialize SQLitePCL raw if needed
                    SQLitePCL.Batteries_V2.Init();
                    
                    _db = new SQLiteAsyncConnection(_dbPath);
                    await _db.CreateTableAsync<ItemDto>();
                    
                    // Test the connection
                    await _db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ItemDto");
                    return; // Success
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    
                    if (attempt < maxRetries)
                    {
                        // Wait before retry, with exponential backoff
                        await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt - 1)));
                        
                        // Close any existing connection
                        try
                        {
                            if (_db != null)
                                await _db.CloseAsync();
                        }
                        catch { }
                        _db = null;
                    }
                }
            }
            
            throw new InvalidOperationException(
                $"Failed to initialize SQLite database after {maxRetries} attempts. " +
                $"Last error: {lastException?.Message}", lastException);
        }

        private async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (!_isInitialized)
            {
                await InitAsync();
            }
            
            if (_db == null)
            {
                throw new InvalidOperationException("Database connection is not initialized");
            }
            
            return _db;
        }

        public async Task<Item> UpsertAsync(Item item, CancellationToken ct = default)
        {
            var db = await GetConnectionAsync();
            var dto = ItemDto.From(item);
            await db.InsertOrReplaceAsync(dto);
            return dto.ToModel();
        }

        public async Task<Item?> GetAsync(string id, CancellationToken ct = default)
        {
            var db = await GetConnectionAsync();
            return (await db.Table<ItemDto>().Where(x => x.Id == id).FirstOrDefaultAsync())?.ToModel();
        }

        public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken ct = default)
        {
            var db = await GetConnectionAsync();
            return (await db.Table<ItemDto>().OrderByDescending(x => x.CreatedAt).ToListAsync())
               .Select(d => d.ToModel()).ToList();
        }

        public async Task<IReadOnlyList<Item>> SearchAsync(string q, CancellationToken ct = default)
        {
            var db = await GetConnectionAsync();
            q = $"%{q.ToLower()}%";
            var res = await db.QueryAsync<ItemDto>(
               "SELECT * FROM ItemDto WHERE lower(Title) LIKE ? OR lower(Note) LIKE ?", q, q);
            return res.Select(d => d.ToModel()).ToList();
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            var db = await GetConnectionAsync();
            await db.Table<ItemDto>().DeleteAsync(x => x.Id == id);
        }

        [Table("ItemDto")]
        private class ItemDto
        {
            [PrimaryKey] public string Id { get; set; } = "";
            public int Type { get; set; }
            public int Status { get; set; }
            public string Title { get; set; } = "";
            public string Url { get; set; } = "";
            public string SourceApp { get; set; } = "";
            public string TagsCsv { get; set; } = "";
            public string? Note { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
            public string? AuthorOrCast { get; set; }
            public int? Year { get; set; }
            public int? RuntimeMinutes { get; set; }
            public string? CoverUrl { get; set; }

            public static ItemDto From(Item m) => new()
            {
                Id = m.Id, Type = (int)m.Type, Status = (int)m.Status, Title = m.Title,
                Url = m.Url, SourceApp = m.SourceApp, TagsCsv = string.Join(",", m.Tags),
                Note = m.Note, CreatedAt = m.CreatedAt, CompletedAt = m.CompletedAt,
                AuthorOrCast = m.AuthorOrCast, Year = m.Year, RuntimeMinutes = m.RuntimeMinutes, CoverUrl = m.CoverUrl
            };

            public Item ToModel() => new()
            {
                Id = Id, Type = (ItemType)Type, Status = (ItemStatus)Status, Title = Title,
                Url = Url, SourceApp = SourceApp,
                Tags = string.IsNullOrWhiteSpace(TagsCsv) ? Array.Empty<string>() : TagsCsv.Split(','),
                Note = Note, CreatedAt = CreatedAt, CompletedAt = CompletedAt,
                AuthorOrCast = AuthorOrCast, Year = Year, RuntimeMinutes = RuntimeMinutes, CoverUrl = CoverUrl
            };
        }
    }
}
