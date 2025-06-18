using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.MediaAttachments.DTOs;


namespace MessengerClone.Service.Features.MediaAttachments.Validators
{
    public class AddAttachmentDtoDtoValidator : AbstractValidator<AddAttachmentDto>
    {
        public AddAttachmentDtoDtoValidator()
        {
            RuleFor(x => x.Attachment!)
                    .Must(file => ValidationHelper.HasAllowedExtension(file, enMediaType.Image))
                           .WithMessage($"Attachment must be one of the following types: {string.Join(", ", ValidationHelper.ImageExtensions)}.")
                    .Must(file => ValidationHelper.IsWithinAllowedSize(file, enMediaType.Image))
                           .WithMessage($"Attachment must be {ValidationHelper.MaxImageSize / (1024 * 1024)}MB or smaller.");

            RuleFor(x => x.FileType)
               .Must(fileType => fileType == null || Enum.IsDefined(typeof(enMediaType), fileType))
               .WithMessage("Invalid media file type.");
        }

    }
}
