using StockControlApi.Application.Models;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Persistence.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using StockControlApi.Domain.Enums;

namespace StockControlApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IHostingEnvironment _webHostEnvironment;
        private SignInManager<User> _signManager;
        private UserManager<User> _userManager;
        private ITokenService _tokenService;
        private  IEmailService _emailService;
        public UserService(AppDbContext db, SignInManager<User> signManager, UserManager<User> userManager, ITokenService tokenService, IEmailService emailService, IHostingEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _signManager = signManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
           
        }

        public async Task<ResponseModel> GetAsync(string? id)
        {
            var users = await _db.Users.ToListAsync();
            users = id != null ? users.Where(x=>x.Id !=id).ToList() : users;

            return ResponseModel.BuildOkResponse(users);
        }
        public async Task<ResponseModel> CreateUser(UserModel client)
        {
            try
            {
                var userDB = await _db.Users.FirstOrDefaultAsync(x => x.Email == client.Email);
                //await _userManager.FindByEmailAsync(client.Email);

                if (userDB != null)
                {
                    return ResponseModel.BuildConflictResponse("usuario ja cadastrado");
                }
                else
                {
                    var account = new User { Name = client.Name, Email = client.Email, EmailConfirmed = true, UserName = client.Email, Permission = client.Permission };

                    var result = await _userManager.CreateAsync(account, client.Password);

                    if (result.Succeeded)
                    {
                        //await _signManager.SignInAsync(account, false);
                        return ResponseModel.BuildOkResponse("Usuario criado com sucesso");

                    }
                    else
                    {
                        return ResponseModel.BuildErrorResponse("usuario nao cadastrado");
                    }
                }
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }

        }

        public async Task<ResponseModel> Login(UserModel client)
        {
            try
            {
                var userDB = await _db.Users.FirstOrDefaultAsync(x => x.Email == client.Email);
                if (userDB != null)
                {
                    //
                    //var account = new User { Name = userDB.Name, Password = client.Password, Email = client.Email };
                    var result = await _signManager.PasswordSignInAsync(userDB,
                    client.Password, false, false);
                    UserReturnModel UserReturn = new UserReturnModel() { Email = userDB.Email, Name = userDB.Name, Id = userDB.Id, Permission = userDB.Permission };
                    LoginModel Login = new LoginModel();
                    Login.User = UserReturn;

                    if (result.Succeeded)
                    {
                        var model = await _tokenService.GenerateToken(userDB);
                        Login.Token = model.Token;
                        Login.DateExpires = model.DateExpires;
                        return ResponseModel.BuildOkResponse(Login);
                    }
                    else
                    {
                        return ResponseModel.BuildErrorResponse("usuario ou senha incorretos");
                    }
                }
                else
                {
                    return ResponseModel.BuildErrorResponse("usuario nao cadastrado");
                }
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> ChangePassword(string email, string oldPassword, string newPassword)
        {
            try
            {
                var userDB = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);


                var passwordCheck = await _userManager.CheckPasswordAsync(userDB, oldPassword);
                if (!passwordCheck)
                {
                    return ResponseModel.BuildErrorResponse("senha incorreta");
                }
                var result = await _userManager.ChangePasswordAsync(userDB, oldPassword, newPassword);

                if (result.Succeeded)
                {
                    await _signManager.SignInAsync(userDB, isPersistent: false);
                    return ResponseModel.BuildOkResponse("senha atualizada com sucesso!");
                }
                else
                {
                    return ResponseModel.BuildErrorResponse("senha incorreta");
                }


            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> Logout(UserModel client)
        {
            return ResponseModel.BuildOkResponse("nao implementado");
        }

        public async Task<ResponseModel> ForgotPassword(string email)
        {
            try
            {
                var userDB = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (userDB != null)
                {

                   await  _emailService.SendPasswordResetEmailAsync(email,userDB.Id);
                        return ResponseModel.BuildOkResponse("email enviado");
                   }
                   else
                   {
                       return ResponseModel.BuildErrorResponse("email não cadastrado");
                  }
                
           }
            catch (Exception e)
           {
               return ResponseModel.BuildErrorResponse(e);
           }
       }

        public async Task<ResponseModel> ChangeEmailPassword(string id, string newPassword)
        {
            try
            {
                var userDB = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (userDB == null)
                {
                    return ResponseModel.BuildNotFoundResponse("usuario não encontrado");
                }

                userDB.PasswordHash=_userManager.PasswordHasher.HashPassword(userDB,newPassword);


                var result = await _userManager.UpdateAsync(userDB);

                if (result.Succeeded)
                {
                    await _signManager.SignInAsync(userDB, isPersistent: false);
                    return ResponseModel.BuildOkResponse("senha atualizada com sucesso!");
                }
                else
                {
                    return ResponseModel.BuildErrorResponse(" ");
                }


            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> UploadAvatar(AvatarModel model)
        {
            try
            {
               

                if (model.Avatar == null)
                {
                    ResponseModel.BuildErrorResponse("");
                }
                var userDB = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
                string savePath = _webHostEnvironment.WebRootPath + "\\images\\";
                string newNameFile = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;
                if(!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
          

                using (var stream = System.IO.File.Create(savePath + newNameFile))
                {
                    await model.Avatar.CopyToAsync(stream);
                   
                }
                userDB.Avatar = "https://localhost:44313/images/" + newNameFile;
                _db.Update(userDB);
               
                await _db.SaveChangesAsync();

                return ResponseModel.BuildOkResponse("imagem atualizada com sucesso!");
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetUserById(string Id)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == Id);
                return ResponseModel.BuildOkResponse(user);
            }
            catch (Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }

        public async Task<ResponseModel> UpdateUserPermission(UserModel user)
        {
            try
            {

                var client = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
                client.Permission = user.Permission;
                UserReturnModel UserReturn = new UserReturnModel() { Email = client.Email, Name = client.Name, Id = client.Id, Permission = client.Permission };
                _db.Update(client);
                await _db.SaveChangesAsync();

                return ResponseModel.BuildOkResponse(new { msg="Permissão de usuario atualizada!", user=UserReturn });
            }
            catch (Exception e)
            {

                return ResponseModel.BuildErrorResponse(e);
            }
        }
        public async Task<ResponseModel> GetUsersByName( string? search)
        {
            try
            {
                var user = await _db.Users.AsNoTracking().ToListAsync();
                user = !string.IsNullOrEmpty(search) ? user.Where(x => x.Name.ToLower().Replace(" ","").Contains(search.ToLower().Replace(" ", ""))).ToList() : user;

                return ResponseModel.BuildOkResponse(user);
            }
            catch (Exception e)
            {
                return ResponseModel.BuildErrorResponse(e);
            }
        }


    }
}

