using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.General.Helpers;

namespace MessengerClone.Service.Features.Chats.Validators
{
    public class ResetGroupChatImageDtoValidator : AbstractValidator<ResetGroupChatImageDto>
    {
        public ResetGroupChatImageDtoValidator()
        {
            RuleFor(x => x.NewImage)
                .NotNull().WithMessage("A new image file is required.")
                .Must(file => ValidationHelper.HasAllowedExtension(file!, enMediaType.Image))
                    .WithMessage($"Image must be one of: {string.Join(", ", ValidationHelper.ImageExtensions)}")
                .Must(file => ValidationHelper.IsWithinAllowedSize(file!, enMediaType.Image))
                    .WithMessage($"Image must be {ValidationHelper.MaxImageSize / (1024 * 1024)}MB or smaller.");
        }
    }
}
