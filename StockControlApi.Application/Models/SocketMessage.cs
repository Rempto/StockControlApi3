using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Models
{
    public class SocketMessage
    {
        public string ReceiveId { get; set; }
        public string Message { get; set; }

        public SocketMessage(string message, string receiveId)
        {
            Message = message;
            ReceiveId = receiveId;
        }

    }
    public class SendMessageSocketModel
    {
        public string? Message { get; set; }
        public Guid? MessageId { get; set; }
        public Guid? GroupMessageId { get; set; }
    }
}
