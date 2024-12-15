using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Agh.Domain.Base.Entities;
using Agh.Infrastructure.Base.Repositories.RequestModels;

namespace Agh.Infrastructure.Base.Repositories;

public interface IAggregateRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
{
    Task<IQueryable<TAggregate>> GetAll(CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties);

    Task<TAggregate> GetById(Guid id, CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties);

    Task<PagedResult<TAggregate>> GetPaged(PagedRequest request,
        CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties);

    Task<bool> Exists(TId id, CancellationToken cancellationToken = default);

    Task<TAggregate> FirstOrDefault(Expression<Func<TAggregate, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties);

    Task<IQueryable<TAggregate>> Find(Expression<Func<TAggregate, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties);

    Task<TAggregate> Add(TAggregate entity, CancellationToken cancellationToken = default,
        bool autoSave = true);

    Task AddMany(List<TAggregate> entities, CancellationToken cancellationToken = default,
        bool autoSave = true);

    Task Update(TAggregate entity, bool autoSave = true);

    Task UpdateMany(List<TAggregate> entities, bool autoSave = true);

    Task Remove(TAggregate entity, bool autoSave = true);

    Task RemoveMany(List<TAggregate> entities, bool autoSave = true);

    Task RemoveById(TId id, CancellationToken cancellationToken = default, bool autoSave = true);

    Task SaveChanges(CancellationToken cancellationToken = default);
}