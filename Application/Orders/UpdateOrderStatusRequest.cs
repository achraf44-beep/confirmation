namespace Application.Orders;

public sealed record UpdateOrderStatusRequest(
    string Status,
    string? Note = null);