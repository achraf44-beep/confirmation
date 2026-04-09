namespace Application.Customers;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string PhoneNumber,
    string? Email,
    string Wilaya,
    string Commune,
    string Address,
    int TotalOrders,
    decimal TotalSpent,
    DateTime CreatedAtUtc);

public sealed record CreateCustomerRequest(
    string Name,
    string PhoneNumber,
    string Wilaya,
    string Commune,
    string Address,
    string? Email = null);

public sealed record UpdateCustomerRequest(
    string Name,
    string PhoneNumber,
    string Wilaya,
    string Commune,
    string Address,
    string? Email = null);
