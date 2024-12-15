namespace Agh.Domain.Base.Specifications;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
}