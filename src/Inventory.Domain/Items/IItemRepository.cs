
using Joseco.DDD.Core.Abstractions;

namespace Inventory.Domain.Items;

public interface IItemRepository : IRepository<Item>
{
    Task UpdateAsync(Item item);
    Task DeleteAsync(Guid id);
}
