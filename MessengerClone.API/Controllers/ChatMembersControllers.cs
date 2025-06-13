using MessengerClone.Domain.Abstractions;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.Chats.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/chats/{chatId:int}/members")]
    [ApiController]
    public class ChatMembersControllers(IChatMemeberService _chatMemberService, IUserContext _userContext) : ControllerBase
    {

        [HttpGet(Name = "GetMemberInChatAsync")]
        public async Task<IActionResult> GetMemberInChatAsync([FromRoute] int chatId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequestResponse("DATA_INVALID", "Validation error in get chat member .", errors);
            }

            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");
                
                var result = await _chatMemberService.GetMemberInChatAsync(chatId, _userContext.UserId);

                return result.Succeeded
                    ? SuccessResponse(result.Data, "Chat member retrieved successfully.")
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
       
        
        [HttpPost(Name = "AddMemberToChatAsync")]
        public async Task<IActionResult> AddMemberToChatAsync([FromRoute] int chatId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequestResponse("DATA_INVALID", "Validation error in Add chat member .", errors);
            }

            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first."); 
                
                var result = await _chatMemberService.AddMemberToChatAsync(new AddChatMemberDto() { ChatId = chatId, UserId = _userContext.UserId });

                return result.Succeeded
                    ? CreatedResponse("GetMemberInChatAsync", new { chatId = chatId }, result.Data!, $"Chat member created successfully.")
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


        [HttpPatch(Name = "ChangeMemberChatRoleAsync")]
        public async Task<IActionResult> ChangeMemberChatRoleAsync([FromRoute] int chatId, AddMemberChatRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequestResponse("DATA_INVALID", "Validation error in Add chat member .", errors);
            }

            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");
                
                var result = await _chatMemberService.ChangeMemberChatRoleAsync(chatId, _userContext.UserId, dto);

                return result.Succeeded
                    ? SuccessResponse(result.Data, "Chat member role updated successfully.")
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


        [HttpDelete(Name = "RemoveMemberFromChatAsync")]
        public async Task<IActionResult> RemoveMemberFromChatAsync([FromRoute] int chatId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequestResponse("DATA_INVALID", "Validation error in Remove chat member .", errors);
            }

            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first."); 
                
                var result = await _chatMemberService.RemoveMemberFromChatAsync(chatId, _userContext.UserId);

                return result.Succeeded
                       ? SuccessResponse("Chat member removed successfully.")
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
