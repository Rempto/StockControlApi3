using StockControlApi.Application;
using StockControlApi.Application.Models;
using StockControlApi.Application.Services;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using StockControlApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace StockControlApi.Controller
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize("Bearer")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _movementService;

        public MovementController(IMovementService movementService)
        {
            _movementService = movementService;
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAsync()
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetAsync());
        }

        [HttpPost]
        [Route("post")]
        public async Task<IActionResult> CreateMovement(MovementModel movement)
        {
            return new ResponseHelper().CreateResponse(await _movementService.CreateMovement(movement));
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteMovement(Guid Id, string userId)
        {
            return new ResponseHelper().CreateResponse(await _movementService.DeleteMovement(Id,userId));
        }

        [HttpGet]
        [Route("getbyid")]
        public async Task<IActionResult> GetMovementById(Guid id)
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetMovementById(id));
        }

        [HttpGet]
        [Route("getmovements")]
        public async Task<IActionResult> GetProducts(int skip, int take, string? search, eMove? move, int? month)
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetMovements(skip, take,search,move,month));
        }
        [HttpPut]
        [Route("edit")]
        public async Task<IActionResult> UpdateMovement(ReturnMovementModel model)
        {
            return new ResponseHelper().CreateResponse(await _movementService.UpdateMovement(model));
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetByName(string Keyword)
        {
            return new ResponseHelper().CreateResponse(await _movementService.SearchByProdName(Keyword));
        }
        [HttpGet]
        [Route("getbymovement")]
        public async Task<IActionResult> Getbymovement(eMove move)
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetMovementByMove(move));
        }
        [HttpGet]
        [Route("get-graphs-by-date")]
        public async Task<IActionResult> GetSalesPerMonth([FromQuery]DateTime filteredMonth, eMove moveType, Guid? prodId)
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetSalesPerMonthAsync(filteredMonth, moveType,prodId));
        }

        [HttpGet]
        [Route("get-graphs-earnings")]
        public async Task<IActionResult> GetEarnings([FromQuery] DateTime filteredMonth, Guid? prodId)
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetEarnings(filteredMonth, prodId));
        }
        [HttpGet]
        [Route("get-excel-file")]
        public async Task<IActionResult> GetExcelFile( DateTime filteredMonth, Guid? productId)
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetExcelFile(productId, filteredMonth));
        }

        [HttpGet]
        [Route("get-bar-chart-excel")]
        public async Task<IActionResult> GetBarChartExcelFile(Guid? prodId, DateTime filteredMonth)
        {
            return new ResponseHelper().CreateResponse(await _movementService.GetBarChartExcelFile(prodId, filteredMonth));
        }
    }
}
