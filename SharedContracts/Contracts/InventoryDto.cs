namespace SharedContracts.Contracts;

/// <summary>
/// Represents an inventory data transfer object.
/// </summary>
public record InventoryDto(
    Guid ProductId,
    int Quantity
);
