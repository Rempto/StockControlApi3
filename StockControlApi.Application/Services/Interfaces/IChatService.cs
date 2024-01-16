using StockControlApi.Application.Models;

namespace StockControlApi.Application.Services.Interfaces
{
    public interface IChatService
    {
         Task<ResponseModel> GetChatMessagesAsync(string userId, string recipientId, int skip, int take);
        
        Task<ResponseModel> CreateMessage(ChatModel chat);
        Task<ResponseModel> GetChats(string userId, string? search);

        Task<ResponseModel> setVisualized(string userId, string recipientId);
    }
}
