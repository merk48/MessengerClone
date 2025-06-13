using AutoMapper;
using AutoMapper.Execution;
using MessengerClone.Domain.Common.Interfaces;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MessengerClone.Service.Features.ChatMembers.Services
{
    public class ChatMemeberService(IUnitOfWork _unitOfWork, IMapper _mapper) : IChatMemeberService
    {
        public async Task<Result<ChatMemberDto>> GetMemberInChatAsync(int userId, int chatId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<ChatMember>().GetAsync(x => x.UserId == userId && x.ChatId == chatId);

                if (entity == null)
                    return Result<ChatMemberDto>.Failure("Chat member not found");

                var memberDto = _mapper.Map<ChatMemberDto>(entity);

                return Result<ChatMemberDto>.Success(memberDto);

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result<ChatMemberDto>.Failure("Falied to delete the chat member from the database");
            }
        }

        public async Task<Result> IsUserMemberInChat(int chatId, int currentUserId)
        {
            try
            {
                bool userExsists = await _unitOfWork.Repository<ChatMember>()
                    .GetAsync(m => m.UserId == currentUserId && m.ChatId == chatId) != null;

                return userExsists
                    ? Result.Success()
                    : Result.Failure("User is not a member of this chat");

            }
            catch (Exception)
            {
                //Log.Error(ex.Message);
                return Result.Failure("Failed to retrieve this chat from the database"); ;
            }
        }

        public async Task<Result<ChatMemberDto>> AddMemberToChatAsync(AddChatMemberDto dto)
        {
            try
            {
                var entity = _mapper.Map<ChatMember>(dto);

                await _unitOfWork.Repository<ChatMember>().AddAsync(entity);

                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (!saveResult.Succeeded)
                    return Result<ChatMemberDto>.Failure("Failed to save chat member.");

                var chatMemberDto = _mapper.Map<ChatMemberDto>(entity);

                return Result<ChatMemberDto>.Success(chatMemberDto);
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return Result<ChatMemberDto>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result<DataResult<ChatMemberDto>>> AddRangeOfMembersToChatAsync(IEnumerable<AddChatMemberDto> dto)
        {
            try
            {
                // map to chatMemeber
                var entities = _mapper.Map<List<ChatMember>>(dto);

                await _unitOfWork.Repository<ChatMember>().AddRangeAsync(entities);

                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (!saveResult.Succeeded)
                    return Result<DataResult<ChatMemberDto>>.Failure("Failed to save chat members.");

                var chatMemberDtos = _mapper.Map<List<ChatMemberDto>>(entities);

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
                var member = await _unitOfWork.Repository<ChatMember>().GetAsync(x => x.UserId == userId && x.ChatId == chatId);

                if (member == null)
                    return Result<ChatMemberDto>.Failure("Member not found!");

                member.ChatRole = dto.ChatRole;

                var memberDto = _mapper.Map<ChatMemberDto>(member);

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
