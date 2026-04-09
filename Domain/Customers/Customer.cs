using Domain.Common;

namespace Domain.Customers;

public sealed class Customer : AggregateRoot
{
    private Customer(
        Guid id,
        string name,
        string phoneNumber,
        string? email,
        string wilaya,
        string commune,
        string address,
        DateTime createdAtUtc)
        : base(id)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Wilaya = wilaya;
        Commune = commune;
        Address = address;
        CreatedAtUtc = createdAtUtc;
    }

    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string Wilaya { get; private set; }
    public string Commune { get; private set; }
    public string Address { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public int TotalOrders { get; private set; }
    public decimal TotalSpent { get; private set; }

    public static Customer Create(
        string name,
        string phoneNumber,
        string wilaya,
        string commune,
        string address,
        string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

        return new Customer(
            Guid.NewGuid(),
            name,
            phoneNumber,
            email,
            wilaya,
            commune,
            address,
            DateTime.UtcNow);
    }

    public void Update(string name, string phoneNumber, string wilaya, string commune, string address, string? email)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Wilaya = wilaya;
        Commune = commune;
        Address = address;
        Email = email;
    }

    public void IncrementOrderStats(decimal orderTotal)
    {
        TotalOrders++;
        TotalSpent += orderTotal;
    }
}
