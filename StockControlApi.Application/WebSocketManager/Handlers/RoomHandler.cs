
using Newtonsoft.Json;
using StockControlApi.Application.Models;
using System.Text;
using SystemWS = System.Net.WebSockets;

namespace StockControlApi.Application.WebSocketManager.Handlers
{
    public class RoomHandler : WebSocketHandler
    {
        public RoomHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(SystemWS.WebSocket socket, string userId)
        {
            await base.OnConnected(socket, userId);
        }

        public override async Task ReceiveAsync(SystemWS.WebSocket socket, SystemWS.WebSocketReceiveResult result, int byteCount, byte[] buffer)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            var msgObj = JsonConvert.DeserializeObject<SocketMessage>(message);
            if (msgObj != null)
            {
                await SendMessageAsync(msgObj.ReceiveId, msgObj.Message);
            }
        }
    }
}
