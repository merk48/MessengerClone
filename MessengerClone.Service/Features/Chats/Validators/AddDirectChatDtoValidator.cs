using FluentValidation;
using MessengerClone.Service.Features.Chats.DTOs;

namespace MessengerClone.Service.Features.Chats.Validators
{
    public class AddDirectChatDtoValidator : AbstractValidator<AddDirectChatDto>
    {
        public AddDirectChatDtoValidator()
        {
            RuleFor(x => x.OtherMemberId)
                .GreaterThan(0)
                .WithMessage("You must specify a valid user ID for the other member.");
        }
    }
}
