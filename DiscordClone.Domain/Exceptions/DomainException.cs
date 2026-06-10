namespace DiscordClone.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string name, object key)
        : base($"Entity '{name}' with key '{key}' was not found.") { }
}

public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "You do not have permission.")
        : base(message) { }
}

public sealed class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}

public sealed class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}