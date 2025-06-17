using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.MessageReactions.DTOs;
using MessengerClone.Service.Features.MessageReactions.Interfaces;
using MessengerClone.Service.Features.MessageStatuses.DTOs;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/chats/{chatId:int}/messages/{messageId:int}/reaction")]
    [ApiController]
    public class MessageReactionsController(IMessageReactionService _messageReactionService, IUserContext _userContext) : ControllerBase
    {

        [HttpGet(Name = "GetAllMessageReactions")]
        public async Task<IActionResult> GetAllMessageReactionsAsync([FromRoute] int chatId, [FromRoute] int messageId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageReactionService.GetAllMessageReactionsAsync(chatId, messageId, _userContext.UserId);

                return result.Succeeded
                    ? CreatedResponse(null!, result.Data!, "Message reaction added successfully.")
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


        [HttpPost(Name = "AddReactToMessageAsync")]
        public async Task<IActionResult> AddReactToMessageAsync([FromRoute] int chatId, [FromRoute] int messageId, [FromBody] AddMessageReactionDto dto)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageReactionService.AddReactToMessageAsync(chatId, messageId, _userContext.UserId, dto);

                return result.Succeeded
                    ? CreatedResponse(null!, result.Data!, "Message reaction added successfully.")
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


        [HttpDelete(Name = "RemoveReactionToMessageAsync")]
        public async Task<IActionResult> RemoveReactionToMessageAsync([FromRoute] int chatId,[FromRoute] int messageId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageReactionService.RemoveReactionToMessageAsync(chatId, messageId, _userContext.UserId);

                return result.Succeeded
                       ? SuccessResponse("Message reaction removed successfully.")
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
