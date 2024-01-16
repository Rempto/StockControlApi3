using Microsoft.AspNetCore.Http;
using StockControlApi.Application.WebSocketManager.Handlers;
using SystemWS = System.Net.WebSockets;

namespace StockControlApi.Application.WebSocketManager
{
    public class ManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private RoomHandler _Handler { get; set; }

        public ManagerMiddleware(RequestDelegate next, RoomHandler Handler)
        {
            _next = next;
            _Handler = Handler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();

            var userId = context.Request.Query["userId"].ToString();

            if (!String.IsNullOrEmpty(context.Request.Query["chatId"]))
            {
                var chatId = context.Request.Query["chatId"].ToString();
                await _Handler.OnConnectedGroup(socket, userId + chatId, chatId);
            }
            else
            {
                await _Handler.OnConnected(socket, userId);
            }

            await Receive(socket, async (result, byteCount, buffer) =>
            {
                if (result.MessageType == SystemWS.WebSocketMessageType.Text)
                {
                    await _Handler.ReceiveAsync(socket, result, byteCount, buffer);
                }

                else if (result.MessageType == SystemWS.WebSocketMessageType.Close)
                {
                    await _Handler.OnDisconnected(socket);
                }
            });

        }


        private static async Task Receive(SystemWS.WebSocket socket, Action<SystemWS.WebSocketReceiveResult, int, byte[]> handleMessage)
        {
            int bufferSize = 1024 * 4;

            while (socket.State == SystemWS.WebSocketState.Open)
            {
                byte[]? messagePayload = null;
                int byteCount = 0;

                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[bufferSize]);

                var allBytes = new List<byte>();
                SystemWS.WebSocketReceiveResult? result = null;

                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    allBytes.AddRange(buffer.Array!.Take(result.Count));
                    byteCount += result.Count;
                }
                while (!result.EndOfMessage);

                allBytes.Add(0x00);
                messagePayload = allBytes.ToArray();

                handleMessage(result, byteCount, messagePayload);

            }

        }
    }
}
