using Agh.Domain.Base.Entities.Interfaces;
using Agh.Domain.Base.Events;

namespace Agh.Domain.Base.Entities;

public class Entity<TId> : IEntity<TId>
{    
    protected Entity(TId id) { Id = id; }
    public Entity() { }
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    public TId Id { get; set; }

}