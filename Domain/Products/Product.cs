using Domain.Common;

namespace Domain.Products;

public sealed class Product : AggregateRoot
{
    private Product(
        Guid id,
        string name,
        string? sku,
        string? description,
        decimal price,
        int stockQuantity,
        string? category,
        bool isActive,
        DateTime createdAtUtc)
        : base(id)
    {
        Name = name;
        Sku = sku;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Category = category;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }

    public string Name { get; private set; }
    public string? Sku { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string? Category { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; }

    public static Product Create(
        string name,
        decimal price,
        int stockQuantity = 0,
        string? sku = null,
        string? description = null,
        string? category = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        return new Product(
            Guid.NewGuid(),
            name,
            sku,
            description,
            price,
            stockQuantity,
            category,
            true,
            DateTime.UtcNow);
    }

    public void Update(string name, decimal price, int stockQuantity, string? sku, string? description, string? category)
    {
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
        Sku = sku;
        Description = description;
        Category = category;
    }

    public void SetActive(bool isActive) => IsActive = isActive;

    public void DeductStock(int quantity)
    {
        if (quantity > StockQuantity)
            throw new InvalidOperationException("Insufficient stock.");
        StockQuantity -= quantity;
    }

    public void AddStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        StockQuantity += quantity;
    }
}
