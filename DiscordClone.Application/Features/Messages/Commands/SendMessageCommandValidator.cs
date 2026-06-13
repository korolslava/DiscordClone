using FluentValidation;

namespace DiscordClone.Application.Features.Messages.Commands;

public sealed class SendMessageCommandValidator
    : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content cannot be empty.")
            .MaximumLength(4000)
            .WithMessage("Message cannot exceed 4000 characters.");

        RuleFor(x => x.ChannelId)
            .NotEmpty().WithMessage("ChannelId is required.");
    }
}