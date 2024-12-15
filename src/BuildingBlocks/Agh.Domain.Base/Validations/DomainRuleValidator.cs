namespace Agh.Domain.Base.Validations;

public static class BusinessRuleValidator
{
    public static void Check(IDomainRule rule)
    {
        if (rule.IsBroken())
            throw new DomainRuleViolationException(rule.Message);
    }

    public static void Check(IEnumerable<IDomainRule> rules)
    {
        foreach (var rule in rules)
        {
            Check(rule);
        }
    }
}