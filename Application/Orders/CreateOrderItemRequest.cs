namespace Application.Orders;

public sealed record CreateOrderItemRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice);