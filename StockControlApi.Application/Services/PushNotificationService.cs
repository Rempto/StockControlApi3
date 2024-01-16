using StockControlApi.Application.Models;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using StockControlApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly AppDbContext _db;
        public PushNotificationService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<ResponseModel> CreatePushNotification(string token, string userId)
        {
            try
            {
                var NotificationUserToken = await _db.PushNotifications.FirstOrDefaultAsync(x => x.Token == token);

                UserPushNotifications userPushNotifications = new UserPushNotifications()
                {
                    CreatedAt = DateTime.UtcNow,
                };

                if (NotificationUserToken != null)
                {
                    if (NotificationUserToken.UserId != userId) {

                    _db.Remove(NotificationUserToken);
                    }
                    else { return ResponseModel.BuildOkResponse(""); }
                   
                   

                }
                
                    userPushNotifications.Token = token;
                    userPushNotifications.UserId = userId;


                    await _db.PushNotifications.AddAsync(userPushNotifications);
                    await _db.SaveChangesAsync();





                    return ResponseModel.BuildOkResponse("");
            
                
               

            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task sendNotification(PushNotificationModel model, List<string> ids)
        {
            HttpClient httpClient = new HttpClient();

            string apiUrl = "https://fcm.googleapis.com/fcm/send";
            var body = new
            {
                notification = model,
                registration_ids = ids
            };
            string bodyJson = JsonConvert.SerializeObject(body);
            string firebaseKeyServer = "AAAAj45-ty4:APA91bGBd7JLhU0IVQwGT0b9pFlI58qV-03CW2jyQtjnMaf3rzEsLo3jMjm-nWd4uD4JykYBBCcB2wVGfG8fahISOpamFMsMQ1CHQRy8CgWWsoQqHxeeOuFutK8OHW_vuOpSCkjKQ0Pq";
            HttpContent httpContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={firebaseKeyServer}");
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, httpContent);

         var returnModel = await response.Content.ReadFromJsonAsync<PushNotificationReturnModel>();
         

            var notificationIds = _db.PushNotifications.Where(x => ids.Contains(  x.Token)).ToList();

            int pos = 0;
            foreach (var item in returnModel.results)
            {
                if (!String.IsNullOrEmpty(item.error))
                {
                    var not = notificationIds.Where(x => x.Token == ids[pos]).FirstOrDefault();
                    _db.Remove(not);
                }
                pos++;
            }

            _db.SaveChanges();
        }
        }
        
}
