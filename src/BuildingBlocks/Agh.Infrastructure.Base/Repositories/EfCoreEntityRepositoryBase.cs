using System.Linq.Expressions;
using Agh.Domain.Base.Entities.Interfaces;
using Agh.Domain.Base.Exceptions;
using Agh.Infrastructure.Base.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Agh.Infrastructure.Base.Repositories.RequestModels;
using LinqKit;

namespace Agh.Infrastructure.Base.Repositories;

public abstract class EfCoreEntityRepositoryBase<TEntity, TId, TUserId>
    : RepositoryCommon<TEntity, TUserId>
        , IEntityRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    protected EfCoreEntityRepositoryBase(IUserService<TUserId> userService, DbContext context)
        : base(userService, context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public Task<IQueryable<TEntity>> GetAll(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetQueryable());
    }

    public async Task<TEntity> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.Where(ApplySoftDeleteFilter())
            .FirstOrDefaultAsync(e => e.Id != null && e.Id.Equals(id), cancellationToken);

        if (entity == null)
            throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found");

        return entity;
    }

    public async Task<PagedResult<TEntity>> GetPaged(PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();

        if (request.Filters != null)
            foreach (var filter in request.Filters)
                query = query.Where($"{filter.Key}.Contains(@0)", filter.Value);

        if (!string.IsNullOrEmpty(request.SearchQuery))
        {
            var stringProperties = typeof(TEntity)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string)).ToList();

            if (stringProperties.Any())
            {
                Expression<Func<TEntity, bool>>? combinedFilter = null;
                foreach (var property in stringProperties)
                {
                    // x => x.[Property].Contains(request.SearchQuery)
                    var param = Expression.Parameter(typeof(TEntity), "x");
                    var propertyAccess = Expression.Property(param, property.Name);
                    var propertyAccessToLower = Expression.Call(propertyAccess, "ToLower", null);
                    var searchTerm = Expression.Constant(request.SearchQuery);
                    var searchTermToLower = Expression.Call(searchTerm, "ToLower", null);
                    var containsMethod = Expression.Call(propertyAccessToLower, "Contains", null, searchTermToLower);

                    // Lambda oluştur: x => x.[Property].Contains(request.SearchQuery)
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(containsMethod, param);

                    // Her bir string property'yi OR ile birleştir
                    combinedFilter = combinedFilter == null ? lambda : combinedFilter.Or(lambda);
                }
            }
        }
        
        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, request.OrderBy); 
            
            var lambda = Expression.Lambda(property, parameter);

            var methodName = request.OrderByDescending ? "OrderByDescending" : "OrderBy";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { query.ElementType, property.Type },
                query.Expression,
                Expression.Quote(lambda));

            query = query.Provider.CreateQuery<TEntity>(resultExpression);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return new PagedResult<TEntity>
        {
            Queryable = items,
            RowCount = totalCount,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            PageCount = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }

    public async Task<bool> Exists(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id != null && e.Id.Equals(id), cancellationToken);
    }

    public async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.Where(ApplySoftDeleteFilter())
            .FirstOrDefaultAsync(predicate, cancellationToken);

        return entity ?? throw new NotFoundException($"{typeof(TEntity).Name} not found");
    }

    public Task<IQueryable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetQueryable().Where(predicate));
    }

    public async Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken = default,
        bool autoSave = true)
    {
        await SetCreationAudit(entity);
        await _dbSet.AddAsync(entity, cancellationToken);

        return entity;
    }

    public async Task AddMany(List<TEntity> entities, CancellationToken cancellationToken = default,
        bool autoSave = true)
    {
        foreach (var entity in entities)
            await SetCreationAudit(entity);

        await _dbSet.AddRangeAsync(entities, cancellationToken);

        if (autoSave) await SaveChanges(cancellationToken);
    }

    public async Task Update(TEntity entity, bool autoSave = true)
    {
        await SetModificationAudit(entity);
        _dbSet.Update(entity);
    }

    public async Task UpdateMany(List<TEntity> entities, bool autoSave = true)
    {
        foreach (var entity in entities)
            await SetModificationAudit(entity);

        _dbSet.UpdateRange(entities);
        if (autoSave) await SaveChanges();
    }

    public async Task Remove(TEntity entity, bool autoSave = true)
    {
        if (entity is IDeletionAudited<TUserId>)
        {
            await SetDeletionAudit(entity);
            _dbSet.Update(entity);
        }
        else
            _dbSet.Remove(entity);

        if (autoSave) await SaveChanges();
    }

    public async Task RemoveMany(List<TEntity> entities, bool autoSave = true)
    {
        foreach (var entity in entities)
        {
            if (entity is IDeletionAudited<TUserId>)
            {
                await SetDeletionAudit(entity);
                _dbSet.Update(entity);
            }
            else
                _dbSet.Remove(entity);
        }

        if (autoSave) await SaveChanges();
    }

    public async Task RemoveById(TId id, CancellationToken cancellationToken = default,
        bool autoSave = true)
    {
        var entity = await FirstOrDefault(e => e.Id != null && e.Id.Equals(id), cancellationToken);

        await Remove(entity, autoSave);
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}