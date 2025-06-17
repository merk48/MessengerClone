using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Service.Features.ChatMembers.DTOs;
using MessengerClone.Service.Features.Chats.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/chats/{chatId:int}/members")]
    [ApiController]
    public class ChatMembersControllers(IChatMemeberService _chatMemberService, IUserContext _userContext) : ControllerBase
    {

        [HttpGet("me",Name = "GetMemberProfileInChat")]
        public async Task<IActionResult> GetMemberProfileInChatAsync([FromRoute] int chatId)
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
                
                var result = await _chatMemberService.GetMemberInChatAsync( _userContext.UserId, chatId);

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


        [HttpGet("{id:int}",Name = "GetMemberInChat")]
        public async Task<IActionResult> GetMemberInChatByIdAsync([FromRoute] int chatId, [FromRoute] int Id)
        {
            try
            {
                var result = await _chatMemberService.GetMemberInChatAsync(Id,chatId);

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


        [HttpGet("{id:int}/check", Name = "IsUserMemberInChat")]
        public async Task<IActionResult> IsUserMemberInChatAsync([FromRoute] int chatId, [FromRoute] int Id)
        {
            try
            {
                var result = await _chatMemberService.IsUserMemberInChatAsync(chatId, Id);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"User membership: User is{(result.Data ? "" : " not")} a member in this chat" )
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


        [HttpGet(Name = "GetAllChatMembers")]
        public async Task<IActionResult> GetAllChatMembersAsync([FromRoute] int chatId, CancellationToken cancellationToken, [FromQuery] int? page = null, [FromQuery] int? size = null, [FromQuery] string? search = null)
        {
            try
            {
                var result = await _chatMemberService.GetAllChatMembersAsync(chatId, cancellationToken, page, size, search);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Chat members retrieved successfuly")
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


        [HttpPost("me", Name = "JoinToChat")]
        public async Task<IActionResult> JoinToChatAsync([FromRoute] int chatId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first."); 
                
                var result = await _chatMemberService.AddMemberToChatAsync(new AddChatMemberDto() {UserId = _userContext.UserId ,ChatRole = enChatRole.GroupMember}, chatId);

                return result.Succeeded
                    ? CreatedResponse("GetMemberInChat", new { chatId = chatId , Id = _userContext.UserId}, result.Data!, $"Chat member created successfully.")
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


        [HttpPost(Name = "AddMemberToChat")]
        public async Task<IActionResult> AddMemberToChatAsync([FromRoute] int chatId, [FromBody] AddChatMemberDto dto)
        {
            try
            {
                var result = await _chatMemberService.AddMemberToChatAsync(dto, chatId);

                return result.Succeeded
                    ? CreatedResponse("GetMemberInChat", new { chatId = chatId, Id = dto.UserId }, result.Data!, $"Chat member created successfully.")
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


        [HttpPost("batch", Name = "AddRangeOfMembersToChat")]
        public async Task<IActionResult> AddRangeOfMembersToChatAsync([FromRoute] int chatId, [FromBody] IEnumerable<AddChatMemberDto> dtos)
        {
            try
            {
                var result = await _chatMemberService.AddRangeOfMembersToChatAsync(dtos, chatId);

                return result.Succeeded
                    ? CreatedResponse("GetMemberInChat", new { chatId = chatId, Id = dtos.First().UserId }, result.Data, "Chat members created successfully.")
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


        [HttpPatch("{id:int}/role-change",Name = "ChangeMemberChatRoleAsync")]
        public async Task<IActionResult> ChangeMemberChatRoleAsync([FromRoute] int chatId,[FromRoute] int Id, AddMemberChatRoleDto dto)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");
                
                var result = await _chatMemberService.ChangeMemberChatRoleAsync(Id,chatId, dto);

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


        [HttpDelete("me", Name = "LeaveFromChat")]
        public async Task<IActionResult> LeaveFromChatAsync([FromRoute] int chatId)
        {

            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first."); 
                
                var result = await _chatMemberService.RemoveMemberFromChatAsync(_userContext.UserId, chatId);

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


        [HttpDelete("{id:int}", Name = "RemoveMemberFromChat")]
        public async Task<IActionResult> RemoveMemberFromChatAsync([FromRoute] int chatId, [FromRoute] int Id)
        {

            try
            {
                var result = await _chatMemberService.RemoveMemberFromChatAsync(chatId, Id);

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
