using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.MessageReactions.DTOs;


namespace MessengerClone.Service.Features.MessageReactions.Validators
{
    public class AddMessageReactionDtoDtoValidator : AbstractValidator<AddMessageReactionDto>
    {
        public AddMessageReactionDtoDtoValidator()
        {
            RuleFor(x => x.ReactionType)
               .Must(type => Enum.IsDefined(typeof(enMessageReactionType), type))
               .WithMessage("Invalid message reaction type.");
        }

    }
}
