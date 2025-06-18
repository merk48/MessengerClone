using FluentValidation;
using MessengerClone.Service.Features.Messages.DTOs;


namespace MessengerClone.Service.Features.Messages.Validators
{
    public class PinUnPinMessageDtoValidator : AbstractValidator<PinUnPinMessageDto>
    {
        public PinUnPinMessageDtoValidator()
        {
            RuleFor(x => x.Pin)
               .NotNull().WithMessage("Lock flag is required.");
        }

    }

}
