using System.Collections;
using System.Linq.Expressions;
using Agh.Domain.Base.Entities.Interfaces;
using Agh.Infrastructure.Base.Services;
using Microsoft.EntityFrameworkCore;

namespace Agh.Infrastructure.Base.Repositories;

public abstract class RepositoryCommon<TEntity, TUserId>
    (IUserService<TUserId> currentUser, DbContext context)
    where TEntity : class
{
    protected Expression<Func<TEntity, bool>> ApplySoftDeleteFilter()
    {
        if (typeof(IDeletionAudited<TUserId>).IsAssignableFrom(typeof(TEntity)))
        {
            return e => ((IDeletionAudited<TUserId>)e).IsDeleted == false;
        }

        return e => true;
    }
    internal virtual async Task SetCreationAudit(object entity)
    {
        if (entity is ICreationAudited<TUserId> createdAudited)
        {
            createdAudited.CreationTime = DateTime.UtcNow;
            createdAudited.CreatorUserId = await currentUser.GetUserId();
        }

        // Alt entityler için audit bilgilerini ayarla
        var navigations = context.Entry(entity)
            .Metadata.GetNavigations()
            .Where(n => !n.IsCollection);

        foreach (var navigation in navigations)
        {
            var propertyEntry = context.Entry(entity).Reference(navigation.Name);
            if (propertyEntry.CurrentValue != null)
            {
                await SetCreationAudit(propertyEntry.CurrentValue);
            }
        }

        var collections = context.Entry(entity)
            .Metadata.GetNavigations()
            .Where(n => n.IsCollection);

        foreach (var collection in collections)
        {
            var collectionEntry = context.Entry(entity).Collection(collection.Name);
            if (collectionEntry.CurrentValue == null) continue;
            
            foreach (var item in (IEnumerable)collectionEntry.CurrentValue)
            {
                await SetCreationAudit(item);
            }
        }
    }

    internal virtual async Task SetModificationAudit(object entity)
    {
        if (entity is IModificationAudited<TUserId> modifiedAudited)
        {
            modifiedAudited.LastModificationTime = DateTime.UtcNow;
            modifiedAudited.LastModifierUserId = await currentUser.GetUserId();
        }
    }

    internal virtual async Task SetDeletionAudit(object entity)
    {
        if (entity is IDeletionAudited<TUserId> deletedAudited)
        {
            deletedAudited.DeletionTime = DateTime.UtcNow;
            deletedAudited.DeleterUserId = await currentUser.GetUserId();
        }
    }
    
    internal virtual IQueryable<TEntity> GetQueryable()
    {
        return context.Set<TEntity>().Where(ApplySoftDeleteFilter());
    }
}