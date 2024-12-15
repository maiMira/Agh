namespace Agh.Domain.Base.Entities.Interfaces;

public interface ICreationAudited<TUserId>
{
    DateTime CreationTime { get; set; }
    TUserId CreatorUserId { get; set; }
}