using InventoryService.Features.Inventories.Contracts;

namespace InventoryService.Features.Inventories.Data;

/// <summary>
/// Defines the contract for an inventory data repository.
/// </summary>
public interface IInventoryRepository
{
    /// <summary>
    /// Retrieves an inventory item by its product ID.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <returns>The inventory item if found, otherwise null.</returns>
    Task<Inventory?> GetByIdAsync(Guid productId);

    /// <summary>
    /// Retrieves all inventory items.
    /// </summary>
    /// <returns>A list of all inventory items.</returns>
    Task<IReadOnlyList<Inventory>> GetAllAsync();

    /// <summary>
    /// Adds a new inventory item.
    /// </summary>
    /// <param name="inventory">The inventory item to add.</param>
    Task AddAsync(Inventory inventory);

    /// <summary>
    /// Updates an existing inventory item.
    /// </summary>
    /// <param name="inventory">The inventory item to update.</param>
    Task UpdateAsync(Inventory inventory);

    /// <summary>
    /// Deletes an inventory item by its product ID.
    /// </summary>
    /// <param name="productId">The ID of the product to delete its inventory.</param>
    Task DeleteAsync(Guid productId);

    /// <summary>
    /// Checks if an inventory item with the given product ID exists.
    /// </summary>
    /// <param name="productId">The product ID to check.</param>
    /// <returns>True if the inventory exists, false otherwise.</returns>
    Task<bool> ExistsAsync(Guid productId);
}
