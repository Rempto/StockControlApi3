using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using StockControlApi.Domain.Enums;

namespace StockControlApi.Domain.Entities
{
    public class User : IdentityUser<string>
    {
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public string Password { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        public eUserPermission Permission { get; set; }
        public List<Product>? Products { get; set; }

        public List<Movement>? Movements { get; set; }
         public List<Notification>? Notifications { get; set; }

        public string userPermission
        {
            get
            {
                switch (Permission)
                {
                    case eUserPermission.admin:
                        return "admin";
                    case eUserPermission.operador:
                        return "operador";
                    case eUserPermission.visualizador:
                        return "visualizador";
                    default:
                        return "none";
                }
            }
        }

    }
}
