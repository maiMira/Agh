using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Agh.Domain.Base.Entities;
using Agh.Domain.Base.Entities.Interfaces;
using Agh.Domain.Base.Exceptions;
using Agh.Infrastructure.Base.Repositories.RequestModels;
using Agh.Infrastructure.Base.Services;
using LinqKit;
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

    public async Task<IQueryable<TAggregate>> GetAll(CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable();
        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
        return query;
    }

    public async Task<TAggregate> GetById(Guid id, CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable();
        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
        return await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public async Task<PagedResult<TAggregate>> GetPaged(PagedRequest request, CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable();
        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                query = query.Where($"{filter.Key}.Contains(@0)", filter.Value);
            }
        }

        if (!string.IsNullOrEmpty(request.SearchQuery))
        {
            var stringProperties = typeof(TAggregate)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string)).ToList();

            if (stringProperties.Any())
            {
                Expression<Func<TAggregate, bool>>? combinedFilter = null;
                foreach (var property in stringProperties)
                {
                    // x => x.[Property].Contains(request.SearchQuery)
                    var param = Expression.Parameter(typeof(TAggregate), "x");
                    var propertyAccess = Expression.Property(param, property.Name);
                    var propertyAccessToLower = Expression.Call(propertyAccess, "ToLower", null);
                    var searchTerm = Expression.Constant(request.SearchQuery);
                    var searchTermToLower = Expression.Call(searchTerm, "ToLower", null);
                    var containsMethod = Expression.Call(propertyAccessToLower, "Contains", null, searchTermToLower);

                    // Lambda oluştur: x => x.[Property].Contains(request.SearchQuery)
                    var lambda = Expression.Lambda<Func<TAggregate, bool>>(containsMethod, param);

                    // Her bir string property'yi OR ile birleştir
                    combinedFilter = combinedFilter == null ? lambda : combinedFilter.Or(lambda);
                }
            }
        }
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = query.OrderBy(request.OrderBy + (request.OrderByDescending ? " descending" : ""));
        }

        var count = await query.CountAsync(cancellationToken);

        query = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return new PagedResult<TAggregate>
        {
            Queryable = query,
            RowCount = count,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

    }

    public async Task<bool> Exists(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public async Task<TAggregate> FirstOrDefault(Expression<Func<TAggregate, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable();
        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IQueryable<TAggregate>> Find(Expression<Func<TAggregate, bool>> predicate,
        CancellationToken cancellationToken = default, params Expression<Func<TAggregate, object>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable();
        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
        return query.Where(predicate);
    }

    public async Task<TAggregate> Add(TAggregate entity, CancellationToken cancellationToken = default, bool autoSave = true)
    {
        await SetCreationAudit(entity);
        await _dbSet.AddAsync(entity, cancellationToken);
        if (autoSave)
        {
            await SaveChanges(cancellationToken);
        }
        return entity;
    }

    public async Task AddMany(List<TAggregate> entities, CancellationToken cancellationToken = default, bool autoSave = true)
    {
        foreach (var entity in entities)
        {
            await SetCreationAudit(entity);
        }
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        if (autoSave)
        {
            await SaveChanges(cancellationToken);
        }
    }

    public async Task Update(TAggregate entity, bool autoSave = true)
    {
        await SetModificationAudit(entity);
        _dbSet.Update(entity);
        if (autoSave)
        {
            await SaveChanges();
        }
    }

    public async Task UpdateMany(List<TAggregate> entities, bool autoSave = true)
    {
        foreach (var entity in entities)
        {
            await SetModificationAudit(entity);
        }
        _dbSet.UpdateRange(entities);
        if (autoSave)
        {
            await SaveChanges();
        }
    }

    public async Task Remove(TAggregate entity, bool autoSave = true)
    {

        if (entity is IDeletionAudited<TUserId>)
        {
            await SetDeletionAudit(entity);
            _dbSet.Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
        if (autoSave)
        {
            await SaveChanges();
        }
    }

    public async Task RemoveMany(List<TAggregate> entities, bool autoSave = true)
    {
        foreach (var entity in entities)
        {
            if (entity is IDeletionAudited<TUserId>)
            {
                await SetDeletionAudit(entity);
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }
        if (autoSave)
        {
            await SaveChanges();
        }
    }

    public async Task RemoveById(TId id, CancellationToken cancellationToken = default, bool autoSave = true)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("The entity not found ");
        }
        if (entity is IDeletionAudited<TUserId>)
        {
            await SetDeletionAudit(entity);
            _dbSet.Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
        if (autoSave)
        {
            await SaveChanges();
        }
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}