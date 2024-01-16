using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string? MessageContent { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public string? FilePath { get; set; } = string.Empty;
        public eMessageType MessageType { get; set; }

        public bool isVisualized { get; set; }
        public Guid ChatId { get; set; }
        public Chat chat { get; set; }
        public string UserId { get; set; } = string.Empty;
        
        public User? User { get; set; }
      

        public DateTime CreatedAt { get; set; }
        public string MessageTypeName
        {
            get
            {
                switch (MessageType)
                {
                    case eMessageType.arquivo:
                        return "arquivo";
                    case eMessageType.texto:
                        return "texto";
                    case eMessageType.audio:
                        return "audio";
                    default:
                        return "none";
                }
            }
        }

        public bool IsImage { get
            {
                if (string.IsNullOrEmpty(FilePath)) return false;

                switch (FilePath.Split(".")[FilePath.Split(".").Length -1]){
                    case "png":
                        return true;
                    case "jpeg":
                        return true;
                    case "jpg":
                        return true;
                    case "webp":
                        return true;
                    case "svg":
                        return true;
                    default: return false;


                }
            } }

    }
}
