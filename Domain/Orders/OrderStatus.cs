namespace Domain.Orders;

public enum OrderStatus
{
    Pending = 0,
    CalledNoAnswer = 1,
    Confirmed = 2,
    Reprogrammed = 3,
    Shipped = 4,
    Delivered = 5,
    Returned = 6,
    Cancelled = 7
}