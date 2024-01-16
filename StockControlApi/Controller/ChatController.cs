using StockControlApi.Application.Models;
using StockControlApi.Application.Services;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace StockControlApi.Controller
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize("Bearer")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
           _chatService = chatService;
        }
        [HttpGet]
        [Route("messages")]
        public async Task<IActionResult> GetMessageAsync(string userId, string recipientId, int skip, int take)
        {
            return new ResponseHelper().CreateResponse(await _chatService.GetChatMessagesAsync(userId, recipientId,skip,take)) ;
        }
        [HttpGet]
        [Route("chats")]
        public async Task<IActionResult> GetChatAsync(string userId, string? search)
        {
            return new ResponseHelper().CreateResponse(await _chatService.GetChats(userId,search));
        }
       

        [HttpPost]
        [Route("create-message")]
        public async Task<IActionResult> CreateMessage(ChatModel chat)
        {
            return new ResponseHelper().CreateResponse(await _chatService.CreateMessage(chat));
        }
        [HttpPut]
        [Route("set-visualized")]
        public async Task<IActionResult> setVisualized(string userId, string recipientId)
        {
            return new ResponseHelper().CreateResponse(await _chatService.setVisualized(userId,recipientId));
        }

    }
}
