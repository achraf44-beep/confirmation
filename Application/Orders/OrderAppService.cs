using Application.Orders.Abstractions;
using Domain.Orders;

namespace Application.Orders;

public sealed class OrderAppService : IOrderAppService
{
    private readonly IOrderRepository repository;

    public OrderAppService(IOrderRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IReadOnlyList<OrderListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetAllAsync(cancellationToken);

        return orders
            .OrderByDescending(order => order.CreatedAtUtc)
            .Select(MapListItem)
            .ToList();
    }

    public async Task<OrderDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);

        return order is null ? null : MapDetails(order);
    }

    public async Task<Guid> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = Order.Create(
            request.OrderNumber,
            ParseSource(request.Source),
            request.CustomerName,
            request.PhoneNumber,
            request.Wilaya,
            request.Commune,
            request.DeliveryAddress,
            request.Items.Select(item => new OrderItem(item.ProductName, item.Quantity, item.UnitPrice)),
            request.Note);

        await repository.AddAsync(order, cancellationToken);
        return order.Id;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return false;
        }

        order.UpdateStatus(ParseStatus(request.Status), request.Note);
        await repository.UpdateAsync(order, cancellationToken);
        return true;
    }

    private static OrderListItemDto MapListItem(Order order)
    {
        return new OrderListItemDto(
            order.Id,
            order.OrderNumber,
            order.Source.ToString(),
            order.CustomerName,
            order.Wilaya,
            order.PhoneNumber,
            order.Status.ToString(),
            order.Subtotal,
            order.CreatedAtUtc);
    }

    private static OrderDetailsDto MapDetails(Order order)
    {
        return new OrderDetailsDto(
            order.Id,
            order.OrderNumber,
            order.Source.ToString(),
            order.CustomerName,
            order.PhoneNumber,
            order.Wilaya,
            order.Commune,
            order.DeliveryAddress,
            order.Status.ToString(),
            order.CreatedAtUtc,
            order.Note,
            order.Items
                .Select(item => new OrderItemDto(item.ProductName, item.Quantity, item.UnitPrice, item.LineTotal))
                .ToList(),
            order.Subtotal);
    }

    private static OrderSource ParseSource(string value)
    {
        return Enum.TryParse<OrderSource>(value, true, out var source)
            ? source
            : OrderSource.Manual;
    }

    private static OrderStatus ParseStatus(string value)
    {
        return Enum.TryParse<OrderStatus>(value, true, out var status)
            ? status
            : OrderStatus.Pending;
    }
}