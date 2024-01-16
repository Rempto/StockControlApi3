using System.Collections.Concurrent;
using SystemWS = System.Net.WebSockets;


namespace StockControlApi.Application.WebSocketManager
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<string, SystemWS.WebSocket> _sockets = new ConcurrentDictionary<string, SystemWS.WebSocket>();

        private readonly ConcurrentDictionary<string, List<SystemWS.WebSocket>> _groups = new ConcurrentDictionary<string, List<SystemWS.WebSocket>>();

        public SystemWS.WebSocket GetSocketById(string id)
        {
            var socket = _sockets.FirstOrDefault(p => p.Key == id);
            return socket.Value;
        }

        public List<SystemWS.WebSocket> GetSocketGroup(string groupId)
        {
            if (_groups.ContainsKey(groupId))
            {
                return _groups[groupId];
            }
            else
            {
                return new List<SystemWS.WebSocket>();
            }
        }

        public ConcurrentDictionary<string, SystemWS.WebSocket> GetAll()
        {
            return _sockets;
        }

        public string GetId(SystemWS.WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }
        public void AddSocket(SystemWS.WebSocket socket, string socketId)
        {
            _sockets.TryAdd(socketId, socket);
        }

        public void AddToGroup(SystemWS.WebSocket socket, string boardId)
        {
            if (!_groups.ContainsKey(boardId))
            {
                _groups.TryAdd(boardId, new List<SystemWS.WebSocket>());
            }

            var group = _groups[boardId];
            group.Add(socket);
            _groups[boardId] = group;
        }

        public async Task RemoveSocket(string id)
        {
            SystemWS.WebSocket socket;
            if (id != null)
            {
                _sockets.TryRemove(id, out socket!);

                if (socket != null)
                {
                    await socket.CloseAsync(closeStatus: SystemWS.WebSocketCloseStatus.NormalClosure,
                                            statusDescription: "Closed by the WebSocketManager",
                                            cancellationToken: CancellationToken.None);
                }
            }
        }
    }
}
