using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.MediaAttachments.DTOs;


namespace MessengerClone.Service.Features.MediaAttachments.Interfaces
{
    public interface IMediaAttachmentService
    {
        Task<Result<AttachmentDto>> AddAsync(int messageId, AddAttachmentDto dto);
        Task<Result> DeleteAsync(int messageId);
    }

}
