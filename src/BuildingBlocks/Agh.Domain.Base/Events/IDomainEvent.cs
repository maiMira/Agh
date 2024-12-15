namespace Agh.Domain.Base.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}