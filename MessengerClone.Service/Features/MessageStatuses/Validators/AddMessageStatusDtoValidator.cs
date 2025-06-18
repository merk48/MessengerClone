using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.MessageStatuses.DTOs;


namespace MessengerClone.Service.Features.MessageStatuses.Validators
{
    public class AddMessageStatusDtoValidator : AbstractValidator<AddMessageStatusDto>
    {
        public AddMessageStatusDtoValidator()
        {
            RuleFor(x => x.Status)
               .Must(type => Enum.IsDefined(typeof(enMessageStatus), type))
               .WithMessage("Invalid message status type.");
        }

    }
}
