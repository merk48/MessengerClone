using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.Files.Helpers;
using MessengerClone.Service.Features.Files.Interfaces;
using MessengerClone.Service.Features.MediaAttachments.DTOs;
using MessengerClone.Service.Features.MediaAttachments.Interfaces;

namespace MessengerClone.Service.Features.MediaAttachments.Services
{
    public class MediaAttachmentService(IUnitOfWork _unitOfWork, IMapper _mapper, IFileService _FileService) 
        : IMediaAttachmentService
    {
        public async Task<Result<MediaAttachmentDto>> AddAsync(int messageId,AddAttachmentDto dto)
        {
            var hasOwnTr = false;

            try
            {
                var startTrResult = await _unitOfWork.StartTransactionAsync();

                if (startTrResult.Succeeded)
                    hasOwnTr = true;
                else if (startTrResult.FailureType == enFailureType.TransactionInProgress)
                    hasOwnTr = false;
                else
                    return Result<MediaAttachmentDto>.Failure("Falied to start a transaction.");


                var result = await _FileService.SaveAsync(dto.Attachment ,
                                    dto.FileType == enMediaType.Image ? enFileCategory.MessageImage : dto.FileType == enMediaType.Audio 
                                    ? enFileCategory.MessageAudio : enFileCategory.MessageVideo
                                    , messageId);

                if(!result.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<MediaAttachmentDto>.Failure("Failed to save message's media attachment");
                }

                MediaAttachment entity = _mapper.Map<MediaAttachment>(dto,opt =>
                {
                    opt.Items["AttachmentUrl"] = result.Data;
                });

                await _unitOfWork.Repository<MediaAttachment>().AddAsync(entity);

                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (!saveResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<MediaAttachmentDto>.Failure("Failed to save message's media attachment to the database");
                }

                MediaAttachmentDto attachmentDto = _mapper.Map<MediaAttachmentDto>(entity);

                return Result<MediaAttachmentDto>.Success(attachmentDto);
            }
            catch (Exception)
            {
                // Log
                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                return Result<MediaAttachmentDto>.Failure("Failed to save message's media attachment to the database");
            }
        }

        public async Task<Result> DeleteAsync(int messageId)
        {
            var hasOwnTr = false;

            try
            {
                var startTrResult = await _unitOfWork.StartTransactionAsync();

                if (startTrResult.Succeeded)
                    hasOwnTr = true;
                else if (startTrResult.FailureType == enFailureType.TransactionInProgress)
                    hasOwnTr = false;
                else
                    return Result<MessageDto>.Failure("Falied to start a transaction.");


                var entity = await _unitOfWork.Repository<MediaAttachment>().GetAsync(x => x.MessageId == messageId);

                var result = await _FileService.DeleteAsync(entity!.AttachmentUrl);

                if (!result.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<MediaAttachmentDto>.Failure("Failed to delete message's media attachment");
                }


                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (!saveResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<MediaAttachmentDto>.Failure("Failed to save message's media attachment to the database");
                }


                return Result.Success();
            }
            catch (Exception)
            {
                // Log
                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                return Result<MediaAttachmentDto>.Failure("Failed to save message's media attachment to the database");
            }
        }
    }

}
