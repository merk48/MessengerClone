using FluentValidation;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.Users.DTOs;

namespace MessengerClone.Service.Features.Users.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(ValidationHelper.MaxNameLength).WithMessage($"First name must be ≤ {ValidationHelper.MaxNameLength} chars.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(ValidationHelper.MaxNameLength).WithMessage($"Last name must be ≤ {ValidationHelper.MaxNameLength} chars.");
        }
    }


}
