namespace Application.Products;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string? Sku,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? Category,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record CreateProductRequest(
    string Name,
    decimal Price,
    int StockQuantity = 0,
    string? Sku = null,
    string? Description = null,
    string? Category = null);

public sealed record UpdateProductRequest(
    string Name,
    decimal Price,
    int StockQuantity,
    string? Sku = null,
    string? Description = null,
    string? Category = null);
