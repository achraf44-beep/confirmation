namespace Application.Orders;

public sealed record OrderListItemDto(
    Guid Id,
    string OrderNumber,
    string Source,
    string CustomerName,
    string Wilaya,
    string PhoneNumber,
    string Status,
    decimal Subtotal,
    DateTime CreatedAtUtc);