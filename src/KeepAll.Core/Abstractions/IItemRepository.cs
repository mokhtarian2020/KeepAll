using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KeepAll.Core.Models;

namespace KeepAll.Core.Abstractions
{
    public interface IItemRepository
    {
        Task<Item> UpsertAsync(Item item, CancellationToken ct = default);
        Task<Item?> GetAsync(string id, CancellationToken ct = default);
        Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Item>> SearchAsync(string query, CancellationToken ct = default);
        Task DeleteAsync(string id, CancellationToken ct = default);
    }
}
