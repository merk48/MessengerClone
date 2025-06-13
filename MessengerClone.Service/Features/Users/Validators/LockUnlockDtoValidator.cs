using FluentValidation;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.Users.Validators
{
    public class LockUnlockDtoValidator : AbstractValidator<LockUnlockDto>
    {
        public LockUnlockDtoValidator()
        {
            RuleFor(x => x.Lock)
                .NotNull().WithMessage("Lock flag is required.");
        }
    }


}
