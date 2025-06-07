using InventoryService.Features.Inventories.Contracts;

namespace InventoryService.Features.Inventories.Commands;

/// <summary>
/// Represents a command to create a new inventory item.
/// </summary>
public record CreateInventoryCommand(
    Guid ProductId,
    int Quantity
)
{
    /// <summary>
    /// Converts the command to an Inventory record.
    /// </summary>
    public Inventory ToInventory() => new (ProductId, Quantity);
}
