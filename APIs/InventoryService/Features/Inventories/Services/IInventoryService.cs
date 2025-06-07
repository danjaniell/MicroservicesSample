using InventoryService.Features.Inventories.Commands;
using InventoryService.Features.Inventories.Contracts;
using InventoryService.Features.Inventories.Queries;
using SharedContracts.Contracts;

namespace InventoryService.Features.Inventories.Services;

/// <summary>
/// Defines the contract for the inventory business logic service.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Retrieves an inventory item by its product ID.
    /// </summary>
    /// <param name="query">The query containing the product ID.</param>
    /// <returns>A DTO representing the inventory item, or null if not found.</returns>
    Task<InventoryDto?> GetInventoryByIdAsync(GetInventoryByIdQuery query);

    /// <summary>
    /// Retrieves all inventory items.
    /// </summary>
    /// <param name="query">The query to get all inventories.</param>
    /// <returns>A list of DTOs representing all inventory items.</returns>
    Task<IReadOnlyList<InventoryDto>> GetAllInventoriesAsync(GetAllInventoriesQuery query);

    /// <summary>
    /// Creates a new inventory item.
    /// </summary>
    /// <param name="command">The command to create the inventory.</param>
    /// <returns>A DTO representing the newly created inventory item.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an inventory for the product already exists.</exception>
    Task<InventoryDto> CreateInventoryAsync(CreateInventoryCommand command);

    /// <summary>
    /// Updates an existing inventory item's quantity.
    /// </summary>
    /// <param name="command">The command to update the inventory.</param>
    /// <returns>A DTO representing the updated inventory item.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the inventory for the product does not exist.</exception>
    Task<InventoryDto> UpdateInventoryAsync(UpdateInventoryCommand command);

    /// <summary>
    /// Deletes an inventory item by its product ID.
    /// </summary>
    /// <param name="command">The command to delete the inventory.</param>
    /// <exception cref="InvalidOperationException">Thrown if the inventory for the product does not exist.</exception>
    Task DeleteInventoryAsync(DeleteInventoryCommand command);
}
