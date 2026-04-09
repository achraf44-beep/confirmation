namespace Application.Orders;

public sealed record OrderItemDto(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);