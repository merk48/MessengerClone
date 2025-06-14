using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.General.DTOs;
using Microsoft.EntityFrameworkCore;


namespace MessengerClone.Service.Features.ChatMembers.Services
{
    public class ChatMemeberService(IUnitOfWork _unitOfWork, IMapper _mapper) : IChatMemeberService
    {
        public async Task<Result<ChatMemberDto>> GetMemberInChatAsync(int userId, int chatId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<ChatMember>()
                    .GetAsync(x => x.UserId == userId && x.ChatId == chatId, include: x => x.Include(x => x.User));

                if (entity == null)
                    return Result<ChatMemberDto>.Failure("User is not a member of this chat");

                var memberDto = _mapper.Map<ChatMemberDto>(entity);

                return Result<ChatMemberDto>.Success(memberDto);

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<ChatMemberDto>.Failure("Falied to delete the chat member from the database");
            }
        }

        public async Task<Result<bool>> IsUserMemberInChatAsync(int chatId, int currentUserId)
        {
            try
            {
                bool userExsists = await _unitOfWork.Repository<ChatMember>()
                    .GetAsync(m => m.UserId == currentUserId && m.ChatId == chatId) != null;

                return Result<bool>.Success(userExsists);

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<bool>.Failure("Failed to retrieve this chat from the database"); ;
            }
        }

        public async Task<Result<ChatMemberDto>> AddMemberToChatAsync(AddChatMemberDto dto, int chatId)
        {
            try
            {
                //var exsistingEntity = await _unitOfWork.Repository<ChatMember>().GetAsync(x => x.UserId == userId && x.ChatId == chatId, include: x => x.Include(x => x.User));

                //if (exsistingEntity is not null)
                //{
                //    var exsistingChatMemberDtoResult = _mapper.Map<ChatMemberDto>(exsistingEntity);
                //    return Result<ChatMemberDto>.Success(exsistingChatMemberDtoResult.Data);
                //}

                var entity = await _unitOfWork.Repository<ChatMember>().GetAsync(x => x.UserId == dto.UserId && x.ChatId == chatId);

                if (entity is not null)
                        return Result<ChatMemberDto>.Failure("User is already a member in this chat");

                entity = _mapper.Map<ChatMember>(dto, opt =>
                {
                    opt.Items["ChatId"] = chatId;
                });

                await _unitOfWork.Repository<ChatMember>().AddAsync(entity);

                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (!saveResult.Succeeded)
                    return Result<ChatMemberDto>.Failure("Failed to save chat member.");

                var chatMemberDtoResult = await GetMemberInChatAsync(entity.UserId, entity.ChatId);
                if (!chatMemberDtoResult.Succeeded)
                    return Result<ChatMemberDto>.Failure(chatMemberDtoResult.ToString());

                return Result<ChatMemberDto>.Success(chatMemberDtoResult.Data);
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return Result<ChatMemberDto>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
        
        public async Task<Result<DataResult<ChatMemberDto>>> AddRangeOfMembersToChatAsync(IEnumerable<AddChatMemberDto> dtos, int chatId)
        {
            try
            {
                // map to chatMemeber
                var entities = _mapper.Map<List<ChatMember>>(dtos, opt =>
                {
                    opt.Items["ChatId"] = chatId;
                });

                await _unitOfWork.Repository<ChatMember>().AddRangeAsync(entities);

                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (!saveResult.Succeeded)
                    return Result<DataResult<ChatMemberDto>>.Failure("Failed to save chat members.");

               
                List<ChatMemberDto> chatMemberDtos = new();
                foreach (var member in entities)
                {
                    var memberDtoResult = await GetMemberInChatAsync(member.UserId, member.ChatId);
                    if (!memberDtoResult.Succeeded)
                        return Result<DataResult<ChatMemberDto>>.Failure(memberDtoResult.ToString());

                    chatMemberDtos.Add(memberDtoResult.Data!);
                }

                return Result<DataResult<ChatMemberDto>>.Success(new DataResult<ChatMemberDto>
                {
                    Data = chatMemberDtos,
                    TotalRecordsCount = chatMemberDtos.Count
                });
            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<DataResult<ChatMemberDto>>.Failure("Failed to add chat members");
            }
        }

        public async Task<Result> RemoveMemberFromChatAsync(int userId,int chatId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<ChatMember>().GetAsync(x => x.UserId == userId && x.ChatId == chatId);

                if(entity == null)
                    return Result.Failure($"Chat member not found");

                await _unitOfWork.Repository<ChatMember>().DeleteAsync(entity);

                var saveResult = await _unitOfWork.SaveChangesAsync();

                return saveResult.Succeeded
                    ? Result.Success()
                    : Result.Failure("Failed to delete chat mebmer from the database");

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result.Failure("Falied to delete the chat member from the database");
            }
        }

        public async Task<Result<ChatMemberDto>> ChangeMemberChatRoleAsync(int userId, int chatId, AddMemberChatRoleDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<ChatMember>()
                    .GetAsync(x => x.UserId == userId && x.ChatId == chatId, include: x => x.Include(x => x.User));

                if (entity == null)
                    return Result<ChatMemberDto>.Failure("Member not found!");

                entity.ChatRole = dto.ChatRole;

                await _unitOfWork.Repository<ChatMember>().UpdateAsync(entity);

                var memberDto = _mapper.Map<ChatMemberDto>(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                return saveReult.Succeeded
                    ? Result<ChatMemberDto>.Success(memberDto)
                    : Result<ChatMemberDto>.Failure("Failed to update chat member role to the database");

            }
            catch (Exception)
            {
                // Log.
                return Result<ChatMemberDto>.Failure("Failed to update chat member role to the database");
            }
        }
    }
}
