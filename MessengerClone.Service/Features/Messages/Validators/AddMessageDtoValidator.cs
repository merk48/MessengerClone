using FluentValidation;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.MediaAttachments.DTOs;
using MessengerClone.Service.Features.MessageReactions.DTOs;
using MessengerClone.Service.Features.Messages.DTOs;
using MessengerClone.Service.Features.MessageStatuses.DTOs;


namespace MessengerClone.Service.Features.Messages.Validators
{
    public class AddMessageDtoValidator : AbstractValidator<AddMessageDto>
    {
        public AddMessageDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message Content is required.");

            RuleFor(x => x.Type)
               .Must(type => Enum.IsDefined(typeof(enMessageType), type))
               .WithMessage("Invalid message type.");

            When(x => x.AddAttachmentDto != null, () =>
            {
                RuleFor(x => x.AddAttachmentDto!.Attachment!)
                    .Must(file => ValidationHelper.HasAllowedExtension(file, enMediaType.Image))
                           .WithMessage($"Message attachment must be one of the following types: {string.Join(", ", ValidationHelper.ImageExtensions)}.")
                    .Must(file => ValidationHelper.IsWithinAllowedSize(file, enMediaType.Image))
                           .WithMessage($"Message attachment must be {ValidationHelper.MaxImageSize / (1024 * 1024)}MB or smaller.");

                RuleFor(x => x.AddAttachmentDto!.FileType)
                   .Must(fileType => fileType == null || Enum.IsDefined(typeof(enMediaType), fileType))
                   .WithMessage("Invalid media file type.");

            });
        }

    }
}
