using StockControlApi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Services.Interfaces
{
    public interface IPushNotificationService
    {
        Task<ResponseModel> CreatePushNotification(string token, string userId);
        Task sendNotification(PushNotificationModel model, List<string> ids);
    }
}
