namespace Domain.Common;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(id));
        }

        Id = id;
    }

    public Guid Id { get; }

    public override bool Equals(object? obj)
    {
        return obj is Entity other && other.GetType() == GetType() && other.Id == Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}