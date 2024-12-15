namespace Agh.Infrastructure.Base.Services;

public interface IUserService<TId>
{
    Task<TId> GetUserId();
}