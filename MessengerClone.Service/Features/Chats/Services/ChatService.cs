using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.Files.Helpers;
using MessengerClone.Service.Features.Files.Interfaces;
using MessengerClone.Service.Features.General.DTOs;
using MessengerClone.Service.Features.General.Extentions;
using MessengerClone.Service.Features.General.Helpers;
using MessengerClone.Service.Features.Messages.Interfaces;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using MessengerClone.Service.Features.Users.Interfaces;
using MessengerClone.Service.Features.Users.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;

namespace MessengerClone.Service.Features.Chats.Services
{
    public class ChatService(IUnitOfWork _unitOfWork, IMapper _mapper,IChatMemeberService _memeberService,
            IMessageStatusService _messageStatusService, IFileService _FileService, IUserService _userService, ILogger<ChatService> _logger)
        : IChatService
    {
      
        public async Task<Result<DataResult<ChatSidebarDto>>> GetAllForSidebarByUserIdAsync(int userId, CancellationToken cancellationToken, int? page = null, int? size = null, string? strFilter = null, Expression<Func<Chat, bool>>? filter = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var query = _unitOfWork.Repository<Chat>()
                    .Table                 
                    .AsNoTracking()
                    .Include(c => c.ChatMembers)
                        .ThenInclude(cm => cm.User)
                    .Include(c => c.Messages)
                    .Where(c => c.ChatMembers.Any(cm => cm.UserId == userId))
                    .AsQueryable();

                if (filter is not null)
                    query = query.Where(filter);

                if (strFilter is not null)
                    query = query.Where(FilterExpressionHelper<Chat>.GetFilter(strFilter));

                query = query
                    .OrderByDescending(c =>
                        c.LastMessage != null
                            ? c.LastMessage.SentAt
                            : c.CreatedAt)
                    .ThenByDescending(c => c.CreatedAt);

                var totalCount = await query.CountAsync();

                if (page.HasValue && size.HasValue)
                    query = query.Pagination(page.Value, size.Value);

                var chats = await query.ToListAsync();
                var chatDtos = _mapper.Map<List<ChatSidebarDto>>(chats, opt =>
                {
                    opt.Items["CurrentUserId"] = userId;
                });

                foreach (var chatDto in chatDtos)
                {
                    var UnreadCountResult = await _messageStatusService.GetChatUnreadMessagesCountForUserAsync(chatDto.Id, userId);

                    if (UnreadCountResult.Succeeded)
                        chatDto.UnreadCount = UnreadCountResult.Data;
                }

                if (page.HasValue && size.HasValue)
                {
                    return Result<DataResult<ChatSidebarDto>>.Success(
                        new PaginatedResult<ChatSidebarDto>
                        {
                            Data = chatDtos ?? Enumerable.Empty<ChatSidebarDto>(),
                            TotalRecordsCount = totalCount,
                            PageNumber = page.Value,
                            PageSize = size.Value
                        });
                }

                return Result<DataResult<ChatSidebarDto>>.Success(
                    new DataResult<ChatSidebarDto>
                    {
                        Data = chatDtos ?? Enumerable.Empty<ChatSidebarDto>(),
                        TotalRecordsCount = totalCount
                    });
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(GetAllForSidebarByUserIdAsync));
                return Result<DataResult<ChatSidebarDto>>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(GetAllForSidebarByUserIdAsync));
                return Result<DataResult<ChatSidebarDto>>.Failure("Failed to retrieve user chats from the database") ;
            }
        }

        public async Task<Result<DataResult<int>>> GetUserAllChatIdsAsync(int userId, CancellationToken cancellationToken, int? page = null, int? size = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var query = _unitOfWork.Repository<Chat>().Table
                  .Where(c => c.ChatMembers.Any(cm => cm.UserId == userId))
                    .Select(x => x.Id)
                    .AsQueryable();

                if (page.HasValue && size.HasValue)
                    query = query.Pagination(page.Value, size.Value);

                var ids = await query.ToListAsync();

                return Result<DataResult<int>>.Success(new DataResult<int>
                {
                    Data = ids ?? Enumerable.Empty<int>(),
                    TotalRecordsCount = ids!.Count
                });
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(GetUserAllChatIdsAsync));
                return Result<DataResult<int>>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(GetUserAllChatIdsAsync));
                return Result<DataResult<int>>.Failure("Failed to retrieve chats ids from the database"); ;
            }
        }

        public async Task<Result<ChatMetadataDto>> GetChatMetadataById(int chatId,int currentUserId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();


                var chat = await _unitOfWork.Repository<Chat>()
                    .GetAsync(x => x.Id == chatId, 
                    include: x => x
                    .Include(x => x.LastMessage)
                    .Include(x => x.ChatMembers)
                        .ThenInclude(cm => cm.User));


                if (chat == null)
                {
                    _logger.LogWarning("Chat {Id} not found in {Method}", chatId, nameof(GetChatMetadataById));
                    return Result<ChatMetadataDto>.Failure("Chat not found");
                }

                ChatMetadataDto chatDto = await MapperHelper.BuildChatMetadataDto(chat, currentUserId, _messageStatusService, _mapper);

                return Result<ChatMetadataDto>.Success(chatDto);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(GetChatMetadataById));
                return Result<ChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(GetChatMetadataById));
                return Result<ChatMetadataDto>.Failure("Failed to retrieve this chat from the database"); ;
            }
        }
       
        public async Task<Result<DirectChatMetadataDto>> AddDirectChatAsync(AddDirectChatDto dto, int currentUserId, CancellationToken cancellationToken)
        {
            var hasOwnTr = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (dto.OtherMemberId <= 0)
                    return Result<DirectChatMetadataDto>.Failure("Chat must have at least one other member.");

                if (dto.OtherMemberId == currentUserId)
                    return Result<DirectChatMetadataDto>.Failure("Cannot create a direct chat with yourself.");

                var startTrResult = await _unitOfWork.StartTransactionAsync();

                if (startTrResult.Succeeded)
                    hasOwnTr = true;
                else if (startTrResult.FailureType == enFailureType.TransactionInProgress)
                    hasOwnTr = false;
                else
                    return Result<DirectChatMetadataDto>.Failure("Falied to start a transaction.");


                DirectChat? existing = await _unitOfWork.Repository<DirectChat>()
                    .GetAsync(x =>
                    x.ChatMembers!.Any(m => m.UserId == dto.OtherMemberId)
                    && x.ChatMembers!.Any(m => m.UserId == currentUserId)
                    && x.Type == enChatType.Direct);


                // Already there an chat between them
                if (existing != null)
                {
                    var getExistingDirectDtoResult = await GetChatMetadataById(existing.Id, currentUserId,cancellationToken);

                    if (!getExistingDirectDtoResult.Succeeded || getExistingDirectDtoResult.Data is not DirectChatMetadataDto existingDirectDto)
                    {
                        if (hasOwnTr) await _unitOfWork.RollbackAsync();
                        return Result<DirectChatMetadataDto>.Failure("Failed to load saved group chat metadata.");
                    }
                    var existingDirectDto2 = (await MapperHelper.BuildChatMetadataDto(existing, currentUserId, _messageStatusService, _mapper)) as DirectChatMetadataDto;


                    if (!(await _unitOfWork.CommitAsync()).Succeeded)
                    {
                        if (hasOwnTr) await _unitOfWork.RollbackAsync();
                        return Result<DirectChatMetadataDto>.Failure("Failed to commit transaction.");
                    }

                    _logger.LogInformation("Direct Chat {Id} has retrieved successfully", existingDirectDto.Id);
                    return Result<DirectChatMetadataDto>.Success(existingDirectDto);
                }

                var otherUserResult = await _userService.GetUserByIdAsync(dto.OtherMemberId,cancellationToken);
                if (!otherUserResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<DirectChatMetadataDto>.Failure("Failed to save the chat details.");
                }

                DirectChat directEntity = _mapper.Map<DirectChat>(dto, opt =>
                {
                    opt.Items["Title"] = otherUserResult.Data!.Username;
                });

                directEntity.LastMessage = new();

                 await _unitOfWork.Repository<Chat>().AddAsync(directEntity);

                var saveChatResult = await _unitOfWork.SaveChangesAsync();
                if (!saveChatResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<DirectChatMetadataDto>.Failure("Failed to save the chat details.");
                }

                List<AddChatMemberDto> members = new() 
                {
                    new AddChatMemberDto { UserId = currentUserId,ChatRole = enChatRole.Participant},
                    new AddChatMemberDto { UserId = dto.OtherMemberId,ChatRole = enChatRole.Participant } 
                };


                var addDirectChatMembersResult = await _memeberService.AddRangeOfMembersToChatAsync(members, directEntity.Id);
                if (!addDirectChatMembersResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<DirectChatMetadataDto>.Failure("Failed to save the chat members.");
                }

                Result<ChatMetadataDto> getDirectDtoResult = new();
                if (hasOwnTr)
                {
                    var commitTrResult = await _unitOfWork.CommitAsync();
                    if (!commitTrResult.Succeeded)
                    {
                        await _unitOfWork.RollbackAsync();
                        return Result<DirectChatMetadataDto>.Failure("Failed to commit transaction.");
                    }

                    _logger.LogInformation("New Direct Chat {Id} has added successfully", directEntity.Id);

                    getDirectDtoResult = await GetChatMetadataById(directEntity.Id, currentUserId, cancellationToken);
                    if (!getDirectDtoResult.Succeeded || getDirectDtoResult.Data is not DirectChatMetadataDto directDto)
                        return Result<DirectChatMetadataDto>.Failure("Failed to load saved group chat metadata.");
                    
                    return Result<DirectChatMetadataDto>.Success(directDto);

                }
                else
                {
                    getDirectDtoResult = await GetChatMetadataById(directEntity.Id, currentUserId, cancellationToken);
                    if (!getDirectDtoResult.Succeeded || getDirectDtoResult.Data is not DirectChatMetadataDto directDto)
                        return Result<DirectChatMetadataDto>.Failure("Failed to load saved group chat metadata.");

                    return Result<DirectChatMetadataDto>.Success(directDto);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(AddDirectChatAsync));
                return Result<DirectChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(AddDirectChatAsync));
                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                return Result<DirectChatMetadataDto>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result<GroupChatMetadataDto>> AddGroupChatAsync(AddGroupChatDto dto, int currentUserId, CancellationToken cancellationToken)
        {
            var hasOwnTr = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (dto.MemberIds == null || dto.MemberIds.Length < 1)
                    return Result<GroupChatMetadataDto>.Failure("Chat must have at least one other member.");

                var startTrResult = await _unitOfWork.StartTransactionAsync();

                if (startTrResult.Succeeded)
                    hasOwnTr = true;
                else if (startTrResult.FailureType == enFailureType.TransactionInProgress)
                    hasOwnTr = false;
                else
                    return Result<GroupChatMetadataDto>.Failure("Falied to start a transaction.");


                GroupChat groupEntity = _mapper.Map<GroupChat>(dto);
                groupEntity.LastMessage = new();

                await _unitOfWork.Repository<GroupChat>().AddAsync(groupEntity);

                var saveChatResult = await _unitOfWork.SaveChangesAsync();
                if (!saveChatResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<GroupChatMetadataDto>.Failure("Failed to save the chat details.");
                }

                List<AddChatMemberDto> members = new();

                if (!dto.MemberIds.Contains(currentUserId))
                    members.Add(new AddChatMemberDto { UserId = currentUserId ,ChatRole = enChatRole.GroupOwner });

                foreach (int memberId in dto.MemberIds)
                {
                    members.Add(new AddChatMemberDto { UserId = memberId, ChatRole = enChatRole.GroupMember });
                }

                var addGroupChatMembersResult = await _memeberService.AddRangeOfMembersToChatAsync(members, groupEntity.Id);
                if (!addGroupChatMembersResult.Succeeded)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<GroupChatMetadataDto>.Failure("Failed to save the chat members.");
                }

                #region Handle Group image

                string imageUrl = "";
                if (dto.GroupCoverImage is not null)
                {
                    var saveGroupCoverImageResult = await _FileService.SaveAsync(dto.GroupCoverImage, enFileCategory.GroupCover, groupEntity.Id);

                    if (!saveGroupCoverImageResult.Succeeded)
                    {
                        if (hasOwnTr) await _unitOfWork.RollbackAsync();
                        return Result<GroupChatMetadataDto>.Failure("Failed to save group chat's cover image");
                    }

                    imageUrl = saveGroupCoverImageResult.Data!;
                    groupEntity.GroupCoverImageUrl = saveGroupCoverImageResult.Data!;

                     await _unitOfWork.Repository<GroupChat>().UpdateAsync(groupEntity);
                }

                #endregion

                var result = await GetChatMetadataById(groupEntity.Id, currentUserId, cancellationToken);

                if (!result.Succeeded || result.Data is not GroupChatMetadataDto groupDto)
                {
                    if (hasOwnTr) await _unitOfWork.RollbackAsync();
                    return Result<GroupChatMetadataDto>.Failure("Failed to load saved group chat metadata.");
                }
               

                if (!hasOwnTr)
                    return Result<GroupChatMetadataDto>.Success(groupDto);

                var commitTrResult = await _unitOfWork.CommitAsync();
                if (!commitTrResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result<GroupChatMetadataDto>.Failure("Failed to commit transaction.");
                }

                _logger.LogInformation("New Group Chat {Id} has added successfully", groupEntity.Id);
               
                return Result<GroupChatMetadataDto>.Success(groupDto);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(AddGroupChatAsync));
                return Result<GroupChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(AddGroupChatAsync));
                if (hasOwnTr) await _unitOfWork.RollbackAsync();
                return Result<GroupChatMetadataDto>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Result> IsChatExisits(int chatId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var chat = await _unitOfWork.Repository<Chat>()
                    .GetAsync(x => x.Id == chatId);

                return chat != null
                    ? Result.Success()
                    : Result.Failure("Chat is not exists");

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(IsChatExisits));
                return Result<GroupChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(IsChatExisits));
                return Result.Failure("Failed to retrieve this chat from the database"); ;
            }
        }

        public async Task<bool> IsGroupTitleTakenAsync(string title, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var chat = await _unitOfWork.Repository<Chat>()
                    .GetAsync(x => x.Title == title);

                return chat != null;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(IsGroupTitleTakenAsync));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(IsGroupTitleTakenAsync));
                return false;
            }
        }

        public async Task<bool> IsGroupTitleTakenAsync(string title, int chatId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var chat = await _unitOfWork.Repository<Chat>()
                    .GetAsync(x => x.Title == title && x.Id != chatId);

                return chat != null;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(IsGroupTitleTakenAsync));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(IsGroupTitleTakenAsync));
                return false;
            }
        }

        public async Task<Result<GroupChatMetadataDto>> RenameGroupChatAsync(int chatId,int currentUserId, RenameGroupChatDto dto, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
               
                var entity = await _unitOfWork.Repository<GroupChat>().GetAsync(x => x.Id == chatId, include: x => x.Include(x => x.ChatMembers));

                if(entity == null)
                {
                    _logger.LogWarning("Chat {Id} not found in {Method}", chatId, nameof(RenameGroupChatAsync));
                    return Result<GroupChatMetadataDto>.Failure("Group Chat not found");
                }

                if (!entity.ChatMembers.Select(x => x.UserId).Contains(currentUserId))
                {
                    _logger.LogWarning("Attempted to rename chat {chatId} that user {UserId} is not a member at!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not a member in this chat!, can't be renamed");
                }

                if (entity.ChatMembers.FirstOrDefault(x => x.UserId == currentUserId)!.ChatRole != enChatRole.GroupAdmin)
                {
                    _logger.LogWarning("Attempted to rename chat {chatId} that user {UserId} is not a admin of this caht!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not an admin of this chat!, can't be renamed");
                }

                entity.Title = dto.NewTitle;

                await _unitOfWork.Repository<GroupChat>().UpdateAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                if(!saveReult.Succeeded)
                    return Result<GroupChatMetadataDto>.Failure("Failed to rename chat title");

                var chatDto = (await MapperHelper.BuildChatMetadataDto(entity, currentUserId, _messageStatusService, _mapper)) as GroupChatMetadataDto;
                
                _logger.LogInformation("Chat {Id} renamed successfully", entity.Id);

                return Result<GroupChatMetadataDto>.Success(chatDto);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(RenameGroupChatAsync));
                return Result<GroupChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(RenameGroupChatAsync));
                return Result<GroupChatMetadataDto>.Failure("Failed to rename chat title");
            }
        }

        public async Task<Result<GroupChatMetadataDto>> ResetGroupChatImageAsync(int chatId, int currentUserId, ResetGroupChatImageDto dto, CancellationToken cancellationToken)
        {
            string imageUrl = "";
            try
            {
                var entity = await _unitOfWork.Repository<GroupChat>().GetAsync(x => x.Id == chatId, include: x => x.Include(x => x.ChatMembers));

                if (entity == null)
                {
                    _logger.LogWarning("Chat {Id} not found in {Method}", chatId, nameof(ResetGroupChatImageAsync));
                    return Result<GroupChatMetadataDto>.Failure("Group Chat not found");
                }

                if (!entity.ChatMembers.Select(x => x.UserId).Contains(currentUserId))
                {
                    _logger.LogWarning("Attempted to reset chat {chatId} cover image that user {UserId} is not a member at!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not a member in this chat!, can't be reset its cover image");
                }

                if (entity.ChatMembers.FirstOrDefault(x => x.UserId == currentUserId)!.ChatRole != enChatRole.GroupAdmin)
                {
                    _logger.LogWarning("Attempted to reset chat {chatId} cover image that user {UserId} is not a admin of this caht!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not an admin of this chat!, can't be reset its cover imag");
                }


                imageUrl = entity.GroupCoverImageUrl;

                if (!string.IsNullOrWhiteSpace(entity.GroupCoverImageUrl))
                {
                    var replaceAvatarImageResult = await _FileService.ReplaceAsync(dto.NewImage, imageUrl,enFileCategory.GroupCover,entity.Id);

                    if (!replaceAvatarImageResult.Succeeded)
                    {
                        return Result<GroupChatMetadataDto>.Failure("Failed to reset group chat cover image");
                    }

                    imageUrl = replaceAvatarImageResult.Data!;
                }
                else
                {
                    var saveAvatarImageResult = await _FileService.SaveAsync(dto.NewImage, enFileCategory.GroupCover,entity.Id);
                    
                    if (!saveAvatarImageResult.Succeeded)
                    {
                        return Result<GroupChatMetadataDto>.Failure("Failed to reset group chat cover image");
                    }

                    imageUrl = saveAvatarImageResult.Data!;
                }

                entity.GroupCoverImageUrl = imageUrl;

                await _unitOfWork.Repository<GroupChat>().UpdateAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();
                if (!saveReult.Succeeded)
                    return Result<GroupChatMetadataDto>.Failure("Failed to reset group chat cover image");


                var chatDto = (await MapperHelper.BuildChatMetadataDto(entity, currentUserId, _messageStatusService, _mapper)) as GroupChatMetadataDto;

                _logger.LogInformation("Chat {Id} cover image reset successfully", entity.Id);
                
                return Result<GroupChatMetadataDto>.Success(chatDto);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(ResetGroupChatImageAsync));
                return Result<GroupChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(ResetGroupChatImageAsync));
                return Result<GroupChatMetadataDto>.Failure("Failed to reset group chat cover image"); ;
            }
        }

        public async Task<Result<GroupChatMetadataDto>> DeletetGroupChatImageAsync(int chatId,int currentUserId, CancellationToken cancellationToken)
        {

            string imageUrl = "";
            try 
            { 
                var entity = await _unitOfWork.Repository<GroupChat>().GetAsync(x => x.Id == chatId, include: x => x.Include(x => x.ChatMembers));

                if (entity == null)
                {
                    _logger.LogWarning("Chat {Id} not found in {Method}", chatId, nameof(DeletetGroupChatImageAsync));
                    return Result<GroupChatMetadataDto>.Failure("Group Chat not found");
                }

                if (!entity.ChatMembers.Select(x => x.UserId).Contains(currentUserId))
                {
                    _logger.LogWarning("Attempted to delete chat {chatId} cover image that user {UserId} is not a member at!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not a member in this chat!, can't be delete its cover image");
                }

                if (entity.ChatMembers.FirstOrDefault(x => x.UserId == currentUserId)!.ChatRole != enChatRole.GroupAdmin)
                {
                    _logger.LogWarning("Attempted to delete chat {chatId} cover image that user {UserId} is not a admin of this caht!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not an admin of this chat!, can't be delete its cover imag");
                }

                imageUrl = entity.GroupCoverImageUrl;

                entity.GroupCoverImageUrl = null!; 

                await _unitOfWork.Repository<GroupChat>().UpdateAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                if (!saveReult.Succeeded)
                    return Result<GroupChatMetadataDto>.Failure("Failed to update chat description");


                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    var deleteAvatarImageResult = await _FileService.DeleteAsync(imageUrl);

                    if (!deleteAvatarImageResult.Succeeded)
                    {
                        return Result<GroupChatMetadataDto>.Failure("Failed to delete user's profile image");
                    }
                }

                var chatDto = (await MapperHelper.BuildChatMetadataDto(entity, currentUserId, _messageStatusService, _mapper)) as GroupChatMetadataDto;
                
                _logger.LogInformation("Chat {Id} cover image deleted successfully", entity.Id);

                return Result<GroupChatMetadataDto>.Success(chatDto);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(DeletetGroupChatImageAsync));
                return Result<GroupChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(DeletetGroupChatImageAsync));
                return Result<GroupChatMetadataDto>.Failure("Failed to delete chat File"); ;
            }
}

        public async Task<Result> UpdateGroupLastMessageAsync(int chatId, int currentUserId, MessageDto msgDto, CancellationToken cancellationToken)
        {
             try 
             {
                var entity = await _unitOfWork.Repository<Chat>().GetAsync(x => x.Id == chatId,
                                include: x => x.Include(x => x.ChatMembers));

                if (entity == null)
                {
                    _logger.LogWarning("Chat {Id} not found in {Method}", chatId, nameof(UpdateGroupLastMessageAsync));
                    return Result.Failure("Group Chat not found");
                }

                entity.LastMessage = _mapper.Map<LastMessageSnapshot>(msgDto);

                await _unitOfWork.Repository<Chat>().UpdateAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                if (!saveReult.Succeeded)
                    return Result.Failure("Failed to update chat last message");


                _logger.LogInformation("Chat {Id} last message updated successfully", entity.Id);

                return Result.Success();

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(UpdateGroupLastMessageAsync));
                return Result.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(UpdateGroupLastMessageAsync));
                return Result.Failure("Failed update chat description"); ;
            }
           
        }

        public async Task<Result<GroupChatMetadataDto>> UpdateGroupChatDescriptionAsync(int chatId, int currentUserId, UpdateGroupChatDescriptionDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _unitOfWork.Repository<GroupChat>().GetAsync(x => x.Id == chatId, include: x => x.Include(x => x.ChatMembers));

                if (entity == null)
                {
                    _logger.LogWarning("Chat {Id} not found in {Method}", chatId, nameof(UpdateGroupChatDescriptionAsync));
                    return Result<GroupChatMetadataDto>.Failure("Group Chat not found");
                }

                if (!entity.ChatMembers.Select(x => x.UserId).Contains(currentUserId))
                {
                    _logger.LogWarning("Attempted to update chat {chatId} description that user {UserId} is not a member at!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not a member in this chat!, can't be update its description");
                }

                if (entity.ChatMembers.FirstOrDefault(x => x.UserId == currentUserId)!.ChatRole != enChatRole.GroupAdmin)
                {
                    _logger.LogWarning("Attempted to update chat {chatId} description that user {UserId} is not a admin of this caht!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not an admin of this chat!, can't be update its description");
                }


                entity.Description = dto.NewDescription;

                await _unitOfWork.Repository<GroupChat>().UpdateAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                if (!saveReult.Succeeded)
                    return Result<GroupChatMetadataDto>.Failure("Failed to update chat description");


                var chatDto = (await MapperHelper.BuildChatMetadataDto(entity, currentUserId, _messageStatusService, _mapper)) as GroupChatMetadataDto;

                _logger.LogInformation("Chat {Id} description updated successfully", entity.Id);

                return Result<GroupChatMetadataDto>.Success(chatDto);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(UpdateGroupChatDescriptionAsync));
                return Result<GroupChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(UpdateGroupChatDescriptionAsync));
                return Result<GroupChatMetadataDto>.Failure("Failed update chat description"); ;
            }
        }


        public async Task<Result> DeleteGroupChatAsync(int chatId, int currentUserId, CancellationToken cancellationToken)
        {
            string imageUrl = "";
            try
            {
                var entity = await _unitOfWork.Repository<GroupChat>()
                    .GetAsync(x => x.Id == chatId , include: x => x.Include(x => x.ChatMembers));

                if (entity == null)
                {
                    _logger.LogWarning("Chat {Id} not found in {Method}", chatId, nameof(DeleteGroupChatAsync));
                    return Result<GroupChatMetadataDto>.Failure("Group Chat not found");
                }

                if (!entity.ChatMembers.Select(x => x.UserId).Contains(currentUserId))
                {
                    _logger.LogWarning("Attempted to delete chat {chatId} that user {UserId} is not a member at!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not a member in this chat!, can't be deleted");
                }

                if (entity.ChatMembers.FirstOrDefault(x => x.UserId == currentUserId)!.ChatRole != enChatRole.GroupOwner)
                {
                    _logger.LogWarning("Attempted to delete chat {chatId} that user {UserId} is not the owner of this caht!", chatId, currentUserId);
                    return Result<GroupChatMetadataDto>.Failure("User is not the owner of this chat!, can't be deleted");
                }


                imageUrl = entity.GroupCoverImageUrl;

                await _unitOfWork.Repository<GroupChat>().DeleteAsync(entity);

                var saveReult = await _unitOfWork.SaveChangesAsync();

                if (!saveReult.Succeeded)
                    return Result.Failure("Failed to delete the chat");

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    var deleteAvatarImageResult = await _FileService.DeleteAsync(imageUrl);

                    if (!deleteAvatarImageResult.Succeeded)
                        return Result.Failure("Failed to delete the chat cover image");
                }

                _logger.LogInformation("Chat {Id} deleted successfully", entity.Id);
               
                return Result.Success();

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled in {Method}", nameof(DeleteGroupChatAsync));
                return Result<GroupChatMetadataDto>.Failure("Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method} ", nameof(DeleteGroupChatAsync));
                return Result.Failure("Failed to delete the chat from the database"); ;
            }
        }

    }
}
