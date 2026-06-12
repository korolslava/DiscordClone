using FluentValidation;

namespace DiscordClone.Application.Features.Channels.Commands;

public sealed class CreateChannelCommandValidator
    : AbstractValidator<CreateChannelCommand>
{
    private static readonly string[] ValidTypes = ["Text", "Voice", "Announcement"];

    public CreateChannelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Channel name is required.")
            .MaximumLength(100).WithMessage("Channel name must not exceed 100 characters.")
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Channel name can only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.Type)
            .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.");

        RuleFor(x => x.Topic)
            .MaximumLength(1024)
            .WithMessage("Topic must not exceed 1024 characters.")
            .When(x => x.Topic is not null);
    }
}