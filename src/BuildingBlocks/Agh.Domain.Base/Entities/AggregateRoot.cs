using Agh.Domain.Base.Entities.Interfaces;

namespace Agh.Domain.Base.Entities;

public class AggregateRoot<TId> : Entity<TId>,
    IAggregateRoot<TId>
{
}