using System;
using System.Collections.Generic;
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
        private readonly SQLiteAsyncConnection _db;

        public SqliteItemRepository(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitAsync()
        {
            await _db.CreateTableAsync<ItemDto>();
        }

        public async Task<Item> UpsertAsync(Item item, CancellationToken ct = default)
        {
            var dto = ItemDto.From(item);
            await _db.InsertOrReplaceAsync(dto);
            return dto.ToModel();
        }

        public async Task<Item?> GetAsync(string id, CancellationToken ct = default)
            => (await _db.Table<ItemDto>().Where(x => x.Id == id).FirstOrDefaultAsync())?.ToModel();

        public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken ct = default)
            => (await _db.Table<ItemDto>().OrderByDescending(x => x.CreatedAt).ToListAsync())
               .Select(d => d.ToModel()).ToList();

        public async Task<IReadOnlyList<Item>> SearchAsync(string q, CancellationToken ct = default)
        {
            q = $"%{q.ToLower()}%";
            var res = await _db.QueryAsync<ItemDto>(
               "SELECT * FROM ItemDto WHERE lower(Title) LIKE ? OR lower(Note) LIKE ?", q, q);
            return res.Select(d => d.ToModel()).ToList();
        }

        public Task DeleteAsync(string id, CancellationToken ct = default)
            => _db.Table<ItemDto>().DeleteAsync(x => x.Id == id);

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
