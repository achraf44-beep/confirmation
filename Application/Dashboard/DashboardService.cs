using Application.Customers.Abstractions;
using Application.Orders.Abstractions;
using Application.Products.Abstractions;
using Domain.Orders;

namespace Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}

public sealed class DashboardService : IDashboardService
{
    private readonly IOrderRepository orderRepository;
    private readonly ICustomerRepository customerRepository;
    private readonly IProductRepository productRepository;

    public DashboardService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        this.orderRepository = orderRepository;
        this.customerRepository = customerRepository;
        this.productRepository = productRepository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.GetAllAsync(cancellationToken);
        var customers = await customerRepository.GetAllAsync(cancellationToken);
        var products = await productRepository.GetAllAsync(cancellationToken);

        var totalOrders = orders.Count;
        var pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);
        var confirmedOrders = orders.Count(o => o.Status == OrderStatus.Confirmed);
        var shippedOrders = orders.Count(o => o.Status == OrderStatus.Shipped);
        var deliveredOrders = orders.Count(o => o.Status == OrderStatus.Delivered);
        var returnedOrders = orders.Count(o => o.Status == OrderStatus.Returned);
        var cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);

        var totalRevenue = orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Sum(o => o.Subtotal);

        var confirmationRate = totalOrders > 0 
            ? (decimal)(confirmedOrders + shippedOrders + deliveredOrders) / totalOrders * 100 
            : 0;

        var deliveryRate = (confirmedOrders + shippedOrders + deliveredOrders + returnedOrders) > 0
            ? (decimal)deliveredOrders / (confirmedOrders + shippedOrders + deliveredOrders + returnedOrders) * 100
            : 0;

        var ordersBySource = orders
            .GroupBy(o => o.Source)
            .Select(g => new OrdersBySourceDto(g.Key.ToString(), g.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();

        var topWilayas = orders
            .GroupBy(o => o.Wilaya)
            .Select(g => new OrdersByWilayaDto(g.Key, g.Count()))
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();

        return new DashboardStatsDto(
            totalOrders,
            pendingOrders,
            confirmedOrders,
            shippedOrders,
            deliveredOrders,
            returnedOrders,
            cancelledOrders,
            totalRevenue,
            Math.Round(confirmationRate, 1),
            Math.Round(deliveryRate, 1),
            customers.Count,
            products.Count,
            ordersBySource,
            topWilayas);
    }
}
