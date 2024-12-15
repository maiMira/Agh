using Agh.Domain.Base.Exceptions;

public class DomainRuleViolationException : DomainException
{
    public DomainRuleViolationException(string message) : base(message) { }
}