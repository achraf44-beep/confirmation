using Domain.Delivery;

namespace Application.Delivery.Abstractions;

public interface IDeliveryCompanyRepository
{
    Task<IReadOnlyList<DeliveryCompany>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DeliveryCompany?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DeliveryCompany?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(DeliveryCompany company, CancellationToken cancellationToken = default);
    Task UpdateAsync(DeliveryCompany company, CancellationToken cancellationToken = default);
}
