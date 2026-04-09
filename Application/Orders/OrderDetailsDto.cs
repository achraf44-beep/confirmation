namespace Application.Orders;

public sealed record OrderDetailsDto(
    Guid Id,
    string OrderNumber,
    string Source,
    string CustomerName,
    string PhoneNumber,
    string Wilaya,
    string Commune,
    string DeliveryAddress,
    string Status,
    DateTime CreatedAtUtc,
    string? Note,
    IReadOnlyList<OrderItemDto> Items,
    decimal Subtotal);