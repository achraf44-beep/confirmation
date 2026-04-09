using Application.Customers.Abstractions;
using Domain.Customers;

namespace Application.Customers;

public interface ICustomerAppService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class CustomerAppService : ICustomerAppService
{
    private readonly ICustomerRepository repository;

    public CustomerAppService(ICustomerRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await repository.GetAllAsync(cancellationToken);
        return customers.OrderByDescending(c => c.CreatedAtUtc).Select(MapToDto).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : MapToDto(customer);
    }

    public async Task<Guid> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = Customer.Create(
            request.Name,
            request.PhoneNumber,
            request.Wilaya,
            request.Commune,
            request.Address,
            request.Email);

        await repository.AddAsync(customer, cancellationToken);
        return customer.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return false;

        customer.Update(request.Name, request.PhoneNumber, request.Wilaya, request.Commune, request.Address, request.Email);
        await repository.UpdateAsync(customer, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return false;

        await repository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private static CustomerDto MapToDto(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.PhoneNumber,
        customer.Email,
        customer.Wilaya,
        customer.Commune,
        customer.Address,
        customer.TotalOrders,
        customer.TotalSpent,
        customer.CreatedAtUtc);
}
