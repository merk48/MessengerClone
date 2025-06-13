using FluentValidation;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.Accounts.Validators
{
    public class DeactivateReactivateAccountDtoValidator : AbstractValidator<DeactivateReactivateAccountDto>
    {
        public DeactivateReactivateAccountDtoValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive flag is required.");
        }
    }


}
