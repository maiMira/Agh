using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Agh.Domain.Base.Entities.Interfaces;
using Agh.Infrastructure.Base.Repositories.RequestModels;

namespace Agh.Infrastructure.Base.Repositories;

public interface IEntityRepository<TEntity,TId> where TEntity : IEntity<TId>
{
    Task<IQueryable<TEntity>> GetAll(CancellationToken cancellationToken = default);
    Task<TEntity> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetPaged(PagedRequest request, 
        CancellationToken cancellationToken = default);
    Task<bool> Exists(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IQueryable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken = default, bool autoSave = true);
    Task AddMany(List<TEntity> entities, CancellationToken cancellationToken = default, bool autoSave = true);
    Task Update(TEntity entity, bool autoSave = true);
    Task UpdateMany(List<TEntity> entities, bool autoSave = true);

    Task Remove(TEntity entity, bool autoSave = true);
    Task RemoveMany(List<TEntity> entities, bool autoSave = true);
    Task RemoveById(TId id, CancellationToken cancellationToken = default, bool autoSave = true);
    Task SaveChanges(CancellationToken cancellationToken = default);
}