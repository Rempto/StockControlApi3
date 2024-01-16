using StockControlApi.Application.Models;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using StockControlApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace StockControlApi.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;
        public NotificationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ResponseModel> GetNotifications(int skip, int take, string userId)
        {
            try
            {
                var notifications = await _db.Notifications.AsNoTracking().Where(x => x.RecipientUserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync();

                var totalPages = Math.Ceiling(notifications.Count() / (decimal)take);
                var result = notifications.Skip(skip).Take(take);

                return ResponseModel.BuildOkResponse(new { notification = result, totalPages = totalPages });
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> CreateNotification(NotificationModel notification)
        {
            try
            {
                

                var recipientUser = await _db.Users.Where(x=>x.Permission==eUserPermission.admin).ToListAsync();
                var creationUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == notification.CreationUserId);
                foreach ( var recipient in recipientUser )
                {
                   
                    Notification nt = new Notification()
                    {
                        UserId = notification.CreationUserId,
                        Type = notification.Type,
                        IsVisualized = false,
                        CreatedAt = DateTime.Now,
                        User = creationUser,


                    };

                    if (notification.Type == eType.saida)
                    {
                        nt.Title = "Saida de produtos";
                        nt.Description = "o usuario " + creationUser.Name + " removeu " + notification.Qtd + "unidades do produto: " + notification.ProductName;
                    }
                    else if (notification.Type == eType.entrada)
                    {
                        nt.Title = "Entrada de produtos";
                        nt.Description = "o usuario " + creationUser.Name + " adicionou " + notification.Qtd + "unidades do produto: " + notification.ProductName;
                    }
                    else if (notification.Type == eType.edicao)
                    {
                        nt.Title = "Edição de produto";
                        nt.Description = "o usuario " + creationUser.Name + " editou o produto: " + notification.ProductName;
                    }
                    else if (notification.Type == eType.registro)
                    {
                        nt.Title = "Registro de produto";
                        nt.Description = "o usuario " + creationUser.Name + " registrou o produto: " + notification.ProductName;
                    }
                    else if (notification.Type == eType.exclusaoProduto)
                    {
                        nt.Title = "Exclusão de produto";
                        nt.Description = "o usuario " + creationUser.Name + " excluiu o produto: " + notification.ProductName;
                    }
                    else if (notification.Type == eType.exclusaoMovimentacao)
                    {
                        nt.Title = "Exclusão de movimentacao";
                        nt.Description = "o usuario " + creationUser.Name + " excluiu a movimentacão com id: " + notification.MoveId;
                    }
                    else if (notification.Type == eType.edicaoMovimentacao)
                    {
                        nt.Title = "Edição de movimentacao";
                        nt.Description = "o usuario " + creationUser.Name + " alterou a movimentacão com id: " + notification.MoveId;
                    }

                   
                    nt.RecipientUser = recipient;
                    nt.RecipientUserId =  recipient.Id;
                    await _db.Notifications.AddAsync(nt);
                    await _db.SaveChangesAsync();

                }

                

                return ResponseModel.BuildOkResponse("");

            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }

           
        }

        public async Task<ResponseModel> GetUserNotifications(string id)
        {
            try
            {
                var prod = await _db.Notifications.AsNoTracking().Where(x => x.RecipientUserId == id).Where(x=>x.IsVisualized==false).OrderByDescending(x => x.CreatedAt).ToListAsync();
                

                return ResponseModel.BuildOkResponse(prod);
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

          public async Task<ResponseModel> setVisualized(Guid id)
         {
            var notification = await _db.Notifications.FirstOrDefaultAsync(x=>x.Id==id);

            notification.IsVisualized = true;
            _db.Update(notification);

            await _db.SaveChangesAsync();

            return ResponseModel.BuildOkResponse("");
          }

        public async Task<ResponseModel> setAllVisualized(string userId)

          
        {
            var notifications = await _db.Notifications.Where(x => x.RecipientUserId == userId).Where(x => x.IsVisualized == false).ToListAsync();
           
            foreach (var notification in notifications)
            {
                notification.IsVisualized = true;
                _db.Update(notification);
            }

            await _db.SaveChangesAsync();

            return ResponseModel.BuildOkResponse("");
        }

    }

}
