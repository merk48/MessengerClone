using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.General.Helpers;

namespace MessengerClone.Service.Features.Chats.Validators
{
    public class AddGroupChatDtoValidator : AbstractValidator<AddGroupChatDto>
    {
        public AddGroupChatDtoValidator()
        {
            RuleFor(x => x.MemberIds)
                .NotNull().WithMessage("MemberIds is required.")
                .Must(arr => arr.Length >= 1)
                    .WithMessage("Group chat must have at least one other member.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Group title is required.")
                .MaximumLength(ValidationHelper.MaxTitleLength)
                    .WithMessage($"Group title must be at most {ValidationHelper.MaxTitleLength} characters.");

            When(x => x.GroupCoverImage != null, () =>
            {
                RuleFor(x => x.GroupCoverImage!)
                    .Must(file => ValidationHelper.HasAllowedExtension(file, enMediaType.Image))
                        .WithMessage($"Cover image must be one of: {string.Join(", ", ValidationHelper.ImageExtensions)}")
                    .Must(file => ValidationHelper.IsWithinAllowedSize(file, enMediaType.Image))
                        .WithMessage($"Cover image must be {ValidationHelper.MaxImageSize / (1024 * 1024)}MB or smaller.");
            });

            RuleFor(x => x.Description)
                .MaximumLength(ValidationHelper.MaxDescriptionLength)
                    .WithMessage($"Description must be at most {ValidationHelper.MaxDescriptionLength} characters.")
                .When(x => x.Description != null);
        }
    }
}
