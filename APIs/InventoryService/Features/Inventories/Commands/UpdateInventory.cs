namespace InventoryService.Features.Inventories.Commands;

/// <summary>
/// Represents a command to update an existing inventory item's quantity.
/// </summary>
public record UpdateInventoryCommand(
    Guid ProductId,
    int Quantity
);
