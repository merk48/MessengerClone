using AutoMapper;
using AutoMapper.QueryableExtensions;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.General.Extentions;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.MessageStatuses.DTOs;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using MessengerClone.Service.Features.Users.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MessengerClone.Service.Features.MessageStatuses.Services
{
    public class MessageStatusService(IUnitOfWork _unitOfWork, IMapper _mapper, IUserService _userService) : IMessageStatusService
    {

        public async Task<Result<MessageStatusDto>> AddMessageStatusAsync(AddMessageStatusDto dto)
        {
            try
            {
                var entity = _mapper.Map<MessageStatus>(dto);

                await _unitOfWork.Repository<MessageStatus>().AddAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                if(!saveReult.Succeeded)
                    return Result<MessageStatusDto>.Failure("Failed to add message status to database");

                var messageStatuesDto = _mapper.Map<MessageStatusDto>(entity);

                return Result<MessageStatusDto>.Success(messageStatuesDto);

            }
            catch (Exception)
            {
                // Log.
                return Result<MessageStatusDto>.Failure("Failed to add message status to database");
            }
        }

        public async Task<Result<DataResult<MessageStatusDto>>> AddMessageInfoAsync(IEnumerable<AddMessageStatusDto> dto)
        {
            try
            {
                var entities = _mapper.Map<List<MessageStatus>>(dto);

                await _unitOfWork.Repository<MessageStatus>().AddRangeAsync(entities);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                if (!saveReult.Succeeded)
                    return Result<DataResult<MessageStatusDto>>.Failure("Failed to add message info of statuses to database");

                var messageStatuesDtos = _mapper.Map<List<MessageStatusDto>>(entities);

                return Result<DataResult<MessageStatusDto>>.Success(new DataResult<MessageStatusDto>
                {
                    Data = messageStatuesDtos ?? Enumerable.Empty<MessageStatusDto>(),
                    TotalRecordsCount = messageStatuesDtos!.Count
                });

            }
            catch (Exception)
            {
                // Log.
                return Result<DataResult<MessageStatusDto>>.Failure("Failed to add message info of statuses to database");
            }
        }

        public async Task<Result> MarkAsDeliveredAsync(int chatId, int messageId, int userId)
        {
            try
            {
                var status = await _unitOfWork.Repository<MessageStatus>().GetAsync(x => x.Message.ChatId == chatId && x.MessageId == messageId && x.MemberId == userId);

                if (status == null)
                    return Result.Failure("Status not found!");

                status.Status = enMessageStatus.Delivered;
                status.DeliveredAt = DateTime.UtcNow;

                await _unitOfWork.Repository<MessageStatus>().UpdateAsync(status);

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

        public async Task<Result> MarkAsReadAsync(int chatId, int messageId, int userId)
        {
            try
            {
                var status = await _unitOfWork.Repository<MessageStatus>().GetAsync(x => x.Message.ChatId == chatId && x.MessageId == messageId && x.MemberId == userId);

                if (status == null) 
                    return Result.Failure("Status not found!");

                status.Status = enMessageStatus.Read;
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

        public async Task<Result<DataResult<MessageStatusDto>>> GetStatusesForMessageAsync(int chatId, int messageId)
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

        public async Task<Result<int>> GetChatUnreadMessagesCountForUserAsync(int chatId, int currentUserId)
        {
            try
            {
                var unReadMessagesCount = await _unitOfWork.Repository<MessageStatus>().Table
                    .Where(x => x.Message.ChatId == chatId && x.MemberId == currentUserId && x.Status != enMessageStatus.Read)
                    .CountAsync();

                return Result<int>.Success(unReadMessagesCount);
            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<int>.Failure("Failed to retrieve chat un read it messages"); ;
            }
        }

        public async Task<Result<DataResult<MessageDto>>> GetChatUnreadMessagesForUserAsync(int chatId, int currentUserId, CancellationToken cancellationToken, int? page = null, int? size = null)
        {
            try
            {
                var query = _unitOfWork.Repository<MessageStatus>().Table
                     .Where(x => x.Message.ChatId == chatId && x.MemberId == currentUserId && x.Status != enMessageStatus.Read)
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
                return Result<DataResult<MessageDto>>.Failure("Failed to retrieve chat un read messages"); ;
            }
        }

    }
}
