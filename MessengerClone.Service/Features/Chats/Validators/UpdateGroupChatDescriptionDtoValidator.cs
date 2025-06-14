using FluentValidation;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.General.Helpers;

namespace MessengerClone.Service.Features.Chats.Validators
{
    public class UpdateGroupChatDescriptionDtoValidator : AbstractValidator<UpdateGroupChatDescriptionDto>
    {
        public UpdateGroupChatDescriptionDtoValidator()
        {
            RuleFor(x => x.NewDescription)
                .NotEmpty().WithMessage("New description is required.")
                .MaximumLength(ValidationHelper.MaxDescriptionLength)
                    .WithMessage($"Description must be at most {ValidationHelper.MaxDescriptionLength} characters.");
        }
    }
}
