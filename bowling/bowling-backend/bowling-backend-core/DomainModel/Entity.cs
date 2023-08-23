namespace bowling_backend_core.DomainModel;

/// <summary>
/// Base class for entities.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Entity()
    { }

    public Entity(Guid id)
    { 
        Id = id;
    }
}