namespace SharedContracts.Contracts;

/// <summary>
/// Represents a product data transfer object.
/// </summary>
public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price
);
