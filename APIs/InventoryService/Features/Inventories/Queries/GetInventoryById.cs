using InventoryService.Features.Inventories.Contracts;

namespace InventoryService.Features.Inventories.Queries;

/// <summary>
/// Represents a query to get an inventory item by its product ID.
/// </summary>
public record GetInventoryByIdQuery(
    Guid ProductId
);

/// <summary>
/// Represents the result of a query to get an inventory item.
/// </summary>
public record GetInventoryQueryResult(
    Inventory? Inventory
);
