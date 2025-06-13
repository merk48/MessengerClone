using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace MessengerClone.Service.Features.Users.Validators
{
    public class AddUpdateUserProfileImageDtoValidator : AbstractValidator<AddUpdateUserProfileImageDto>
    {
        public AddUpdateUserProfileImageDtoValidator()
        {
            RuleFor(x => x.ProfileImage)
                .NotNull().WithMessage("Profile image is required.")
                 .Must(file => ValidationHelper.HasAllowedExtension(file, enMediaType.Image))
                      .WithMessage($"Profile image must be one of the following types: {string.Join(", ", ValidationHelper.ImageExtensions)}.")
                 .Must(file => ValidationHelper.IsWithinAllowedSize(file, enMediaType.Image))
                      .WithMessage($"Profile image must be {ValidationHelper.MaxImageSize / (1024 * 1024)}MB or smaller.");
        }

    }


}
