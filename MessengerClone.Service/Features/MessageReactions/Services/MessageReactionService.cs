using AutoMapper;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.MessageReactions.DTOs;
using MessengerClone.Service.Features.MessageReactions.Interfaces;

namespace MessengerClone.Service.Features.MessageReactions.Services
{
    public class MessageReactionService(IUnitOfWork _unitOfWork, IMapper _mapper) : IMessageReactionService
    {
        public async Task<Result<MessageReactionDto>> AddReactToMessageAsync(int messageId, int currentUserId, AddMessageReactionDto dto)
        {
            try
            {
                var entity = _mapper.Map<MessageReaction>(dto, opt =>
                {
                    opt.Items["CurrentUserId"] = currentUserId;
                    opt.Items["MessageId"] = messageId;
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

        public async Task<Result> RemoveReactionToMessageAsync(int messageId, int currentUserId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<MessageReaction>().GetAsync(x => x.UserId == currentUserId && x.MessageId == messageId);

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
