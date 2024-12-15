namespace Agh.Domain.Base.Validations;

public interface IDomainRule
{
    string Message { get; }
    bool IsBroken();
}