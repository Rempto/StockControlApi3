using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Models
{
    public class AvatarModel
    {
        public string UserId { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
