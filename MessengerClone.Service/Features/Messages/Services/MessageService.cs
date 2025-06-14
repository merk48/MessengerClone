using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.General.Extentions;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.MediaAttachments.Interfaces;
using MessengerClone.Service.Features.Messages.DTOs;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using MessengerClone.Service.Features.Users.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MessengerClone.Service.Features.Messages.Interfaces
{
          
    public class MessageService(IUnitOfWork _unitOfWork, IMapper _mapper, IChatMemeberService _memeberService,IMediaAttachmentService _attachmentService,
        IMessageStatusService _messageStatusService, IUserService _userService) 
        : IMessageService     
    {
        public async Task<Result<DataResult<MessageDto>>> GetChatMessagesForUserAsync(int chatId, int currentUserId,CancellationToken cancellationToken, int? page = null, int? size = null,string? strFilter = null, Expression<Func<Message, bool>>? filter = null)
        {
            try 
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var memberInChatResult = await _memeberService.IsUserMemberInChatAsync(chatId,currentUserId);

                if (!memberInChatResult.Succeeded)
                {
                    return Result<DataResult<MessageDto>>.Failure(memberInChatResult.ToString());
                }

                if (!memberInChatResult.Data)
                    return Result<DataResult<MessageDto>>.Failure("User is not a member of this chat, can't be send a message in it!");

                if (page == null) size = 1;
                if(size == null) size = 50;

                var UnreadCountResult = await _messageStatusService.GetChatUnreadMessagesForUserCountAsync(chatId, currentUserId);

                int UnreadCount = 0;

                if (!UnreadCountResult.Succeeded)
                    return Result<DataResult<MessageDto>>.Failure(memberInChatResult.Errors);

                UnreadCount = UnreadCountResult.Data;

                var query = _unitOfWork.Repository<Message>().Table
                        .AsNoTracking()
                        .Include(x => x.Sender)
                        .Include(x => x.Attachment)
                        .Include(x => x.MessageInfo)
                        .Include(x => x.MessageReactions)
                        .Where(x => x.ChatId == chatId)
                        .AsQueryable();

                if (filter is not null)
                    query = query.Where(filter);

                if (strFilter is not null)
                    query = query.Where(FilterExpressionHelper<Message>.GetFilter(strFilter));

                query = query
                    .OrderByDescending(x => x.CreatedAt);

                var totalCount = await query.CountAsync();

                if (page.HasValue && size.HasValue)
                    query = query.Pagination(page.Value, size.Value + UnreadCount); // test this

                var messages = await query.ToListAsync();

                List<MessageDto> messageDtos = new();

                foreach (var msg in messages)
                {
                    var dto = await MapperHelper.BuildMessageDto(msg, _userService, _mapper, cancellationToken);

                    messageDtos.Add(dto);
                }

                if (page.HasValue && size.HasValue)
                {
                    return Result<DataResult<MessageDto>>.Success(
                        new PaginatedResult<MessageDto>
                        {
                            Data = messageDtos ?? Enumerable.Empty<MessageDto>(),
                            TotalRecordsCount = totalCount,
                            PageNumber = page.Value,
                            PageSize = size.Value
                        });
                }

                return Result<DataResult<MessageDto>>.Success(
                    new DataResult<MessageDto>
                    {
                        Data = messageDtos ?? Enumerable.Empty<MessageDto>(),
                        TotalRecordsCount = totalCount
                    });

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<DataResult<MessageDto>>.Failure("Failed to retrieve chat history from the database"); ;
            }
        }

        public async Task<Result<LastMessageDto>> GetLatestMessageInChatAsync(int chatId, int currentUserId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Message? entity = await _unitOfWork.Repository<Message>()
                    .GetAsync(x => x.ChatId == chatId && x.SenderId == currentUserId
                    , include: x => x.Include(x => x.Sender)
                                     .Include(x => x.Attachment)
                                     .Include(x => x.MessageInfo)
                                     .Include(x => x.MessageReactions));

                if (entity == null)
                    return Result<LastMessageDto>.Failure("Message not found!");

                LastMessageDto messageDto = _mapper.Map<LastMessageDto>(entity);

                return Result<LastMessageDto>.Success(messageDto);

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<LastMessageDto>.Failure("Failed to retrieve the message from the database"); ;
            }
        }

        public async Task<Result<MessageDto>> GetMessageByIdForUserAsync(int id, int currentUserId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
               
                Message? entity = await _unitOfWork.Repository<Message>()
                    .GetAsync(x => x.Id == id && x.SenderId ==currentUserId 
                    ,include : x=> x.Include(x => x.Sender)
                                     .Include(x => x.Attachment)
                                     .Include(x => x.MessageInfo)
                                     .Include(x => x.MessageReactions));

                if (entity == null)
                    return Result<MessageDto>.Failure("Message not found!");

                MessageDto messageDto = await MapperHelper.BuildMessageDto(entity,_userService ,_mapper, cancellationToken) ;

                return Result<MessageDto>.Success(messageDto);

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<MessageDto>.Failure("Failed to retrieve the message from the database"); ;
            }
        }

        public async Task<Result<MessageDto>> AddMessageAsync(AddMessageDto dto, int senderId, int chatId, CancellationToken cancellationToken)
        {
            var hasOwnTr = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
               
                var userIsMemberResult = await _memeberService.IsUserMemberInChatAsync(chatId, senderId);
                if (!userIsMemberResult.Succeeded)
                {
                    return Result<MessageDto>.Failure(userIsMemberResult.ToString());
                }

                if(!userIsMemberResult.Data)
                    return Result<MessageDto>.Failure("User is not a member of this chat, can't be send a message in it!");


                var startTrResult = await _unitOfWork.StartTransactionAsync();

                if (startTrResult.Succeeded)
                    hasOwnTr = true;
                else if (startTrResult.FailureType == enFailureType.TransactionInProgress)
                    hasOwnTr = false;
                else
                    return Result<MessageDto>.Failure("Falied to start a transaction.");


                Message entity = _mapper.Map<Message>(dto, opt => {
                    opt.Items["SenderId"] = senderId;
                    opt.Items["ChatId"] = chatId;
                });

                await _unitOfWork.Repository<Message>().AddAsync(entity);

                var addMessageResult = _unitOfWork.SaveChanges();
                if (!addMessageResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<MessageDto>.Failure("Failed to save the message to the database");
                }

                #region  handle attachment

                if (dto.AddAttachmentDto is not null &&  dto.Type == enMessageType.Image || dto.Type == enMessageType.Audio || dto.Type == enMessageType.Video)
                {
                    var addAttachmentResult = await _attachmentService.AddAsync(entity.Id ,dto.AddAttachmentDto!);
                    if (!addAttachmentResult.Succeeded)
                    {
                        if (hasOwnTr) await _unitOfWork.RollbackAsync();
                        return Result<MessageDto>.Failure(addAttachmentResult.ToString());
                    }
                }

                #endregion


                var messageDtoResult = await GetMessageByIdForUserAsync(entity.Id, senderId,cancellationToken);

                if (!messageDtoResult.Succeeded )
                    return Result<MessageDto>.Failure("Failed to add the message.");


                if (!hasOwnTr)
                    return Result<MessageDto>.Success(messageDtoResult.Data);

                var commitTrResult = await _unitOfWork.CommitAsync();
                if (!commitTrResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result<MessageDto>.Failure("Failed to commit transaction.");
                }

                return Result<MessageDto>.Success(messageDtoResult.Data);

            }
            catch (Exception)
            {
                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                //Log.Error(ex.Message);
                return Result<MessageDto>.Failure("Failed to add the message to the database"); ;
            }
        }

        public async Task<Result<MessageDto>> PinMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Message? entity = await _unitOfWork.Repository<Message>()
                    .GetAsync(x => x.Id == Id && x.SenderId == currentUserId
                    , include: x => x.Include(x => x.Sender)
                                     .Include(x => x.Attachment)
                                     .Include(x => x.MessageInfo)
                                     .Include(x => x.MessageReactions));

                if (entity == null)
                    return Result<MessageDto>.Failure("Message not found!");

                entity.IsPinned = true;
                entity.PinnedById = currentUserId;

                await _unitOfWork.Repository<Message>().UpdateAsync(entity);
                var updateSaveResult = await _unitOfWork.SaveChangesAsync();

                if(!updateSaveResult.Succeeded)
                    return Result<MessageDto>.Failure("Failed to pin the message!");


                MessageDto messageDto = await MapperHelper.BuildMessageDto(entity, _userService, _mapper, cancellationToken);

                return Result<MessageDto>.Success(messageDto);

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<MessageDto>.Failure("Failed to pin the message!");
            }
        }

        public async Task<Result<MessageDto>> UnPinMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Message? entity = await _unitOfWork.Repository<Message>()
                    .GetAsync(x => x.Id == Id && x.SenderId == currentUserId
                    , include: x => x.Include(x => x.Sender)
                                     .Include(x => x.Attachment)
                                     .Include(x => x.MessageInfo)
                                     .Include(x => x.MessageReactions));

                if (entity == null)
                    return Result<MessageDto>.Failure("Message not found!");

                entity.IsPinned = false;
                entity.PinnedById = null;

                await _unitOfWork.Repository<Message>().UpdateAsync(entity);
                var updateSaveResult = await _unitOfWork.SaveChangesAsync();

                if (!updateSaveResult.Succeeded)
                    return Result<MessageDto>.Failure("Failed to unpin the message!");


                MessageDto messageDto = await MapperHelper.BuildMessageDto(entity, _userService, _mapper, cancellationToken);

                return Result<MessageDto>.Success(messageDto);

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<MessageDto>.Failure("Failed to unpin the message!");
            }
        }
        
        public async Task<Result<MessageDto>> DeleteMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
               
                Message? entity = await _unitOfWork.Repository<Message>()
                   .GetAsync(x => x.Id == Id && x.SenderId == currentUserId
                   , include: x => x.Include(x => x.Sender)
                                    .Include(x => x.Attachment)
                                    .Include(x => x.MessageInfo)
                                    .Include(x => x.MessageReactions));

                if (entity == null)
                {
                    return Result<MessageDto>.Failure("Message not found!");
                }

                // handle attachment
                if (entity.Attachment is not null)
                {
                    var deleteAttachmentResult = await _attachmentService.DeleteAsync(entity.Id);
                    if (!deleteAttachmentResult.Succeeded)
                    {
                        return Result<MessageDto>.Failure(deleteAttachmentResult.ToString());
                    }
                }

                await _unitOfWork.Repository<Message>().DeleteAsync(entity);

                var saveResult = await _unitOfWork.SaveChangesAsync();
                if (!saveResult.Succeeded)
                {
                    return Result<MessageDto>.Failure("Failed to delete the message from the database");
                }

                MessageDto messageDto = await MapperHelper.BuildMessageDto(entity, _userService, _mapper, cancellationToken);

                return Result<MessageDto>.Success(messageDto);
            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<MessageDto>.Failure("Failed to delete the message from the database");
            }
        }

        public async Task<Result<MessageDto>> UndoDeleteMessageAsync(int Id, int chatId, int currentUserId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Message? entity = await _unitOfWork.Repository<Message>()
                   .GetAsync(x => x.Id == Id && x.SenderId == currentUserId
                   , include: x => x.Include(x => x.Sender)
                                    .Include(x => x.Attachment)
                                    .Include(x => x.MessageInfo)
                                    .Include(x => x.MessageReactions));

                if (entity == null)
                    return Result<MessageDto>.Failure("Message not found!");

                entity.UndoDelete();

                await _unitOfWork.Repository<Message>().UpdateAsync(entity);

                var saveResult = _unitOfWork.SaveChanges();

                if (!saveResult.Succeeded)
                    return Result<MessageDto>.Failure("Failed to delete the message from the database");

                MessageDto messageDto = await MapperHelper.BuildMessageDto(entity, _userService, _mapper, cancellationToken);

                return messageDto is not null
                    ? Result<MessageDto>.Success(messageDto)
                    : Result<MessageDto>.Failure("Failed to delete the message from the database");
            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<MessageDto>.Failure("Failed to delete the message from the database");
            }
        }

    }
}
