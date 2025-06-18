using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;


namespace MessengerClone.Service.Features.ChatMembers.Validators
{
    public class AddChatMemberDtoValidator : AbstractValidator<AddChatMemberDto>
    {
        public AddChatMemberDtoValidator()
        {
            RuleFor(x => x.ChatRole)
                 .Must(type => Enum.IsDefined(typeof(enChatRole), type))
                 .WithMessage("Invalid chat role type.");
        }

    }
}
