using System.Threading;
using System.Threading.Tasks;
using KeepAll.Core.Abstractions;
using KeepAll.Core.Models;

namespace KeepAll.Metadata
{
    // Stub: later enrich item with Open Library / TMDb / iTunes Search
    public sealed class MetadataService : IMetadataService
    {
        public Task<Item> EnrichAsync(Item item, CancellationToken ct = default)
            => Task.FromResult(item);
    }
}
