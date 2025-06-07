namespace InventoryService.Features.Inventories.Contracts;

/// <summary>
/// Represents an inventory item.
/// </summary>
public record Inventory(
    Guid ProductId,
    int Quantity
);
