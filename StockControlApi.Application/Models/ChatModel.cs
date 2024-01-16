using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Models
{
    public class ChatModel
    {

        public string MessageContent { get; set; } = string.Empty;
        public eMessageType MessageType { get; set; }

        public Guid? chatId { get; set; }
        public string UserId { get; set; } = string.Empty;
        [NotMapped]
        public string RecipientUserId { get; set; } = string.Empty;
       
        public string? FileExtention { get; set; }

    }
    public class ChatMessagesModel
    {
        
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public User? User { get; set; }
        public string RecipientUserId { get; set; } = string.Empty;

        public User? RecipientUser { get; set; }
        public List<MessageModel>? Messages { get; set; }

    }
    public class MessageModel
    {
        public MessageModel(Message message)
        {
            this.id=message.Id;
            this.chatId=message.ChatId;
            this.messageContent=message.MessageContent;
            this.messageType=message.MessageType;
            this.createdAt=message.CreatedAt;
            this.userId=message.UserId;
            this.filePath=message.FilePath;

            
        }
        public Guid id { get; set; }
        public string? messageContent { get; set; } = string.Empty;
        public eMessageType messageType { get; set; }
         
        public string? filePath { get; set; }
        public string userId { get; set; }
        public Guid chatId { get; set; }
        public DateTime createdAt { get; set; }
        public string messageTypeName
        {
            get
            {
                switch (messageType)
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
        public bool isImage
        {
            get
            {
                if (string.IsNullOrEmpty(filePath)) return false;

                switch (filePath.Split(".")[filePath.Split(".").Length - 1])
                {
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
            }
        }


    }
}
    