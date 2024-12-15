using Agh.Domain.Base.Entities.Interfaces;

namespace Agh.Domain.Base.Entities;

public class AuditedAggregateRoot<TId, TUserId>(TUserId creatorUserId) : AggregateRoot<TId>,
    IAggregateRoot<TId>,
    ICreationAudited<TUserId>,
    IModificationAudited<TUserId>,
    IDeletionAudited<TUserId>
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public TUserId? DeleterUserId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public TUserId? LastModifierUserId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public TUserId CreatorUserId { get; set; } = creatorUserId;
}