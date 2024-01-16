using StockControlApi.Application;
using Microsoft.AspNetCore.Mvc;

namespace StockControlApi.Helpers
{
    public class ResponseHelper : ControllerBase
    {
        public IActionResult CreateResponse(ResponseModel response) => response.Status switch
        {
            200 => Ok(String.IsNullOrEmpty(response.Message) ? response.Content : response.Message),
            500 => StatusCode(500, String.IsNullOrEmpty(response.Message) ? response.Content : response.Message),
            422 => UnprocessableEntity(String.IsNullOrEmpty(response.Message) ? response.Content : response.Message),
            409 => Conflict(String.IsNullOrEmpty(response.Message) ? response.Content : response.Message),
            401 => Unauthorized(String.IsNullOrEmpty(response.Message) ? response.Content : response.Message),
            403 => Forbid(response.Message),
            404 => NotFound(String.IsNullOrEmpty(response.Message) ? response.Content : response.Message),
            _ => StatusCode(400, response.Message),
        };
    }
}
