namespace Application.Orders;

public sealed record CreateOrderRequest(
    string OrderNumber,
    string Source,
    string CustomerName,
    string PhoneNumber,
    string Wilaya,
    string Commune,
    string DeliveryAddress,
    List<CreateOrderItemRequest> Items,
    string? Note = null);