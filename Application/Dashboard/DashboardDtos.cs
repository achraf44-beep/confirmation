namespace Application.Dashboard;

public sealed record DashboardStatsDto(
    int TotalOrders,
    int PendingOrders,
    int ConfirmedOrders,
    int ShippedOrders,
    int DeliveredOrders,
    int ReturnedOrders,
    int CancelledOrders,
    decimal TotalRevenue,
    decimal ConfirmationRate,
    decimal DeliveryRate,
    int TotalCustomers,
    int TotalProducts,
    IReadOnlyList<OrdersBySourceDto> OrdersBySource,
    IReadOnlyList<OrdersByWilayaDto> TopWilayas);

public sealed record OrdersBySourceDto(string Source, int Count);

public sealed record OrdersByWilayaDto(string Wilaya, int Count);
