using InventoryService.Features.Inventories.Commands;
using InventoryService.Features.Inventories.Data;
using InventoryService.Features.Inventories.Queries;
using SharedContracts.Contracts;
using InventoryService.Features.Inventories.Contracts;

namespace InventoryService.Features.Inventories.Services;

/// <summary>
/// Implements the IInventoryService, containing the business logic for inventory management.
/// </summary>
public sealed class InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger) : IInventoryService
{
    /// <summary>
    /// Retrieves an inventory item by its product ID.
    /// </summary>
    /// <param name="query">The query containing the product ID.</param>
    /// <returns>A DTO representing the inventory item, or null if not found.</returns>
    public async Task<InventoryDto?> GetInventoryByIdAsync(GetInventoryByIdQuery query)
    {
        var inventory = await inventoryRepository.GetByIdAsync(query.ProductId);
        if (inventory == null)
        {
            logger.LogWarning("Inventory for product ID {ProductId} not found.", query.ProductId);
            return null;
        }
        return new InventoryDto(inventory.ProductId, inventory.Quantity);
    }

    /// <summary>
    /// Retrieves all inventory items.
    /// </summary>
    /// <param name="query">The query to get all inventories.</param>
    /// <returns>A list of DTOs representing all inventory items.</returns>
    public async Task<IReadOnlyList<InventoryDto>> GetAllInventoriesAsync(GetAllInventoriesQuery query)
    {
        var inventories = await inventoryRepository.GetAllAsync();
        return inventories.Select(i => new InventoryDto(i.ProductId, i.Quantity)).ToList();
    }

    /// <summary>
    /// Creates a new inventory item.
    /// </summary>
    /// <param name="command">The command to create the inventory.</param>
    /// <returns>A DTO representing the newly created inventory item.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an inventory for the product already exists.</exception>
    public async Task<InventoryDto> CreateInventoryAsync(CreateInventoryCommand command)
    {
        var existingInventory = await inventoryRepository.GetByIdAsync(command.ProductId);
        if (existingInventory != null)
        {
            logger.LogError("Attempted to create inventory for product ID {ProductId} that already exists.", command.ProductId);
            throw new InvalidOperationException($"Inventory for product ID {command.ProductId} already exists.");
        }

        var newInventory = new Inventory(command.ProductId, command.Quantity);
        await inventoryRepository.AddAsync(newInventory);
        logger.LogInformation("Created inventory for product ID {ProductId} with quantity {Quantity}.", newInventory.ProductId, newInventory.Quantity);
        return new InventoryDto(newInventory.ProductId, newInventory.Quantity);
    }

    /// <summary>
    /// Updates an existing inventory item's quantity.
    /// </summary>
    /// <param name="command">The command to update the inventory.</param>
    /// <returns>A DTO representing the updated inventory item.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the inventory for the product does not exist.</exception>
    public async Task<InventoryDto> UpdateInventoryAsync(UpdateInventoryCommand command)
    {
        var existingInventory = await inventoryRepository.GetByIdAsync(command.ProductId);
        if (existingInventory == null)
        {
            logger.LogError("Attempted to update inventory for product ID {ProductId} that does not exist.", command.ProductId);
            throw new InvalidOperationException($"Inventory for product ID {command.ProductId} does not exist.");
        }

        var updatedInventory = existingInventory with { Quantity = command.Quantity }; // Use with-expression for immutability
        await inventoryRepository.UpdateAsync(updatedInventory);
        logger.LogInformation("Updated inventory for product ID {ProductId} to quantity {Quantity}.", updatedInventory.ProductId, updatedInventory.Quantity);
        return new InventoryDto(updatedInventory.ProductId, updatedInventory.Quantity);
    }

    /// <summary>
    /// Deletes an inventory item by its product ID.
    /// </summary>
    /// <param name="command">The command to delete the inventory.</param>
    /// <exception cref="InvalidOperationException">Thrown if the inventory for the product does not exist.</exception>
    public async Task DeleteInventoryAsync(DeleteInventoryCommand command)
    {
        var existingInventory = await inventoryRepository.GetByIdAsync(command.ProductId);
        if (existingInventory == null)
        {
            logger.LogError("Attempted to delete inventory for product ID {ProductId} that does not exist.", command.ProductId);
            throw new InvalidOperationException($"Inventory for product ID {command.ProductId} does not exist.");
        }

        await inventoryRepository.DeleteAsync(command.ProductId);
        logger.LogInformation("Deleted inventory for product ID {ProductId}.", command.ProductId);
    }
}
