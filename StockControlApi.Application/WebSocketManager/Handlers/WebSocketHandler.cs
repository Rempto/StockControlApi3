using System.Text;
using SystemWS = System.Net.WebSockets;

namespace StockControlApi.Application.WebSocketManager.Handlers
{
    public abstract class WebSocketHandler
    {
        protected ConnectionManager ConnectionManager { get; set; }

        public WebSocketHandler(ConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public virtual async Task OnConnected(SystemWS.WebSocket socket, string userId)
        {
            await Task.Run(() => ConnectionManager.AddSocket(socket, userId));
        }

        public virtual async Task OnConnectedGroup(SystemWS.WebSocket socket, string socketId, string chatId)
        {
            await Task.Run(() => ConnectionManager.AddSocket(socket, socketId));
            await Task.Run(() => ConnectionManager.AddToGroup(socket, chatId));
        }

        public virtual async Task OnDisconnected(SystemWS.WebSocket socket)
        {
            await ConnectionManager.RemoveSocket(ConnectionManager.GetId(socket));
        }

        public static async Task SendMessageAsync(SystemWS.WebSocket socket, string message)
        {
            if (socket.State != SystemWS.WebSocketState.Open)
                return;

            await socket.SendAsync(
                buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message), offset: 0, count: message.Length),
                messageType: SystemWS.WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None)
            ;
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            var socket = ConnectionManager.GetSocketById(socketId);

            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socketId));
            }

            await SendMessageAsync(socket, message);
        }

        public async Task SendMessageToGroup(string groupId, string message)
        {
            List<SystemWS.WebSocket> sockets = ConnectionManager.GetSocketGroup(groupId);
            foreach (var socket in sockets)
            {
                if (socket.State != SystemWS.WebSocketState.Closed)
                {
                    await SendMessageAsync(socket, message);
                }
            }
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in ConnectionManager.GetAll())
            {
                if (pair.Value.State == SystemWS.WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public abstract Task ReceiveAsync(SystemWS.WebSocket socket, SystemWS.WebSocketReceiveResult result, int byteCount, byte[] buffer);
    }
}
