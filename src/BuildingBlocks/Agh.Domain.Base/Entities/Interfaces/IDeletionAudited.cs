namespace Agh.Domain.Base.Entities.Interfaces;
public interface IDeletionAudited<TUserId>
{
    bool IsDeleted { get; set; }
    DateTime? DeletionTime { get; set; }
    TUserId? DeleterUserId { get; set; }
}
