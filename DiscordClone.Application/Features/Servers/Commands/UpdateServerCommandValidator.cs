using FluentValidation;

namespace DiscordClone.Application.Features.Servers.Commands;

public sealed class UpdateServerCommandValidator
    : AbstractValidator<UpdateServerCommand>
{
    public UpdateServerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Server name is required.")
            .MinimumLength(2).WithMessage("Server name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Server name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1024)
            .WithMessage("Description must not exceed 1024 characters.")
            .When(x => x.Description is not null);
    }
}