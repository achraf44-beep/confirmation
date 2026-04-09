namespace Domain.Identity;

public sealed class UserClaim
{
    public UserClaim(string type, string value)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Claim type is required.", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Claim value is required.", nameof(value));
        }

        Type = type;
        Value = value;
    }

    public string Type { get; private set; }

    public string Value { get; private set; }
}