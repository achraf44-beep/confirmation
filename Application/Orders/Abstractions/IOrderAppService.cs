using Domain.Orders;

namespace Application.Orders.Abstractions;

public interface IOrderAppService
{
    Task<IReadOnlyList<OrderListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<OrderDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
}