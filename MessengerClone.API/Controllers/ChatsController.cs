using FluentValidation;
using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.Chats.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatsController(IChatService _chatService, IUserContext _userContext) : ControllerBase
    {

        [HttpGet("sidebar",Name = "GetAllChatsForSidebarByUserIdAsync")]
        public async Task<IActionResult> GetAllChatsForSidebarByUserIdAsync([FromQuery] int? page, [FromQuery] int? size, CancellationToken cancellationToken, [FromQuery] string? name = null, [FromQuery] string? search = null)
        {
            if (page.HasValue && page <= 0)
                return BadRequestResponse("INVALID_PAGE_ID", "Page number is not valid.", "Page number Id should be positive number");

            if (size.HasValue && size <= 0)
                return BadRequestResponse("INVALID_SIZE_ID", "Page size is not valid.", "Page size Id should be positive number");

            try
            {
                if ( _userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");


                var result = await _chatService.GetAllForSidebarByUserIdAsync(_userContext.UserId, cancellationToken, page, size, search,
                                                            name is not null
                                                            ? (x => x.Type == enChatType.Direct
                                                                ? x.ChatMembers.FirstOrDefault(x => x.UserId == _userContext.UserId)!.User.UserName!.Contains(name)
                                                                : x.Title!.Contains(name)) 
                                                            : null);

                return result.Succeeded
                       ? SuccessResponse(result.Data, "Chats retrieved successfully.")
                       : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpGet("ids" , Name = "GetAllChatslIdsByUserId")]
        public async Task<IActionResult> GetAllChatslIdsByUserId([FromQuery] int? page, [FromQuery] int? size, CancellationToken cancellationToken)
        {
            if (page.HasValue &&  page <= 0)
                return BadRequestResponse("INVALID_PAGE_ID", "Page number is not valid.", "Page number Id should be positive number");

            if (size.HasValue && size <= 0)
                return BadRequestResponse("INVALID_SIZE_ID", "Page size is not valid.", "Page size Id should be positive number");

            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _chatService.GetUserAllChatIdsAsync(_userContext.UserId, cancellationToken, page,size);

                return result.Succeeded
                   ? SuccessResponse(result.Data, "Chats Ids retrieved successfully.")
                   : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpGet("{id:int}", Name = "GetChatMetadataById")]
        public async Task<IActionResult> GetChatMetadataById(int Id, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_CHAT_ID", "Chat id not valid.", "Chat Id should be positive number");
            
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _chatService.GetChatMetadataById(Id,_userContext.UserId, cancellationToken);

                if(!result.Succeeded)
                    return StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());

                return result.Data != null
                    ? SuccessResponse(result.Data, $"{result.Data?.Type.ToString()} Chat retrieved successfully.")
                    : NotFoundResponse("CHAT_NOT_FOUND", "Chat not found.", $"Chat with Id: {Id} was not found.");
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPost(Name = "AddDirectChatAsync")]
        public async Task<IActionResult> AddDirectChatAsync([FromForm] AddDirectChatDto dto, CancellationToken cancellationToken) // From form for Group and could accept json in body for direct
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequestResponse("DATA_INVALID", "Validation error in Add Direct Chat Dto.", errors);
            }

            try
            {
                var result = await _chatService.AddDirectChatAsync(dto, _userContext.UserId, cancellationToken);

                return result.Succeeded
                    ? CreatedResponse("GetChatMetadataById", new { id = result.Data?.Id }, result.Data, "Direct Chat created successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "CREATION_ERROR", result.ToString());

            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPost("group", Name = "AddGroupChatAsync")]
        public async Task<IActionResult> AddGroupChatAsync([FromForm] AddGroupChatDto dto , CancellationToken cancellationToken) // From form for Group and could accept json in body for direct
        {
            try
            {
                var result = await _chatService.AddGroupChatAsync(dto, _userContext.UserId, cancellationToken);

                return result.Succeeded
                    ? CreatedResponse("GetChatMetadataById", new { id = result.Data?.Id }, result.Data, "Group Chat created successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "CREATION_ERROR", result.ToString());

            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPatch("{id:int}/rename", Name = "RenameGroupChatAsync")]
        public async Task<IActionResult> RenameGroupChatAsync([FromRoute]int Id, [FromBody] RenameGroupChatDto dto, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_CHAT_ID", "Chat id not valid.", "Chat Id should be positive number");
            
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _chatService.RenameGroupChatAsync(Id, _userContext.UserId, dto, cancellationToken);
                
                return result.Succeeded
                      ? SuccessResponse(result.Data, $"Group Chat title renamed successfully.")
                      : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());

            }
            catch (HttpRequestException ex)
            {
                // Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPatch("{id:int}/description", Name = "UpdateGroupChatDescriptionAsync")]
        public async Task<IActionResult> UpdateGroupChatDescriptionAsync([FromRoute] int Id, [FromBody] UpdateGroupChatDescriptionDto dto, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_CHAT_ID", "Chat id not valid.", "Chat Id should be positive number");
            
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _chatService.UpdateGroupChatDescriptionAsync(Id, _userContext.UserId, dto, cancellationToken);

                return result.Succeeded
                      ? SuccessResponse(result.Data, $"Group Chat description updated successfully.")
                   : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());

            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPatch("{id:int}/image", Name = "ResetGroupChatImageAsync")]
        public async Task<IActionResult> ResetGroupChatImageAsync([FromRoute] int Id, [FromForm] ResetGroupChatImageDto dto, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_CHAT_ID", "Chat id not valid.", "Chat Id should be positive number");
            
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _chatService.ResetGroupChatImageAsync(Id, _userContext.UserId, dto, cancellationToken);

                return result.Succeeded
                      ? SuccessResponse(result.Data, $"Group Chat image reset successfully.")
                   : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());

            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }

     
        [HttpDelete("{id:int}/image", Name = "DeletetGroupChatImageAsync")]
        public async Task<IActionResult> DeletetGroupChatImageAsync([FromRoute] int Id, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_CHAT_ID", "Chat id not valid.", "Chat Id should be positive number");
            
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _chatService.DeletetGroupChatImageAsync(Id, _userContext.UserId, cancellationToken);

                return result.Succeeded
                      ? SuccessResponse(result.Data, $"Group Chat image deleted successfully.")
                   : StatusCodeResponse(StatusCodes.Status500InternalServerError, "DELETION_ERROR", result.ToString());

            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpDelete("{id:int}/group", Name = "DeleteGroupChat")]
        public async Task<IActionResult> DeleteGroupChatAsync([FromRoute] int Id, CancellationToken cancellationToken)
        {
            if (Id <= 0)
                return BadRequestResponse("INVALID_CHAT_ID", "Chat id not valid.", "Chat Id should be positive number");
            
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _chatService.DeleteGroupChatAsync(Id, _userContext.UserId, cancellationToken);

                return result.Succeeded
                   ? SuccessResponse("Chat deleted successfully.")
                   : StatusCodeResponse(StatusCodes.Status500InternalServerError, "DELETION_ERROR", result.ToString());

            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }

    }
}
