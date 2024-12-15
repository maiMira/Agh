namespace Agh.Domain.Base.Exceptions;

public class NotFoundException(string message) : DomainException(message);
