using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.MessageReactions.DTOs;
using MessengerClone.Service.Features.MessageReactions.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading;
using AutoMapper.QueryableExtensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace MessengerClone.Service.Features.MessageReactions.Services
{
    public class MessageReactionService(IUnitOfWork _unitOfWork, IMapper _mapper) : IMessageReactionService
    {
        public async Task<Result<DataResult<MessageReactionDto>>> GetAllMessageReactionsAsync(int chatId, int messageId, int currentUserId)
        {
            try
            {
                var query = _unitOfWork.Repository<MessageReaction>()
                    .Table.Where(x => x.ChatId == chatId && x.UserId == currentUserId && x.MessageId == messageId)
                    .AsQueryable();

                var totalCount = await query.CountAsync();

                var reactionDtos = await query.ProjectTo<MessageReactionDto>(_mapper.ConfigurationProvider).ToListAsync();

                return Result<DataResult<MessageReactionDto>>.Success(new DataResult<MessageReactionDto>()
                {
                    Data = reactionDtos ?? Enumerable.Empty<MessageReactionDto>(),
                    TotalRecordsCount = totalCount
                });
            }
            catch (Exception)
            {
                // Log.
                return Result<DataResult<MessageReactionDto>>.Failure("Failed to retrieve message reactions from the database");
            }
        } 
        
        public async Task<Result<MessageReactionDto>> AddReactToMessageAsync(int chatId, int messageId, int currentUserId, AddMessageReactionDto dto)
        {
            try
            {
                var entity = _mapper.Map<MessageReaction>(dto, opt =>
                {
                    opt.Items["ChatId"] = chatId;
                    opt.Items["MessageId"] = messageId;
                    opt.Items["CurrentUserId"] = currentUserId;
                });

                await _unitOfWork.Repository<MessageReaction>().AddAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                MessageReactionDto reactionDto = _mapper.Map<MessageReactionDto>(entity);

                return saveReult.Succeeded
                    ? Result<MessageReactionDto>.Success(reactionDto)
                    : Result<MessageReactionDto>.Failure("Failed to add message reaction to the database");

            }
            catch (Exception)
            {
                // Log.
                return Result<MessageReactionDto>.Failure("Failed to add message reaction to the database");
            }
        }

        public async Task<Result> RemoveReactionToMessageAsync(int chatId, int messageId, int currentUserId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<MessageReaction>()
                    .GetAsync(x => x.UserId == currentUserId && x.MessageId == messageId && x.ChatId == chatId);

                if(entity == null)
                    return Result<MessageReactionDto>.Failure("There is no reaction found");

                await _unitOfWork.Repository<MessageReaction>().DeleteAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                return saveReult.Succeeded
                    ? Result<MessageReactionDto>.Success()
                    : Result<MessageReactionDto>.Failure("Failed to delete message reaction from the database");

            }
            catch (Exception)
            {
                // Log.
                return Result<MessageReactionDto>.Failure("Failed to delete message reaction from the database");
            }
        }
    }
}
