using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.Auth.DTOs;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.Users.Interfaces;
using MessengerClone.Service.Features.Users.Services;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace MessengerClone.Service.Features.Auth.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator(IUserService _userService)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(ValidationHelper.MaxNameLength).WithMessage($"First name must be ≤ {ValidationHelper.MaxNameLength} chars.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(ValidationHelper.MaxNameLength).WithMessage($"Last name must be ≤ {ValidationHelper.MaxNameLength} chars.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(ValidationHelper.PhoneRegex).WithMessage("Phone number must be in E.164 format (e.g. +123456789).");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                 .MustAsync(async (email, cancellationToken) =>
                 {
                     return !await _userService.EmailExistsAsync(email, cancellationToken);
                 }).WithMessage("Email is already in use.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one digit.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be specified.")
                .Must(list => list.All(r => !string.IsNullOrWhiteSpace(r)))
                    .WithMessage("Roles cannot contain empty values.");

            When(x => x.ProfileImage != null, () =>
            {
                RuleFor(x => x.ProfileImage!)
                    .Must(file => ValidationHelper.HasAllowedExtension(file, enMediaType.Image))
                           .WithMessage($"Profile image must be one of the following types: {string.Join(", ", ValidationHelper.ImageExtensions)}.")
                    .Must(file => ValidationHelper.IsWithinAllowedSize(file, enMediaType.Image))
                           .WithMessage($"Profile image must be {ValidationHelper.MaxImageSize / (1024 * 1024)}MB or smaller.");
            });
        }
    }


}
