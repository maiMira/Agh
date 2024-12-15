using Agh.Domain.Base.Entities.Interfaces;

namespace Agh.Domain.Base.Entities;

public class AuditedEntity<TId,TUserId> : Entity<TId>,
    ICreationAudited<TUserId>,
    IModificationAudited<TUserId>,
    IDeletionAudited<TUserId>
{
    public DateTime CreationTime { get; set; }
    public TUserId CreatorUserId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public TUserId? LastModifierUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public TUserId? DeleterUserId { get; set; }

    protected AuditedEntity(TId id, TUserId creatorUserId) : base(id)
    {
        CreatorUserId = creatorUserId;
        CreationTime = DateTime.Now;    
    }
}