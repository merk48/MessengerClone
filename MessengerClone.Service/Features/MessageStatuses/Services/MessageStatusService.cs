using AutoMapper;
using AutoMapper.QueryableExtensions;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.General.Extentions;
using MessengerClone.Service.Features.MessageStatuses.DTOs;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace MessengerClone.Service.Features.MessageStatuses.Services
{
    public class MessageStatusService(IUnitOfWork _unitOfWork, IMapper _mapper) : IMessageStatusService
    {

        public async Task<Result> MarkAsDeliveredAsync(int messageId, int userId)
        {
            try
            {
                var status = await _unitOfWork.Repository<MessageStatus>().GetAsync(x => x.MessageId == messageId && x.UserId == userId);

                if (status == null)
                    return Result.Failure("Status not found!");

                status.DeliveredAt = DateTime.UtcNow;

                var saveReult =  await _unitOfWork.SaveChangesAsync();

                return saveReult.Succeeded 
                    ?  Result.Success()
                    : Result.Failure("Failed to update message status to delivered");
            }
            catch (Exception)
            {
                // Log.
                return Result.Failure("Failed to update message status to delivered");
            }
        }

        public async Task<Result> MarkAsReadAsync(int messageId, int userId)
        {
            try
            {
                var status = await _unitOfWork.Repository<MessageStatus>().GetAsync(x => x.MessageId == messageId && x.UserId == userId);

                if (status == null) 
                    return Result.Failure("Status not found!");

                status.ReadAt = DateTime.UtcNow;

                var saveReult = await _unitOfWork.SaveChangesAsync();

                return saveReult.Succeeded
                    ? Result.Success()
                    : Result.Failure("Failed to update message status to read");

            }
            catch (Exception)
            {
                // Log.
                return Result.Failure("Failed to update message status to read");
            }
        }

        public async Task<Result<DataResult<MessageStatusDto>>> GetStatusesForMessageAsync(int messageId)
        {
            try
            {
                var query = _unitOfWork.Repository<MessageStatus>().Table
                        .AsNoTracking()
                        .Where(x => x.MessageId == messageId).AsQueryable();

                var messageStatuseDtos = await query.ProjectTo<MessageStatusDto>(_mapper.ConfigurationProvider).ToListAsync();

                return Result<DataResult<MessageStatusDto>>.Success(new DataResult<MessageStatusDto>
                {
                    Data = messageStatuseDtos ?? Enumerable.Empty<MessageStatusDto>(),
                    TotalRecordsCount = messageStatuseDtos!.Count()
                });
            }
            catch (Exception)
            {
                // Log.
                return Result<DataResult<MessageStatusDto>>.Failure("Failed to retireve message info from the database");
            }
        }

        public async Task<Result<int>> GetChatUnreadMessagesForUserCountAsync(int chatId, int currentUserId)
        {
            try
            {
                var unReadMessagesCount = await _unitOfWork.Repository<MessageStatus>().Table
                    .Where(x => x.Message.ChatId == chatId && x.UserId == currentUserId && x.Status != enMessageStatus.Read)
                    .CountAsync();

                return Result<int>.Success(unReadMessagesCount);
            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<int>.Failure("Failed to retrieve chat un read it messages"); ;
            }
        }

        public async Task<Result<DataResult<MessageDto>>> GetChatUnreadMessagesForUserAsync(int chatId, int currentUserId, int? page = null, int? size = null)
        {
            try
            {
                var query = _unitOfWork.Repository<MessageStatus>().Table
                     .Where(x => x.Message.ChatId == chatId && x.UserId == currentUserId && x.Status != enMessageStatus.Read)
                     .Include(x => x.Message)
                     .Select(x => x.Message)
                     .Distinct()
                     .Include(x => x.Sender)
                     .Include(x => x.Attachment)
                     .Include(x => x.MessageInfo)
                     .Include(x => x.MessageReactions)
                     .OrderByDescending(x => x.CreatedAt)
                     .AsQueryable();


                var totalCount = await query.CountAsync();

                if (page.HasValue && size.HasValue)
                    query = query.Pagination(page.Value, size.Value);

                var messages = await query.ToListAsync();

                List<MessageDto> messageDtos = new();

                foreach (var msg in messages)
                {
                    var dto = _mapper.Map<MessageDto>(msg, opts => {
                        opts.Items["JoinedAt"] = msg.CreatedAt;
                    });
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
                return Result<DataResult<MessageDto>>.Failure("Failed to retrieve chat un read messages"); ;
            }
        }

    }
}
