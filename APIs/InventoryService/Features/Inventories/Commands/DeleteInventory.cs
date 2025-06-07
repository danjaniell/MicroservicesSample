namespace InventoryService.Features.Inventories.Commands;

/// <summary>
/// Represents a command to delete an inventory item by its product ID.
/// </summary>
public record DeleteInventoryCommand(
    Guid ProductId
);
