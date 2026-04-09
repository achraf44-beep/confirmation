using Domain.Common;

namespace Domain.Delivery;

public sealed class DeliveryCompany : AggregateRoot
{
    private DeliveryCompany(
        Guid id,
        string name,
        string code,
        string? apiEndpoint,
        string? apiKey,
        bool isActive,
        DateTime createdAtUtc)
        : base(id)
    {
        Name = name;
        Code = code;
        ApiEndpoint = apiEndpoint;
        ApiKey = apiKey;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }

    public string Name { get; private set; }
    public string Code { get; private set; }
    public string? ApiEndpoint { get; private set; }
    public string? ApiKey { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; }

    public static DeliveryCompany Create(string name, string code, string? apiEndpoint = null, string? apiKey = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        return new DeliveryCompany(
            Guid.NewGuid(),
            name,
            code.ToUpperInvariant(),
            apiEndpoint,
            apiKey,
            true,
            DateTime.UtcNow);
    }

    public void Update(string name, string? apiEndpoint, string? apiKey)
    {
        Name = name;
        ApiEndpoint = apiEndpoint;
        ApiKey = apiKey;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}

public static class DeliveryCompanyCodes
{
    public const string Yalidine = "YALIDINE";
    public const string ZRExpress = "ZREXPRESS";
    public const string MaystroDelivery = "MAYSTRO";
    public const string EcoTrack = "ECOTRACK";
}
