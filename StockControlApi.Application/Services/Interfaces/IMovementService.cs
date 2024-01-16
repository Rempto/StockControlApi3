using StockControlApi.Application.Models;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Services.Interfaces
{
    public interface IMovementService
    {
        Task<ResponseModel> GetAsync();
        Task<ResponseModel> CreateMovement(MovementModel movement);
        Task<ResponseModel> DeleteMovement(Guid Id, string userId);
        Task<ResponseModel> GetMovementById(Guid Id);
        Task<ResponseModel> GetMovements(int skip, int take, string? search, eMove? move, int? month);
        Task<ResponseModel> SearchByProdName(string Keyword);
        Task<ResponseModel> GetMovementByMove(eMove Move);
        Task<ResponseModel> GetSalesPerMonthAsync(DateTime filteredMonth, eMove moveType, Guid? prodId, int monthsfiltered = 3);
        Task<ResponseModel> GetEarnings(DateTime filteredMonth, Guid? prodId);
        Task<ResponseModel> GetExcelFile(Guid? productId, DateTime filteredMouth);
        Task<ResponseModel> GetBarChartExcelFile(Guid? prodId, DateTime filteredMonth, int monthsfiltered = 3);
        Task<ResponseModel> UpdateMovement(ReturnMovementModel movement);
    }
}
