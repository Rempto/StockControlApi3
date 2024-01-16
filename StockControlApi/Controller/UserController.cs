using StockControlApi.Application.Models;
using StockControlApi.Application.Services;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using StockControlApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Principal;

namespace StockControlApi.Controller
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize("Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAsync(string? id)
        {
            return new ResponseHelper().CreateResponse(await _userService.GetAsync(id));
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> CreateUser(UserModel user)
        {
            return new ResponseHelper().CreateResponse(await _userService.CreateUser(user));
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserModel user)
        {
            return new ResponseHelper().CreateResponse(await _userService.Login(user));
        }
        [HttpPut]
        [Route("edit-password")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdatePassword(string email,string oldPassword,string newPassword)
        {
            return new ResponseHelper().CreateResponse(await _userService.ChangePassword(email,oldPassword,newPassword));
        }
        [HttpGet]
        [Route("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromQuery]string email)
        {
           return new ResponseHelper().CreateResponse(await _userService.ForgotPassword(email));
        }
        [HttpPut]
        [Route("edit-email-password")]
        public async Task<IActionResult> UpdateEmailPassword(string id, string newPassword)
        {
            return new ResponseHelper().CreateResponse(await _userService.ChangeEmailPassword(id, newPassword));
        }
        [HttpPost]
        [Route("update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm]AvatarModel model) //AvatarModel model
        {
            return new ResponseHelper().CreateResponse(await _userService.UploadAvatar(model));
        }
        [HttpGet]
        [Route("get-by-id")]
        public async Task<IActionResult> GetUserById(string id)
        {
            return new ResponseHelper().CreateResponse(await _userService.GetUserById(id));
        }
        [HttpGet]
        [Route("get-user-by-name")]
        public async Task<IActionResult> GetUserByName(string? search)
        {
            return new ResponseHelper().CreateResponse(await _userService.GetUsersByName(search));
        }

        [HttpPut]
        [Route("edit-permission")]
        public async Task<IActionResult> UpdatePermission(UserModel user)
        {
            return new ResponseHelper().CreateResponse(await _userService.UpdateUserPermission(user));
        }
    }
}
