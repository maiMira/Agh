    namespace Agh.Domain.Base.Exceptions;

    public class ValidationException : DomainException
    {
        public ValidationException(string message) : base(message) { }
    }