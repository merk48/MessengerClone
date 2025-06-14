using FluentValidation;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.General.Helpers;

namespace MessengerClone.Service.Features.Chats.Validators
{
    public class RenameGroupChatDtoValidator : AbstractValidator<RenameGroupChatDto>
    {
        public RenameGroupChatDtoValidator()
        {
            RuleFor(x => x.NewTitle)
                .NotEmpty().WithMessage("New title is required.")
                .MaximumLength(ValidationHelper.MaxTitleLength)
                    .WithMessage($"New title must be at most {ValidationHelper.MaxTitleLength} characters.");
        }
    }
}
