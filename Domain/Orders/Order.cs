using Domain.Common;

namespace Domain.Orders;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> items = [];

    private Order(
        Guid id,
        string orderNumber,
        OrderSource source,
        string customerName,
        string phoneNumber,
        string wilaya,
        string commune,
        string deliveryAddress,
        OrderStatus status,
        DateTime createdAtUtc,
        string? note = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            throw new ArgumentException("Order number is required.", nameof(orderNumber));
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
        }

        if (string.IsNullOrWhiteSpace(wilaya))
        {
            throw new ArgumentException("Wilaya is required.", nameof(wilaya));
        }

        if (string.IsNullOrWhiteSpace(commune))
        {
            throw new ArgumentException("Commune is required.", nameof(commune));
        }

        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            throw new ArgumentException("Delivery address is required.", nameof(deliveryAddress));
        }

        OrderNumber = orderNumber;
        Source = source;
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        Wilaya = wilaya;
        Commune = commune;
        DeliveryAddress = deliveryAddress;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        Note = note;
    }

    public string OrderNumber { get; }

    public OrderSource Source { get; }

    public string CustomerName { get; }

    public string PhoneNumber { get; }

    public string Wilaya { get; }

    public string Commune { get; }

    public string DeliveryAddress { get; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public string? Note { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => items.AsReadOnly();

    public decimal Subtotal => items.Sum(item => item.LineTotal);

    public static Order Create(
        string orderNumber,
        OrderSource source,
        string customerName,
        string phoneNumber,
        string wilaya,
        string commune,
        string deliveryAddress,
        IEnumerable<OrderItem> orderItems,
        string? note = null,
        DateTime? createdAtUtc = null)
    {
        var order = new Order(
            Guid.NewGuid(),
            orderNumber,
            source,
            customerName,
            phoneNumber,
            wilaya,
            commune,
            deliveryAddress,
            OrderStatus.Pending,
            createdAtUtc ?? DateTime.UtcNow,
            note);

        foreach (var item in orderItems)
        {
            order.AddItem(item);
        }

        return order;
    }

    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        items.Add(item);
    }

    public void UpdateStatus(OrderStatus status, string? note = null)
    {
        Status = status;

        if (!string.IsNullOrWhiteSpace(note))
        {
            Note = note;
        }
    }
}