using Application.Products.Abstractions;
using Domain.Products;

namespace Application.Products;

public interface IProductAppService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default);
}

public sealed class ProductAppService : IProductAppService
{
    private readonly IProductRepository repository;

    public ProductAppService(IProductRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.OrderByDescending(p => p.CreatedAtUtc).Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = Product.Create(
            request.Name,
            request.Price,
            request.StockQuantity,
            request.Sku,
            request.Description,
            request.Category);

        await repository.AddAsync(product, cancellationToken);
        return product.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null) return false;

        product.Update(request.Name, request.Price, request.StockQuantity, request.Sku, request.Description, request.Category);
        await repository.UpdateAsync(product, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null) return false;

        await repository.DeleteAsync(id, cancellationToken);
        return true;
    }

    public async Task<bool> UpdateStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null) return false;

        if (quantity > 0)
            product.AddStock(quantity);
        else
            product.DeductStock(-quantity);

        await repository.UpdateAsync(product, cancellationToken);
        return true;
    }

    private static ProductDto MapToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Sku,
        product.Description,
        product.Price,
        product.StockQuantity,
        product.Category,
        product.IsActive,
        product.CreatedAtUtc);
}
