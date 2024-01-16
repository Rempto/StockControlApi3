using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace StockControlApi.Controller
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize("Bearer")]
    public class PushNotificationController : ControllerBase
    {
        private readonly IPushNotificationService _pushNotification;

        public PushNotificationController(IPushNotificationService pushNotification)
        {
            _pushNotification = pushNotification;
        }
        [HttpPost]
        [Route("create-push-notification")]
        public async Task<IActionResult> CreatePushNotification(string token, string userId)
        {
            return new ResponseHelper().CreateResponse(await _pushNotification.CreatePushNotification(token, userId));
        }
    } }
