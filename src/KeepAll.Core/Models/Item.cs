using System;

namespace KeepAll.Core.Models
{
    public enum ItemType { Link, Book, Movie, Podcast }
    public enum ItemStatus { Later, InProgress, Done }

    public sealed class Item
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public ItemType Type { get; set; } = ItemType.Link;
        public ItemStatus Status { get; set; } = ItemStatus.Later;

        public string Title { get; set; } = "";
        public string Url { get; set; } = "";            // original share URL (if any)
        public string SourceApp { get; set; } = "";      // optional: "YouTube", "Spotify"
        public string[] Tags { get; set; } = Array.Empty<string>();
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Media details (optional)
        public string? AuthorOrCast { get; set; }
        public int? Year { get; set; }
        public int? RuntimeMinutes { get; set; }
        public string? CoverUrl { get; set; }
    }
}
