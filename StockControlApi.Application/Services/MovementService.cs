using StockControlApi.Application.Models;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using StockControlApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Buffers.Text;
using NPOI.Util;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using StockControlApi.Application.WebSocketManager.Handlers;

namespace StockControlApi.Application.Services
{
    public class MovementService : IMovementService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;
        private readonly RoomHandler _ws;
        public MovementService(AppDbContext db, INotificationService notificationService, RoomHandler ws)
        {
            _db = db;
            _notificationService = notificationService;
            _ws = ws;

        }

        public async Task<ResponseModel> GetAsync()
        {
            try
            {
                var move = await _db.Movemments.ToListAsync();

                return ResponseModel.BuildOkResponse(move);
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }

        }

        public async Task<ResponseModel> CreateMovement(MovementModel movement)
        {
            try
            {

                Movement mvt = new Movement()
                {
                    ProductName = movement.ProductName,
                    PriceUnit = movement.PriceUnit,
                    PriceTotal = movement.PriceTotal,
                    Qtd = movement.Qtd,
                    Move = movement.Move,
                    Date = movement.Date,
                    ProductId = movement.ProductId,
                    UserId = movement.UserId
                };
                var Prod = await _db.Products.FirstOrDefaultAsync(x => x.Id==mvt.ProductId);

                if (mvt.Move == eMove.saida)
                {
                    Prod.StockAmount = Prod.StockAmount - mvt.Qtd;
                }
                else
                {
                    Prod.StockAmount = Prod.StockAmount + mvt.Qtd;
                }
                
                await _db.Movemments.AddAsync(mvt);
                await _db.SaveChangesAsync();

                NotificationModel notification = new NotificationModel()
                {
                    CreationUserId = movement.UserId.ToString(),
                    ProductName = movement.ProductName,
                    Qtd = movement.Qtd,
                    Type = movement.Move == eMove.saida ? eType.saida : eType.entrada
                };
        

                await _notificationService.CreateNotification(notification);
                string wsResponse = System.Text.Json.JsonSerializer.Serialize(new SendMessageSocketModel() { Message = "aa"}, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                });

                await _ws.SendMessageToGroup("adminNotifications", wsResponse);
                return ResponseModel.BuildOkResponse("movimento registrado com sucesso!");
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> DeleteMovement(Guid Id, string userId)
        {
            try
            {
                var Movement = await _db.Movemments.FirstOrDefaultAsync(x => x.Id == Id);

                var Prod = await _db.Products.FirstOrDefaultAsync(x => x.Name == Movement.ProductName);

                if (Movement.Move == eMove.saida)
                {
                    Prod.StockAmount = Prod.StockAmount + Movement.Qtd;
                }
                else
                {
                    Prod.StockAmount = Prod.StockAmount - Movement.Qtd;
                }
                NotificationModel notification = new NotificationModel()
                {
                    CreationUserId = userId,
                    ProductName = Movement.ProductName,
                    Qtd = Movement.Qtd,
                    Type  = eType.exclusaoMovimentacao,
                    MoveId = Movement.Id
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


                _db.Movemments.Remove(Movement);
                await _db.SaveChangesAsync();

                return ResponseModel.BuildOkResponse("Movimento deletado com sucesso!");
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetMovementById(Guid Id)
        {
            try
            {
                var Movement = await _db.Movemments.FirstOrDefaultAsync(x => x.Id == Id);
                return ResponseModel.BuildOkResponse(Movement);
            }
            catch (Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> UpdateMovement(ReturnMovementModel movement)
        {
            try
            {

                var move = await _db.Movemments.FirstOrDefaultAsync(x => x.Id == movement.Id);
                var Prod = await _db.Products.FirstOrDefaultAsync(x => x.Name == move.ProductName);

                if (move.Move != movement.Move)
                {
                    if (movement.Move == eMove.saida)
                    {
                        Prod.StockAmount = (Prod.StockAmount - move.Qtd) - movement.Qtd;
                    }
                    else
                    {
                        Prod.StockAmount = (Prod.StockAmount + move.Qtd) + movement.Qtd;
                    }
                }
                else
                {
                    if (movement.Move == eMove.saida)
                    {
                        int diference = movement.Qtd - move.Qtd;
                        Prod.StockAmount = Prod.StockAmount - diference;
                    }
                    else
                    {
                        int diference = movement.Qtd - move.Qtd;
                        Prod.StockAmount = Prod.StockAmount + diference;
                    }
                }

                if (Prod.StockAmount < 0)
                {
                    return ResponseModel.BuildErrorResponse("Quantidade acima do estoque");
                }
                else
                {
                    move.ProductName = movement.ProductName;
                    move.Qtd = movement.Qtd;
                    move.Move = movement.Move;
                    await _db.SaveChangesAsync();
                    NotificationModel notification = new NotificationModel()
                    {
                        CreationUserId = move.UserId.ToString(),
                        MoveId = move.Id,
                        Type = eType.edicaoMovimentacao,

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

                    return ResponseModel.BuildOkResponse("Movimentação atualizado com sucesso!");

                }
               
            }
            catch (Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetMovementByMove(eMove Move)
        {
            try
            {
                var filteredProducts = _db.Movemments;
                var result = await filteredProducts.Where(p => p.Move == Move).ToListAsync();

                return ResponseModel.BuildOkResponse(result);
            }
            catch (Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetMovements(int skip, int take, string? search, eMove? move, int? month)
        {
            try
            {
                var movements = await _db.Movemments.AsNoTracking().OrderByDescending(x => x.Date).ThenBy(x => x.ProductName).Include(x => x.Product).ToListAsync();

                movements = !string.IsNullOrEmpty(search) ? movements.Where(x => x.ProductName1.Contains(search)).ToList() : movements;

                movements = move != null && move == eMove.saida ? movements.Where(x => x.Move == eMove.saida).ToList() : movements;

                movements = move != null && move == eMove.entrada ? movements.Where(x => x.Move == eMove.entrada).ToList() : movements;

                movements = month != null ? movements.Where(x=>x.Date.Month==month).ToList() : movements;

                var totalPages = Math.Ceiling(movements.Count() / (decimal)take);
                var result = movements.Skip(skip).Take(take);

                return ResponseModel.BuildOkResponse(new { movements = result, totalPages = totalPages });
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> SearchByProdName(string Keyword)
        {
            try
            {
                var filteredProducts = _db.Movemments;
                var result = string.IsNullOrEmpty(Keyword) ? await filteredProducts.ToListAsync() : await filteredProducts.Where(p => p.ProductName.ToLower().Contains(Keyword.ToLower())).ToListAsync();

                return ResponseModel.BuildOkResponse(result);
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetSalesPerMonthAsync(DateTime filteredMonth, eMove moveType, Guid? prodId, int monthsfiltered = 3)
        {
            try

            {

                var results = _db.Movemments.Where(x => x.Date.Date >= filteredMonth.AddMonths(monthsfiltered * -1).Date && x.Date.Date <= filteredMonth.Date && x.Date.Year == filteredMonth.Year);
                results = prodId != null ? results.Where(x => x.ProductId == prodId) : results;
                results = moveType == eMove.entrada ? results.Where(x => x.Move == eMove.entrada) : results.Where(x => x.Move == eMove.saida);

                List<int> data = new List<int>();

                int currentMonth = 0;

                for (int months = 0; months < monthsfiltered; months++)
                {
                    currentMonth = filteredMonth.AddMonths(-months).Month;
                    data.Add(results.Where(x => x.Date.Month == currentMonth).Sum(x=>x.Qtd));
                }
                data.Reverse();

                return ResponseModel.BuildOkResponse(data);
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetEarnings(DateTime filteredMonth, Guid? prodId)
        {
            try

            {

                var results = _db.Movemments.Where(x => x.Date.Year == filteredMonth.Year);
                results = prodId != null ? results.Where(x => x.ProductId == prodId) : results;
                var sale = results.Where(x => x.Move == eMove.saida);
                var entry = results.Where(x => x.Move == eMove.entrada);

                int currentMonth = 0;
                double earning = 0;
                List<double> data = new List<double>();
                List<double> datasale = new List<double>();
                List<double> dataentry = new List<double>();

                for (int months = 1; months <= 12; months++)
                {
                    currentMonth = months;
                    var salesum = sale.Where(x => x.Date.Month == currentMonth).Sum(x => x.PriceTotal);
                    var entrysum = entry.Where(x => x.Date.Month == currentMonth).Sum(x => x.PriceTotal);
                    earning = salesum - entrysum;
                    data.Add(earning);
                    datasale.Add(salesum);
                    dataentry.Add(entrysum);
                  

                }


                return ResponseModel.BuildOkResponse(new { earnings = data, sales = datasale.Sum(), entry = dataentry.Sum() });
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }


        #region excel
        public async Task<ResponseModel> GetExcelFile(Guid? productId, DateTime filteredMonth)
        {
            var results = await _db.Movemments.Where(x => x.Date.Year == filteredMonth.Year).ToListAsync();
            results = productId != null ? results.Where(x => x.ProductId == productId).ToList(): results;
            var sale = results.Where(x => x.Move == eMove.saida);
            var entry = results.Where(x => x.Move == eMove.entrada);

            var productName = productId != null ? results.FirstOrDefault(x => x.ProductId == productId).ProductName : null;
            Guid idFile = Guid.NewGuid();  
           
        
            
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Relatorio de vendas");
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue(productName!=null?productName:"todos");
            headerRow.CreateCell(1).SetCellValue("Mês");
            headerRow.CreateCell(2).SetCellValue("Quantidade Vendida");
            headerRow.CreateCell(3).SetCellValue("Valor Investido");
            headerRow.CreateCell(4).SetCellValue("Valor Recebido");
            headerRow.CreateCell(5).SetCellValue("Lucro");

            
            double earning = 0;
            List<string> months = new List<string>
            {
                "Jan",
                "Fev",
                "Mar",
                "Abr",
                "Mai",
                "Jun",
                "Jul",
                "Ago",
                "Set",
                "Out",
                "Nov",
                "Dez"
            };

            for (int row = 1; row <= filteredMonth.Month; row++)
            {
                
                string currentMonth = months[row-1];
                var saleSum = sale.Where(x => x.Date.Month == row).Sum(x => x.PriceTotal);
                var entrySum = entry.Where(x => x.Date.Month == row).Sum(x => x.PriceTotal);
                var qtd = sale.Where(x => x.Date.Month == row).Sum(x => x.Qtd);
                earning = saleSum - entrySum;
              

                IRow dataRow = sheet.CreateRow(row);
                dataRow.CreateCell(1).SetCellValue(currentMonth);
                dataRow.CreateCell(2).SetCellValue(qtd);
                dataRow.CreateCell(3).SetCellValue(entrySum);
                dataRow.CreateCell(4).SetCellValue(saleSum);
                dataRow.CreateCell(5).SetCellValue(earning);
            }


           
            ByteArrayOutputStream bos = new ByteArrayOutputStream();
            try
            {
                workbook.Write(bos);
            }
            finally
            {
                bos.Close();
            }
            byte[] bytes = bos.ToByteArray();
            return ResponseModel.BuildOkResponse(  Convert.ToBase64String(bytes));




        }
        public async Task<ResponseModel> GetBarChartExcelFile(Guid? prodId, DateTime filteredMonth, int monthsfiltered = 3)
        {
            var results = _db.Movemments.Where(x => x.Date.Date >= filteredMonth.AddMonths(monthsfiltered * -1).Date && x.Date.Date <= filteredMonth.Date && x.Date.Year == filteredMonth.Year);
            results = prodId != null ? results.Where(x => x.ProductId == prodId) : results;
            var sale = results.Where(x => x.Move == eMove.saida);
            var entry = results.Where(x => x.Move == eMove.entrada);

            var productName = prodId != null ? results.FirstOrDefault(x => x.ProductId == prodId).ProductName : null;
            Guid idFile = Guid.NewGuid();

            int currentMonth = 0;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Relatorio de vendas");
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue(productName != null ? productName : "todos");
            headerRow.CreateCell(1).SetCellValue("Mês");
            headerRow.CreateCell(2).SetCellValue( "adicionadas");
            headerRow.CreateCell(3).SetCellValue("vendidas");



            double earning = 0;
            List<string> months = new List<string>
            {
                "Jan",
                "Fev",
                "Mar",
                "Abr",
                "Mai",
                "Jun",
                "Jul",
                "Ago",
                "Set",
                "Out",
                "Nov",
                "Dez"
            };
            for (int recentMonths = 0; recentMonths < monthsfiltered; recentMonths++)
            {
                currentMonth = filteredMonth.AddMonths(-recentMonths).Month;
                string monthName = months[currentMonth - 1];
                var qtdSale = sale.Where(x => x.Date.Month == currentMonth).Sum(x => x.Qtd);
                var qtdEntry = entry.Where(x => x.Date.Month == currentMonth).Sum(x => x.Qtd);
                IRow dataRow = sheet.CreateRow(recentMonths+1);
                dataRow.CreateCell(1).SetCellValue(monthName);
                dataRow.CreateCell(2).SetCellValue(qtdEntry);
                dataRow.CreateCell(3).SetCellValue(qtdSale);
            }

            ByteArrayOutputStream bos = new ByteArrayOutputStream();
            try
            {
                workbook.Write(bos);
            }
            finally
            {
                bos.Close();
            }
            byte[] bytes = bos.ToByteArray();
            return ResponseModel.BuildOkResponse(Convert.ToBase64String(bytes));




        }
        #endregion
    }


}
