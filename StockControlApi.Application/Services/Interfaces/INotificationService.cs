using StockControlApi.Application.Models;
using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<ResponseModel> GetNotifications(int skip, int take, string userId);
        Task<ResponseModel> CreateNotification(NotificationModel notification);
        Task<ResponseModel> GetUserNotifications(string id);
        Task<ResponseModel> setVisualized(Guid id);
        Task<ResponseModel> setAllVisualized(string userId);
    }
}
