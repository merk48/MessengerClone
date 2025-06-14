using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Service.Features.MediaAttachments.DTOs;

namespace MessengerClone.Service.Features.MediaAttachments.Profiles
{
    public class MediaAttachmentProfile : Profile
    {
        public MediaAttachmentProfile()
        {
            CreateMap<MediaAttachment, MediaAttachmentDto>();

            CreateMap<AddAttachmentDto, MediaAttachment>()
                .ForMember(dest => dest.AttachmentUrl,
                opt => opt.MapFrom((src, dest, destMember, context) => (string)context.Items["AttachmentUrl"]));


        }
    }

}
