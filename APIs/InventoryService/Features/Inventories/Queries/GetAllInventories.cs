using InventoryService.Features.Inventories.Contracts;

namespace InventoryService.Features.Inventories.Queries;

/// <summary>
/// Represents a query to get all inventory items.
/// </summary>
public record GetAllInventoriesQuery();

/// <summary>
/// Represents the result of a query to get all inventory items.
/// </summary>
public record GetAllInventoriesQueryResult(
    IReadOnlyList<Inventory> Inventories
);
