using StockControlApi.Application.Models;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using StockControlApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Org.BouncyCastle.Cms;
using StockControlApi.Application.WebSocketManager.Handlers;

namespace StockControlApi.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _db;
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly RoomHandler _ws;
        public ChatService(AppDbContext db, RoomHandler ws, IHostingEnvironment hostingEnvironment, IPushNotificationService pushNotificationService )
        {
            _db = db;
            _ws = ws;
            _webHostEnvironment = hostingEnvironment;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<ResponseModel> GetChatMessagesAsync(string userId, string recipientId, int skip, int take)
        {
            try
            {
                var chatsDB = await _db.Chats.Include(x=>x.Messages.OrderByDescending(x=>x.CreatedAt).Skip(skip).Take(take)).FirstOrDefaultAsync(x => (x.UserId == userId && x.RecipientUserId == recipientId) || (x.UserId == recipientId && x.RecipientUserId == userId));
                

                if (chatsDB == null)
                {

                    UsersChatModel model = new UsersChatModel()
                    {
                        UserId = userId,
                        RecipientUserId = recipientId,
                    };
                    chatsDB = await CreateChat(model);


                   
                }
                if (chatsDB.Messages != null && chatsDB.Messages.Any())
                {
                    chatsDB.Messages.Reverse();
                }
               
                return ResponseModel.BuildOkResponse(chatsDB);
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetChats(string userId, string? search)
        {
            try
            {
                var chat = await _db.Chats.Where(x => x.UserId == userId  ||  x.RecipientUserId == userId).Include(x=> x.RecipientUser).Include(x => x.User).Include(x=>x.Messages).ToListAsync();


                chat = !string.IsNullOrEmpty(search) ? chat.Where(x => x.RecipientUser.Name.Contains(search) || x.User.Name.Contains(search)).ToList() : chat;

                

                return ResponseModel.BuildOkResponse(chat);
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<Chat> CreateChat(UsersChatModel chat)
        {


            Chat messageChat = new Chat()
            {
                UserId = chat.UserId,

                RecipientUserId = chat.RecipientUserId,

            };

            await _db.Chats.AddAsync(messageChat);
            await _db.SaveChangesAsync();
            return messageChat;

        }
        public async Task<ResponseModel> CreateMessage(ChatModel chat)
        {
            try
            {

                Chat? chatDB = await _db.Chats.FirstOrDefaultAsync(x => (x.UserId == chat.UserId && x.RecipientUserId == chat.RecipientUserId) || (x.UserId == chat.RecipientUserId && x.RecipientUserId == chat.UserId));

                if(chatDB == null)
                {
                    chatDB = new Chat()
                    {
                        UserId = chat.UserId,
                        RecipientUserId = chat.RecipientUserId, 
                    };
                    await _db.AddAsync(chatDB);
                    await _db.SaveChangesAsync();
                }

                Message message = new Message()
                {
                 
                    MessageType = chat.MessageType,
                    ChatId = chatDB.Id,
                    UserId = chat.UserId,
                    CreatedAt = DateTime.Now,
                    isVisualized=false

                };

                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == chat.UserId);
                PushNotificationModel pushNotificationModel = new PushNotificationModel()
                {
                    title = user.Name,
                    click_action = "http://localhost:3000/chats",
                    icon = "http://url-to-an-icon/icon.png",

                };
             
                var ids= await _db.PushNotifications.Where(x=>x.UserId== chat.RecipientUserId).Select(x=>x.Token).ToListAsync();

                if (message.MessageType == eMessageType.arquivo)
                {
                    var bytes = Convert.FromBase64String(chat.MessageContent.Split(",")[1]);
                    // var contents = new MemoryStream(bytes);
            

                    string savePath = _webHostEnvironment.WebRootPath + "\\images\\Chats\\";
                    string newNameFile = Guid.NewGuid().ToString() + '.'+ chat.FileExtention;
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    var fullPath= savePath + newNameFile;
                    File.WriteAllBytes(fullPath, bytes);
                  
                    message.FileName = newNameFile;
                   message.FilePath = "https://localhost:44313/images/Chats/" + newNameFile;
                    pushNotificationModel.body = "arquivo";

                }
                else if (message.MessageType==eMessageType.audio)
                {
                    var bytes = Convert.FromBase64String(chat.MessageContent);
                    // var contents = new MemoryStream(bytes);


                    string savePath = _webHostEnvironment.WebRootPath + "\\audio\\Chats\\";
                    string newNameFile = Guid.NewGuid().ToString() + '.' + chat.FileExtention;
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    var fullPath = savePath + newNameFile;
                    File.WriteAllBytes(fullPath, bytes);

                    message.FileName = newNameFile;
                    message.FilePath = "https://localhost:44313/audio/Chats/" + newNameFile;
                    pushNotificationModel.body = "audio";
                }
                else
                {
                    message.MessageContent = chat.MessageContent;
                    pushNotificationModel.body = chat.MessageContent;
                }
                await _db.Messages.AddAsync(message);
                await _db.SaveChangesAsync();

                string wsResponse = System.Text.Json.JsonSerializer.Serialize(new SendMessageSocketModel() { Message = JsonConvert.SerializeObject(new MessageModel(message))  }, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                });

                await _ws.SendMessageToGroup(chatDB.Id.ToString(), wsResponse);
                await _ws.SendMessageToGroup(chatDB.RecipientUserId.ToString(), wsResponse);

                if (ids.Count > 0) {
                    await _pushNotificationService.sendNotification(pushNotificationModel, ids);
                }
               



                return ResponseModel.BuildOkResponse(new { chatId = chatDB.Id });
            }

            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }


        }
        public async Task<ResponseModel> setVisualized(string userId, string recipientId)
        {
            var chatDB = await _db.Chats.FirstOrDefaultAsync(x => (x.UserId == userId && x.RecipientUserId == recipientId) || (x.UserId == recipientId && x.RecipientUserId == userId));
            var messages = await _db.Messages.Where(x => x.ChatId == chatDB.Id).ToListAsync();
           var recipientMessages=  messages.Where(x => x.UserId == recipientId);
            foreach (var message in recipientMessages)
            {
                message.isVisualized= true;
                _db.Update(message);
            }
            
            await _db.SaveChangesAsync();

            return ResponseModel.BuildOkResponse("");
        }


      



    }
}
