namespace Agh.Domain.Base.Entities.Interfaces;

public interface IModificationAudited<TUserId>
{
    DateTime? LastModificationTime { get; set; }
    TUserId? LastModifierUserId { get; set; }
}
