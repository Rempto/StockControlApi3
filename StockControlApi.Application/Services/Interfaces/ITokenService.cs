using StockControlApi.Application.Models;
using StockControlApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Services.Interfaces
{
    public interface ITokenService
    {
         Task<LoginModel> GenerateToken(User user);
         int? ValidateToken(string token);
    }
}
