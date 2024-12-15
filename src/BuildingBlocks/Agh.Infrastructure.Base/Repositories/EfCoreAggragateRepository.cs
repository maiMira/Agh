using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Agh.Domain.Base.Entities;
using Agh.Domain.Base.Entities.Interfaces;
using Agh.Infrastructure.Base.Repositories.RequestModels;
using Agh.Infrastructure.Base.Services;
using Microsoft.EntityFrameworkCore;

namespace Agh.Infrastructure.Base.Repositories;

public abstract class EfCoreAggragateRepository<TAggregate, TUserId, TId> : RepositoryCommon<TAggregate, TUserId>,
    IAggregateRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>, IEntity<TId>
{
    private readonly DbContext _context;
    private readonly DbSet<TAggregate> _dbSet;

    public EfCoreAggragateRepository(IUserService<TUserId> currentUser, DbContext context)
        : base(currentUser, context)
    {
        _context = context;
        _dbSet = context.Set<TAggregate>();
    }

    public Task<IQueryable<TAggregate>> GetAll(CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> GetById(Guid id, CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<TAggregate>> GetPaged(PagedRequest request, CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> FirstOrDefault(Expression<Func<TAggregate, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<TAggregate>> Find(Expression<Func<TAggregate, bool>> predicate,
        CancellationToken cancellationToken = default, params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> Add(TAggregate entity, CancellationToken cancellationToken = default, bool autoSave = true)
    {
        throw new NotImplementedException();
    }

    public Task AddMany(List<TAggregate> entities, CancellationToken cancellationToken = default, bool autoSave = true)
    {
        throw new NotImplementedException();
    }

    public Task Update(TAggregate entity, bool autoSave = true)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMany(List<TAggregate> entities, bool autoSave = true)
    {
        throw new NotImplementedException();
    }

    public Task Remove(TAggregate entity, bool autoSave = true)
    {
        throw new NotImplementedException();
    }

    public Task RemoveMany(List<TAggregate> entities, bool autoSave = true)
    {
        throw new NotImplementedException();
    }

    public Task RemoveById(TId id, CancellationToken cancellationToken = default, bool autoSave = true)
    {
        throw new NotImplementedException();
    }

    public Task SaveChanges(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}