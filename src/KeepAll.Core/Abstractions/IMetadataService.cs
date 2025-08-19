using System.Threading;
using System.Threading.Tasks;
using KeepAll.Core.Models;

namespace KeepAll.Core.Abstractions
{
    public interface IMetadataService
    {
        Task<Item> EnrichAsync(Item item, CancellationToken ct = default);
    }
}
