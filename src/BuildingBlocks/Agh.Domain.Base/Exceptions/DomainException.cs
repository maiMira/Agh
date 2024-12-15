namespace Agh.Domain.Base.Exceptions;

public class DomainException : Exception
{
    public string Code { get; } = null!;
    public object[] Parameters { get; } = null!;

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, string code)
        : base(message)
    {
        Code = code;
    }

    public DomainException(string message, string code, params object[] parameters)
        : base(message)
    {
        Code = code;
        Parameters = parameters;
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DomainException(string message, string code, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}