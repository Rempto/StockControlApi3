using StockControlApi.Application.Models;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using StockControlApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using StockControlApi.Application.WebSocketManager.Handlers;

namespace StockControlApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;
        private readonly RoomHandler _ws;
        public ProductService(AppDbContext db, INotificationService notificationService,RoomHandler ws)
        {
            _db = db;
            _notificationService = notificationService;
            _ws = ws;
        }

        public async Task<ResponseModel> GetAsync()
        {
            try
            {
                var prod = await _db.Products.ToListAsync();

                return ResponseModel.BuildOkResponse(prod);
            }
            catch(Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> CreateProduct(ProductModel product)
        {
            try
            {

                Product pct = new Product()
                {
                    Name = product.Name,
                    Price = product.Price,
                    StockAmount = product.StockAmount,
                    PriceSale = product.PriceSale,
                    UserId = product.UserId
                };
                var productDB = await _db.Products.FirstOrDefaultAsync(x => x.Name == product.Name);
                //await _userManager.FindByEmailAsync(client.Email);

                if (productDB != null)
                {
                    return ResponseModel.BuildConflictResponse("Produto ja cadastrado");
                }
                else
                {
                    await _db.Products.AddAsync(pct);
                    await _db.SaveChangesAsync();
                    NotificationModel notification = new NotificationModel()
                    {
                        CreationUserId = product.UserId.ToString(),
                        ProductName = product.Name,
                        Type = eType.registro,
                        
                    };


                    await _notificationService.CreateNotification(notification);
                    string wsResponse = System.Text.Json.JsonSerializer.Serialize(new SendMessageSocketModel() { Message = "aa" }, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    });

                    await _ws.SendMessageToGroup("adminNotifications", wsResponse);

                    return ResponseModel.BuildOkResponse("Produto registrado com sucesso!");
                }

            }
            catch(Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> DeleteProduct(Guid Id, string userId)
        {
            try
            {

                var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);
                NotificationModel notification = new NotificationModel()
                {
                    CreationUserId = userId,
                    ProductName = product.Name,
                    Type = eType.exclusaoProduto,

                };


                await _notificationService.CreateNotification(notification);
                string wsResponse = System.Text.Json.JsonSerializer.Serialize(new SendMessageSocketModel() { Message = "aa" }, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                });

                await _ws.SendMessageToGroup("adminNotifications", wsResponse);
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();


                return ResponseModel.BuildOkResponse("Produto deletado com sucesso!");

            }
            catch(Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> UpdateProduct(ProductModel product)
        {
            try
            {
             
                var prod = await _db.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
                prod.Name = product.Name;
                prod.Price = product.Price;
                prod.PriceSale = product.PriceSale;
                prod.StockAmount = product.StockAmount;
                prod.UserId = product.UserId;
                await _db.SaveChangesAsync();
                NotificationModel notification = new NotificationModel()
                {
                    CreationUserId = product.UserId.ToString(),
                    ProductName = product.Name,
                    Type = eType.edicao,

                };


                await _notificationService.CreateNotification(notification);
                string wsResponse = System.Text.Json.JsonSerializer.Serialize(new SendMessageSocketModel() { Message = "aa" }, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                });

                await _ws.SendMessageToGroup("adminNotifications", wsResponse);

                return ResponseModel.BuildOkResponse("Produto atualizado com sucesso!");
            }
            catch(Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> GetProductById(Guid Id)
        {
            try
            {
                var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);
                return ResponseModel.BuildOkResponse(product);
            }
            catch(Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> GetProductByName(string name)
        {
            try
            {
                var prod = await _db.Products.FirstOrDefaultAsync(x => x.Name == name);

                return ResponseModel.BuildOkResponse(prod);
            }
            catch (Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> GetFilterAndPaginatedProducts(int skip, int take, string? search)
        {
            try
            {
                var prod = await _db.Products.AsNoTracking().ToListAsync();
                prod = !string.IsNullOrEmpty(search) ? prod.Where(x => x.Name.Contains(search)).ToList() : prod;

                var totalPages = Math.Ceiling(prod.Count() / (decimal)take);
                var result = prod.Skip(skip).Take(take);

                return ResponseModel.BuildOkResponse(new { products = result, totalPages = totalPages });
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

       

    }
}
