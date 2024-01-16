using StockControlApi.Application.Models;
using StockControlApi.Application.Services;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Enums;
using StockControlApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace StockControlApi.Controller
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize("Bearer")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAsync(int skip, int take, string userId)
        {
            return new ResponseHelper().CreateResponse(await _notificationService.GetNotifications(skip,take,userId));
        }
        [HttpGet]
        [Route("get-by-id")]
        public async Task<IActionResult> GetUserNotification(string id)
        {
            return new ResponseHelper().CreateResponse(await _notificationService.GetUserNotifications(id));
        }

        [HttpPut]
        [Route("set-visualized")]
        public async Task<IActionResult> setVisualized(Guid id)
        {
            return new ResponseHelper().CreateResponse(await _notificationService.setVisualized(id));
        }

        [HttpPut]
        [Route("set-all-visualized")]
        public async Task<IActionResult> setAllVisualized(string userId)
        {
            return new ResponseHelper().CreateResponse(await _notificationService.setAllVisualized(userId));
        }

    }
}
