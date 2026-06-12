using FluentValidation;

namespace DiscordClone.Application.Features.Messages.Commands;

public sealed class EditMessageCommandValidator
    : AbstractValidator<EditMessageCommand>
{
    public EditMessageCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content cannot be empty.")
            .MaximumLength(4000)
            .WithMessage("Message cannot exceed 4000 characters.");
    }
}