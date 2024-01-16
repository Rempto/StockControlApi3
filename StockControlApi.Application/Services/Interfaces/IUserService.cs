using StockControlApi.Application.Models;
using StockControlApi.Domain.Entities;
using StockControlApi.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseModel> GetAsync(string? id);
        Task<ResponseModel> CreateUser(UserModel client);
        Task<ResponseModel> Login(UserModel client);
        Task<ResponseModel> Logout(UserModel client);

        Task<ResponseModel> ChangePassword(string email, string oldPassword, string newPassword);
        Task<ResponseModel> ForgotPassword(string email);
        Task<ResponseModel> ChangeEmailPassword(string id, string newPassword);
        Task<ResponseModel> UploadAvatar(AvatarModel model);
        Task<ResponseModel> GetUserById(string Id);
        Task<ResponseModel> UpdateUserPermission(UserModel user);
        Task<ResponseModel> GetUsersByName(string? search);
    }
}
