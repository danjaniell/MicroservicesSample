using Microsoft.Extensions.Caching.Memory;
using InventoryService.Features.Inventories.Contracts;
using System.Collections.Concurrent;

namespace InventoryService.Features.Inventories.Data;

/// <summary>
/// Implements IInventoryRepository using IMemoryCache as an in-memory data store.
/// </summary>
public sealed class InventoryRepository(IMemoryCache cache, ILogger<InventoryRepository> logger) : IInventoryRepository
{
    private const string CacheKey = "InventoryItems";

    /// <summary>
    /// Retrieves an inventory item by its product ID.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <returns>The inventory item if found, otherwise null.</returns>
    public Task<Inventory?> GetByIdAsync(Guid productId)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Inventory>? inventoryItems))
        {
            inventoryItems.TryGetValue(productId, out var inventory);
            logger.LogInformation("Retrieved inventory for product {ProductId}: {Found}", productId, inventory != null);
            return Task.FromResult(inventory);
        }
        logger.LogWarning("Cache key {CacheKey} not found for inventory items.", CacheKey);
        return Task.FromResult<Inventory?>(null);
    }

    /// <summary>
    /// Retrieves all inventory items.
    /// </summary>
    /// <returns>A list of all inventory items.</returns>
    public Task<IReadOnlyList<Inventory>> GetAllAsync()
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Inventory>? inventoryItems))
        {
            logger.LogInformation("Retrieved all {Count} inventory items from cache.", inventoryItems.Count);
            return Task.FromResult<IReadOnlyList<Inventory>>(inventoryItems.Values.ToList());
        }
        logger.LogWarning("Cache key {CacheKey} not found for inventory items. Returning empty list.", CacheKey);
        return Task.FromResult<IReadOnlyList<Inventory>>(new List<Inventory>());
    }

    /// <summary>
    /// Adds a new inventory item.
    /// If an item with the same ProductId already exists, it will be updated instead of added.
    /// </summary>
    /// <param name="inventory">The inventory item to add.</param>
    public Task AddAsync(Inventory inventory)
    {
        var inventoryItems = cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return new ConcurrentDictionary<Guid, Inventory>();
        });

        inventoryItems.AddOrUpdate(inventory.ProductId, inventory, (key, existingVal) => inventory); // Add or Update
        cache.Set(CacheKey, inventoryItems); // Re-set to ensure cache update, if cache implementation requires it.

        logger.LogInformation("Added/Updated inventory for product {ProductId}.", inventory.ProductId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing inventory item.
    /// </summary>
    /// <param name="inventory">The inventory item to update.</param>
    public Task UpdateAsync(Inventory inventory)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Inventory>? inventoryItems))
        {
            if (inventoryItems.TryUpdate(inventory.ProductId, inventory, inventoryItems[inventory.ProductId]))
            {
                cache.Set(CacheKey, inventoryItems); // Re-set to ensure cache update
                logger.LogInformation("Updated inventory for product {ProductId}.", inventory.ProductId);
            }
            else
            {
                logger.LogWarning("Failed to update inventory for product {ProductId}. Item not found or update conflict.", inventory.ProductId);
            }
        }
        else
        {
            logger.LogWarning("Cache key {CacheKey} not found. Cannot update inventory.", CacheKey);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes an inventory item by its product ID.
    /// </summary>
    /// <param name="productId">The ID of the product to delete its inventory.</param>
    public Task DeleteAsync(Guid productId)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Inventory>? inventoryItems))
        {
            if (inventoryItems.TryRemove(productId, out _))
            {
                cache.Set(CacheKey, inventoryItems); // Re-set to ensure cache update
                logger.LogInformation("Deleted inventory for product {ProductId}.", productId);
            }
            else
            {
                logger.LogWarning("Failed to delete inventory for product {ProductId}. Item not found.", productId);
            }
        }
        else
        {
            logger.LogWarning("Cache key {CacheKey} not found. Cannot delete inventory.", CacheKey);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if an inventory item with the given product ID exists.
    /// </summary>
    /// <param name="productId">The product ID to check.</param>
    /// <returns>True if the inventory exists, false otherwise.</returns>
    public Task<bool> ExistsAsync(Guid productId)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Inventory>? inventoryItems))
        {
            return Task.FromResult(inventoryItems.ContainsKey(productId));
        }
        return Task.FromResult(false);
    }
}
