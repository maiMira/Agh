namespace Agh.Domain.Base.Entities.Interfaces;
public interface IEntity<TId>
{
    TId Id { get; protected set; }
}
